//=============================================================================
// @File	: [ImGuiTest.cs]
// @Brief	: ImGuiTestクラス
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/09	スクリプト作成
//=============================================================================

using ImGuiNET;
#if !UIMGUI_REMOVE_IMNODES
#endif
#if !UIMGUI_REMOVE_IMPLOT
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;
using System.Reflection;
using System;
#endif
#if !UIMGUI_REMOVE_IMGUIZMO
using ImGuizmoNET;
#endif
using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
using Quaternion = UnityEngine.Quaternion;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace UImGui
{
	public class ImGuiTest : MonoBehaviour
	{
		[SerializeField]
		private bool bMode;

#if !UIMGUI_REMOVE_IMPLOT
		private Vector4 _myColor;
		private bool _isOpen;

#endif
		[SerializeField]
		private bool bShowDemoWindow;
		[SerializeField]
		private bool bShowHierarchy;
		[SerializeField]
		private bool bShowInspector;

		private Camera mainCamera;
		private GameObject SelectObj;
		private string ObjectName;

		[SerializeField]
		private List<GameObject> gameObjects = new List<GameObject>();

		private OPERATION mCurrentGizmoOperation = OPERATION.TRANSLATE;
		private ImGuizmoNET.MODE mCurrentGizmoMode = ImGuizmoNET.MODE.LOCAL;

		/// <summary>
		/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
		/// </summary>
		void Awake()
		{
			mainCamera = Camera.main;
			GetGameObject();
		}

		/// <summary>
		/// ヒエラルキービューに変更があったときのコールバック
		/// </summary>
		void OnHierarchyChange()
		{
			if (!gameObjects.Equals(UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject))))
			{
				Debug.Log($"OnHierarchyChange");
				GetGameObject();
			}

		}

		/// <summary>
		/// 最初のフレーム更新の前に呼び出される
		/// </summary>
		void Start()
		{

		}

		/// <summary>
		/// 1フレームごとに呼び出される（端末によって呼び出し回数が異なる）：inputなどの入力処理
		/// </summary>
		void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				if (Input.GetKeyDown(KeyCode.F12))
				{
					bMode = !bMode;
				}
			}

			// もしデバッグモードでない場合実行しない
			if (!bMode) { return; }

			if (!gameObjects.Equals(UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject))))
			{
				GetGameObject();
			}
		}


		private void OnEnable()
		{
			UImGuiUtility.Layout += OnLayout;
		}

		private void OnDisable()
		{
			UImGuiUtility.Layout -= OnLayout;
		}

		private void OnLayout(UImGui uImGui)
		{
			// もしデバッグモードでない場合実行しない
			if (!bMode) { return; }

			// メインメニューバー
			if (ImGui.BeginMainMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("Debug View Exit", "Shift+F12")) { bMode = false; }
					ImGui.EndMenu();
				}
				if (ImGui.BeginMenu("Window"))
				{
					if (ImGui.MenuItem("Demo", null, bShowDemoWindow)) { bShowDemoWindow = !bShowDemoWindow; }
					if (ImGui.MenuItem("Hierarchy", null, bShowHierarchy)) { bShowHierarchy = !bShowHierarchy; }
					if (ImGui.MenuItem("Inspector", null, bShowInspector)) { bShowInspector = !bShowInspector; }
					ImGui.EndMenu();
				}


				ImGui.EndMainMenuBar();
			}

			if (bShowDemoWindow)
				ImGui.ShowDemoWindow();


			// ヒエラルキービュー
			if (bShowHierarchy)
			{
				ImGui.Begin("Hierarchy", ref _isOpen, ImGuiWindowFlags.MenuBar);
				{
					// メニューバー
					//if (ImGui.BeginMenuBar())
					//{
					//	if (ImGui.BeginMenu("File"))
					//	{
					//		if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
					//		if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
					//		if (ImGui.MenuItem("Close", "Ctrl+W")) { _isOpen = false; }
					//		ImGui.EndMenu();
					//	}
					//	ImGui.EndMenuBar();
					//}

					ImGui.BeginChild("GameObjects");
					{
						CreateHierarchy(gameObjects);
					}
					ImGui.EndChild();
				}
				ImGui.End();
			}

			if (bShowInspector)
			{
				// インスペクタビュー
				ImGui.Begin("Inspector", ref _isOpen, ImGuiWindowFlags.MenuBar);
				{
					if (SelectObj != null)
					{
						//--- アクティブ チェックボックス
						bool active = SelectObj.activeSelf;
						if (ImGui.Checkbox("", ref active))
							SelectObj.SetActive(!SelectObj.activeSelf);

						ImGui.SameLine();   // 改行しない

						//--- Name
						if (ImGui.InputText($"##ObjectName", ref ObjectName, uint.MaxValue, ImGuiInputTextFlags.AlwaysOverwrite))
							SelectObj.name = ObjectName;

						//--- Tag
						// https://baba-s.hatenablog.com/entry/2014/02/25/000000
						int Tag = 0;
						string[] Tags = { $"{SelectObj.tag}" };
						ImGui.Combo("Tag", ref Tag, Tags, Tags.Length);

						ImGui.SameLine();   // 改行しない

						//--- Layer
						int layer = 0;
						string[] layers = { $"{SelectObj.layer}" };
						ImGui.Combo("Layer", ref layer, layers, layers.Length);


						//--- Component
						UnityEngine.Component[] components = SelectObj.GetComponents<UnityEngine.Component>();

						for (int i = 0; i < components.Length; i++)
						{
							if (ImGui.CollapsingHeader($"{components[i].GetType().Name}##{i}", ImGuiTreeNodeFlags.DefaultOpen))
							{
								// 名前空間に"UnityEngine"が含まれる場合
								if (components[i].GetType().FullName.Contains("UnityEngine"))
								{
									ShowUnityComponents(components[i]);
								}

								// その他の場合（アセットや自作のクラス）
								else
								{
									// 型を取得
									Type t = components[i].GetType();
									// 取得した型のメンバを全取得する
									var members = t.GetFields(
										BindingFlags.Public |       // パブリックメンバを検索の対象に加える。
										BindingFlags.NonPublic |    // パブリックでないメンバを検索の対象に加える。
										BindingFlags.Instance |     // 非静的メンバ（インスタンスメンバ）を検索の対象に加える。
										BindingFlags.Static |       // 静的メンバを検索の対象に加える。
										BindingFlags.DeclaredOnly   // 継承されたメンバを検索の対象にしない。
										);

									foreach (var member in members)
									{
										//--- アクセス修飾子がPublicの場合
										if (member.IsPublic)
										{
											ShowComponents(member, components[i], false);
										}

										else
										{
											bool IsSerializable;
											var attributes = member.GetCustomAttributes(true);

											if (attributes.Any(attr => attr is NonSerializedAttribute))
												IsSerializable = false;
											else if (member.IsPrivate && !attributes.Any(attr => attr is SerializeField))
												IsSerializable = false;
											else
												IsSerializable = member.FieldType.IsSerializable;

											ShowComponents(member, components[i], IsSerializable);
										}

									}
								}
							}
						}

					}
				}
				ImGui.End();
			}

			// Gizmo
			if (SelectObj != null)
			{
				if (Input.GetKeyDown(KeyCode.T))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.TRANSLATE;
				if (Input.GetKeyDown(KeyCode.E))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.ROTATE;
				if (Input.GetKeyDown(KeyCode.R))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.SCALE;
				//if (Input.GetKeyDown(KeyCode.V))
				//	mCurrentGizmoOperation = OPERATION.BOUNDS;

				ImGuizmoDemo(SelectObj);
			}
		}

		protected virtual void OnSceneGUI()
		{
			EditorGUI.BeginChangeCheck();
			Vector3 newTargetPosition = Handles.PositionHandle(SelectObj.transform.position, Quaternion.identity);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(SelectObj, "Change Look At Target Position");
				SelectObj.transform.position = newTargetPosition;
				//SelectObj.Update();
			}
		}

		// シーン上のゲームオブジェクト一覧取得
		private void GetGameObject()
		{
			gameObjects.Clear();

			foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
			{
				// アセットからパスを取得.シーン上に存在するオブジェクトの場合,シーンファイル（.unity）のパスを取得.
				string path = AssetDatabase.GetAssetOrScenePath(obj);
				// シーン上に存在するオブジェクトかどうか文字列で判定.
				bool isScene = path.Contains(".unity");
				// シーン上に存在するオブジェクトならば処理.
				if (isScene)
				{
					gameObjects.Add(obj);       // 最後
				}
			}
		}

		// ヒエラルキーリスト作成
		private void CreateHierarchy(List<GameObject> Objs)
		{
			List<GameObject> Hierarchy = new List<GameObject>();

			for (int i = 0; i < Objs.Count; i++)
			{
				if (Objs[i].transform.parent == null)
				{
					Hierarchy.Add(Objs[i]);
				}
			}

			for (int i = 0; i < Hierarchy.Count; i++)
			{
				if (Hierarchy[i].name == "AdaptivePerformanceManager")
					continue;

				GetChild(Hierarchy[i].transform);
			}
		}

		// 子オブジェクト取得
		private void GetChild(Transform transform)
		{
			ImGuiTreeNodeFlags node_flags;

			// 子オブジェクトを持っているか
			if (transform.childCount > 0)
				node_flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth;
			else
				node_flags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanAvailWidth;

			// オブジェクトが非アクティブか
			if (!transform.gameObject.activeSelf)
				ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

			if (ImGui.TreeNodeEx($"{transform.name}##{transform.GetInstanceID()}", node_flags))     // ##transform.GetInstanceID()でオブジェクトの名前が被っても大丈夫になる
			{
				// クリックされた時
				if (ImGui.IsItemClicked())
				{
					SelectObj = transform.gameObject;
					ObjectName = transform.name;
				}

				if (transform.childCount > 0)
				{
					foreach (Transform child in transform)
					{
						GetChild(child);
					}
				}
				ImGui.TreePop();
			}
			if (!transform.gameObject.activeSelf)
				ImGui.PopStyleColor(1);
		}

		//private unsafe ImGuiIO* io = ImGuiNative.igGetIO();
		private /*unsafe*/ void ImGuizmoDemo(GameObject gameObject)
		{
			//ImGuizmo.BeginFrame();

			//ImGuizmoNative.ImGuizmo_SetOrthographic(true);
			ImGuizmo.SetRect(0, 0, Screen.width, Screen.height);

			ImGuizmoNET.ImGuizmo.IsUsing();
			ImGuizmo.SetGizmoSizeClipSpace(0.15f);

			Matrix4x4 matrix = gameObject.transform.localToWorldMatrix;
			Matrix4x4 view = mainCamera.worldToCameraMatrix;
			Matrix4x4 projection = mainCamera.projectionMatrix;


			//Vector3 matrixTranslation, matrixRotation, matrixScale;
			//ImGuizmo.DecomposeMatrixToComponents(ref matrix.m00, ref matrixTranslation, ref matrixRotation, ref matrixScale);
			//ImGui.InputFloat3("Tr", ref matrixTranslation);
			//ImGui.InputFloat3("Rt", ref matrixRotation);
			//ImGui.InputFloat3("Sc", ref matrixScale);
			//ImGuizmo.RecomposeMatrixFromComponents(matrixTranslation, matrixRotation, matrixScale, matrix);

			//ImGuizmoNET.ImGuizmo.AllowAxisFlip(SelectObj != null);
			//ImGuizmoNET.ImGuizmo.DrawGrid(ref view.m00, ref projection.m00, ref matrix.m00, 50f);
			//ImGuizmoNET.ImGuizmo.DrawCubes(ref view.m00, ref projection.m00, ref matrix.m00, 1); //(Debug)

			ImGuizmo.Manipulate(ref view.m00, ref projection.m00, mCurrentGizmoOperation, mCurrentGizmoMode, ref matrix.m00);
			//ImGuizmo.DrawGrid(ref view.m00, ref projection.m00, ref matrix.m00, 50f);


			//ImGuiIOPtr ioPtr = ImGui.GetIO();
			//float viewManipulateRight = ioPtr.DisplaySize.x;
			//float viewManipulateTop = 0;
			//ImGuizmo.ViewManipulate(ref view.m00, 2, new Vector2(viewManipulateRight - 128, viewManipulateTop), new Vector2(128, 128), 0x10101010);

			//gameObject.transform.localToWorldMatrix = matrix;

			//Matrix4x4 iview = Matrix4x4.Inverse(view);
			//mainCamera.transform.localRotation = iview.rotation;
			//mainCamera.transform.localPosition = iview.GetPosition();
		}

		// コンポーネント別(UnityEngine)処理
		void ShowUnityComponents(UnityEngine.Component compo)
		{
			Type type = compo.GetType();
			if (type == typeof(Transform))
			{
				Transform t = compo as Transform;
				ShowTransform(t);
			}
			else if (type == typeof(Camera))
			{
				Camera c = compo as Camera;
				ShowCamera(c);
			}
			else if (type == typeof(AudioListener))
			{

			}
			else if (type == typeof(Light))
			{

			}
			else if (type == typeof(MeshFilter))
			{
				MeshFilter filt = compo as MeshFilter;
				ShowMeshFilter(filt);
			}
			else if (type == typeof(MeshRenderer))
			{

			}
			else if (type == typeof(Rigidbody))
			{
				Rigidbody rb = compo as Rigidbody;
				ShowRigidBody(rb);
			}
			else if (type == typeof(Rigidbody2D))
			{

			}
			else if (type == typeof(Collider) ||
				type == typeof(BoxCollider) ||
				type == typeof(CapsuleCollider) ||
				type == typeof(SphereCollider) ||
				type == typeof(MeshCollider) ||
				type == typeof(TerrainCollider) ||
				type == typeof(WheelCollider))
			{
				Collider collider = compo as Collider;
				ShowCollider(collider);
			}
			else if (type == typeof(Collider2D))
			{
				Collider2D collider = compo as Collider2D;
				ShowCollider2D(collider);
			}


			// UI
			//UnityEngine.Canvas
			//UnityEngine.UI.CanvasScaler
			//UnityEngine.UI.GraphicRaycaster
			//UnityEngine.UI.Image
			//UnityEngine.EventSystems.EventSystem

			////UnityEngine.RectTransform:
			else
			{
				ImGui.Text($"\"{compo.GetType().Name}\" component is not supported.");
			}
		}

		// コンポーネント別処理
		void ShowComponents(FieldInfo info, object type, bool IsSerializable)
		{
			Type t = info.FieldType;
			if (t == typeof(bool))
			{
				bool b = (bool)info.GetValue(type);
				b = ImCheckBox($"{info.Name}", b);
			}
			else if (t.IsEnum)
			{
				Enum e = (Enum)info.GetValue(type);
				ImCombo(e);
			}
			else if (t == typeof(int))
			{
				int i = (int)info.GetValue(type);
				i = ImVector($"{info.Name}", i);
			}
			else if (t == typeof(float))
			{
				float f = (float)info.GetValue(type);
				f = ImVector($"{info.Name}", f);
			}
			else if (t == typeof(Vector2))
			{
				Vector2 v = (Vector2)info.GetValue(type);
				v = ImVector($"{info.Name}", v);
			}
			else if (t == typeof(Vector3))
			{
				Vector3 v = (Vector3)info.GetValue(type);
				v = ImVector($"{info.Name}", v);
			}
			else if (t == typeof(Vector4))
			{
				Vector4 v = (Vector4)info.GetValue(type);
				v = ImVector($"{info.Name}", v);
			}
			else
			{
				ImGui.Text($"\"{t}\" class is not supported.");
				Debug.Log($"\"{t}\" class is not supported.");
			}

		}



		static void ImCombo<T>(T @enum) where T : IComparable
		{
			//if (typeof(T) == typeof(Enum))
			{
				int i = Convert.ToInt32(@enum);
				if (ImGui.Combo($"{@enum.GetType().Name}", ref i, Enum.GetNames(@enum.GetType()), Enum.GetValues(@enum.GetType()).Length))
					@enum = i.ConvertTo<T>();
			}
		}

		static T ImVector<T>(string lavel, T vec, float? speed = 0.01f)
		{
			if (!speed.HasValue)
				speed = 0.01f;

			if (typeof(T) == typeof(int))
			{
				int i = vec.ConvertTo<int>();
				if (ImGui.DragInt(lavel, ref i, speed.Value))
					return i.ConvertTo<T>();
			}
			else if (typeof(T) == typeof(float))
			{
				float f = vec.ConvertTo<float>();
				if (ImGui.DragFloat(lavel, ref f, speed.Value))
					return f.ConvertTo<T>();
			}
			else if (typeof(T) == typeof(Vector2))
			{
				Vector2 f = vec.ConvertTo<Vector2>();
				if (ImGui.DragFloat2(lavel, ref f, speed.Value))
					return f.ConvertTo<T>();
			}
			else if (typeof(T) == typeof(Vector3))
			{
				Vector3 f = vec.ConvertTo<Vector3>();
				if (ImGui.DragFloat3(lavel, ref f, speed.Value))
					return f.ConvertTo<T>();
			}
			else if (typeof(T) == typeof(Vector4))
			{
				Vector4 f = vec.ConvertTo<Vector4>();
				if (ImGui.DragFloat4(lavel, ref f, speed.Value))
					return f.ConvertTo<T>();
			}

			return vec;
		}

		static T ImCheckBox<T>(string lavel, T @boolean)
		{
			if (typeof(T) == typeof(bool))
			{
				bool b = @boolean.ConvertTo<bool>();
				if (ImGui.Checkbox(lavel, ref b))
					return b.ConvertTo<T>();
			}

			return @boolean;
		}



		void ShowTransform(Transform t)
		{
			// pos
			t.localPosition = ImVector("Position", t.localPosition);

			// rot
			t.localEulerAngles = ImVector("Rotation", t.localEulerAngles);

			// scale
			t.localScale = ImVector("Scale", t.localScale);
		}

		void ShowCamera(Camera cam)
		{
			cam.enabled = ImCheckBox($"##Component{cam.GetType().Name}", cam.enabled);

			int i;
			i = (int)cam.clearFlags;
			if (ImGui.Combo("Clear Flags(alpha)", ref i, Enum.GetNames(typeof(CameraClearFlags)), Enum.GetValues(typeof(CameraClearFlags)).Length))
				cam.clearFlags = (CameraClearFlags)i;
		}

		void ShowMeshFilter(MeshFilter f)
		{
			ImGui.Text($"mesh:{f.mesh}");
		}

		void ShowRigidBody(Rigidbody rb)
		{
			rb.mass = ImVector("Mass", rb.mass);
			rb.drag = ImVector("Drag", rb.drag);
			rb.angularDrag = ImVector("Angular", rb.angularDrag);

			rb.useGravity = ImCheckBox($"Use Gravity", rb.useGravity);

			rb.isKinematic = ImCheckBox($"Is Kinematic", rb.isKinematic);

			ImCombo(rb.interpolation);

			ImCombo(rb.collisionDetectionMode);

			if (ImGui.TreeNode("Constaints"))
			{
				ImGui.Text($"Comming Soon...");
				//ImGui.Text($"FreezePosition");
				//ImGui.Text($"FreezeRotation");
				//b = rb.constraints
				//ImGui.SameLine();
				//ImGui.Checkbox($"", );
				ImGui.TreePop();
			}

			if (ImGui.TreeNode("Info"))
			{
				ImGui.Text($"Comming Soon...");

				ImGui.TreePop();
			}

		}

		void ShowCollider(UnityEngine.Collider collider)
		{
			ImGui.TextColored(Color.yellow, "Base");

			Collider c = collider as Collider;
			c.enabled = ImCheckBox("Enabled", c.enabled);
			c.isTrigger = ImCheckBox("IsTrigger", c.isTrigger);
			//c.sharedMaterial

			Type type = collider.GetType();

			if (type == typeof(BoxCollider))
			{
				ImGui.TextColored(Color.yellow, "Box");
				BoxCollider coll = (BoxCollider)c;

				coll.center = ImVector("Center", coll.center);
				coll.size = ImVector("Size", coll.size);
				//ImGui.GetBackgroundDrawList().AddCircle(window_center, window_size.x * 0.6f, IM_COL32(255, 0, 0, 200), 0, 10 + 4);
			}
			else if (type == typeof(CapsuleCollider))
			{
				ImGui.TextColored(Color.yellow, "Capsule");
				CapsuleCollider coll = (CapsuleCollider)c;

				coll.center = ImVector("Center", coll.center);
				coll.radius = ImVector("Radius", coll.radius);
				coll.height = ImVector("Height", coll.height);
				//ImCombo(coll.direction);
			}
			else if (type == typeof(SphereCollider))
			{
				ImGui.TextColored(Color.yellow, "Sphere");
				SphereCollider coll = (SphereCollider)c;

				coll.center = ImVector("Center", coll.center);
				coll.radius = ImVector("Radius", coll.radius);
			}
			else if (type == typeof(MeshCollider))
			{
				ImGui.TextColored(Color.yellow, "Mesh");
				MeshCollider coll = (MeshCollider)c;

				coll.convex = ImCheckBox($"Convex", coll.convex);
				if (coll.convex)
					coll.isTrigger = ImCheckBox($"IsTrigger", coll.isTrigger);
				//coll.cookingOptions
			}
			else if (type == typeof(TerrainCollider))
			{
				ImGui.TextColored(Color.yellow, "Terrain");
				TerrainCollider coll = (TerrainCollider)c;

				//coll.
			}
			else if (type == typeof(WheelCollider))
			{
				ImGui.TextColored(Color.yellow, "Wheel");
				WheelCollider coll = (WheelCollider)c;

				coll.mass = ImVector("Mass", coll.mass);
				coll.radius = ImVector("Radius", coll.radius);
				coll.wheelDampingRate = ImVector("Wheel Damping Rate", coll.wheelDampingRate);
				coll.suspensionDistance = ImVector("Suspension Distance", coll.suspensionDistance);
				coll.forceAppPointDistance = ImVector("Force App Point Distance", coll.forceAppPointDistance);
				coll.center = ImVector("Center", coll.center);

				if (ImGui.TreeNode("Suspension Spring"))
				{
					var ss = coll.suspensionSpring;

					ss.spring = ImVector("Spring", coll.suspensionSpring.spring);
					ss.damper = ImVector("Damper", coll.suspensionSpring.damper);
					ss.targetPosition = ImVector("Target Position", coll.suspensionSpring.targetPosition);

					coll.suspensionSpring = ss;

					ImGui.TreePop();
				}

				if (ImGui.TreeNode("Forward Friction"))
				{
					var ff = coll.forwardFriction;

					ff.extremumSlip = ImVector("Extremum Slip", ff.extremumSlip);
					ff.extremumValue = ImVector("Extremum Value", ff.extremumValue);
					ff.asymptoteSlip = ImVector("Asymptote Slip", ff.asymptoteSlip);
					ff.asymptoteValue = ImVector("Asymptote Value", ff.asymptoteValue);
					ff.stiffness = ImVector("Stiffness", ff.stiffness);

					coll.forwardFriction = ff;

					ImGui.TreePop();
				}

				if (ImGui.TreeNode("Sideways Friction"))
				{
					var sf = coll.sidewaysFriction;

					sf.extremumSlip = ImVector("Extremum Slip", sf.extremumSlip);
					sf.extremumValue = ImVector("Extremum Value", sf.extremumValue);
					sf.asymptoteSlip = ImVector("Asymptote Slip", sf.asymptoteSlip);
					sf.asymptoteValue = ImVector("Asymptote Value", sf.asymptoteValue);
					sf.stiffness = ImVector("Stiffness", sf.stiffness);

					coll.sidewaysFriction = sf;

					ImGui.TreePop();
				}
			}
		}

		void ShowCollider2D(Collider2D collider)
		{

		}

	}
}