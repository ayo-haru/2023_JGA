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
using imnodesNET;
#endif
#if !UIMGUI_REMOVE_IMPLOT
using ImPlotNET;
using System.Linq;
using UImGui;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;
using System.Reflection;
using Unity.VisualScripting.FullSerializer;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
//using static GameSceneNetwork;
using System.Runtime.CompilerServices;
using System;
using System.ComponentModel;
#endif
#if !UIMGUI_REMOVE_IMGUIZMO
using ImGuizmoNET;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
//using static Photon.Pun.UtilityScripts.PunTeams;
using System.Numerics;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
using Quaternion = UnityEngine.Quaternion;
using Matrix4x4 = UnityEngine.Matrix4x4;
//using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;
using UnityEditor.PackageManager.UI;
using System.Xml.Linq;

namespace UImGui
{
	public class ImGuiTest : MonoBehaviour
	{
		[SerializeField]
		private bool bMode;
#if !UIMGUI_REMOVE_IMPLOT
		[SerializeField]
		private Vector4 _myColor;
		private bool _isOpen;

#endif
		private bool ena = true;
		private float f = 0.5f;
		private int n = 0;
		private bool b = true;

		private Camera mainCamera;
		private GameObject SelectObj;

		[SerializeField]
		private List<GameObject> gameObjects = new List<GameObject>();

		private OPERATION mCurrentGizmoOperation = OPERATION.TRANSLATE;
		private ImGuizmoNET.MODE mCurrentGizmoMode = ImGuizmoNET.MODE.LOCAL;
		private int gizmoCount = 1;
		private int lastUsing = 0;

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
					ImGui.MenuItem("(demo menu)", null, false, false);
					if (ImGui.MenuItem("New")) { }
					if (ImGui.MenuItem("Open", "Ctrl+O")) { }
					if (ImGui.BeginMenu("Open Recent"))
					{
						ImGui.MenuItem("fish_hat.c");
						ImGui.MenuItem("fish_hat.inl");
						ImGui.MenuItem("fish_hat.h");
						if (ImGui.BeginMenu("More.."))
						{
							ImGui.MenuItem("Hello");
							ImGui.MenuItem("Sailor");
							if (ImGui.BeginMenu("Recurse.."))
							{
								//ShowExampleMenuFile();
								ImGui.EndMenu();
							}
							ImGui.EndMenu();
						}
						ImGui.EndMenu();
					}
					if (ImGui.MenuItem("Save", "Ctrl+S")) { }
					if (ImGui.MenuItem("Save As..")) { }

					ImGui.Separator();
					if (ImGui.BeginMenu("Options"))
					{
						ImGui.MenuItem("Enabled", "", ena);
						ImGui.BeginChild("child", new Vector2(0, 60), true);
						for (int i = 0; i < 10; i++)
							ImGui.Text($"Scrolling Text {i}");
						ImGui.EndChild();
						ImGui.SliderFloat("Value", ref f, 0.0f, 1.0f);
						ImGui.InputFloat("Input", ref f, 0.1f);
						ImGui.Combo("Combo", ref n, "Yes\0No\0Maybe\0\0");
						ImGui.EndMenu();
					}

					if (ImGui.BeginMenu("Colors"))
					{
						float sz = ImGui.GetTextLineHeight();
						for (int i = 0; i < ((int)ImGuiCol.COUNT); i++)
						{
							string name = ImGui.GetStyleColorName((ImGuiCol)i);
							Vector2 p = ImGui.GetCursorScreenPos();
							ImGui.GetWindowDrawList().AddRectFilled(p, new Vector2(p.x + sz, p.y + sz), ImGui.GetColorU32((ImGuiCol)i));
							ImGui.Dummy(new Vector2(sz, sz));
							ImGui.SameLine();
							ImGui.MenuItem(name);
						}
						ImGui.EndMenu();
					}

					// ここでは、「オプション」メニュー(上記で作成済み) に再度追加する方法を示します。
					// もちろん、このデモでは、この関数が BeginMenu("Options") を 2 回呼び出すのは少しばかげています。
					// 実際のコードベースでは、非常に異なるコードの場所からこの機能を使用するのが理にかなっています。
					if (ImGui.BeginMenu("Options")) // <-- Append!
					{
						ImGui.Checkbox("SomeOption", ref b);
						ImGui.EndMenu();
					}

					if (ImGui.BeginMenu("Disabled", false)) // Disabled
					{

					}
					if (ImGui.MenuItem("Checked", null, true)) { }
					if (ImGui.MenuItem("Close ImGui", "Shift+F12")) { bMode = !bMode; }

					ImGui.EndMenu();
				}
				if (ImGui.BeginMenu("Edit"))
				{
					if (ImGui.MenuItem("Undo", "CTRL+Z")) { }
					if (ImGui.MenuItem("Redo", "CTRL+Y", false, false)) { }  // Disabled item
					ImGui.Separator();
					if (ImGui.MenuItem("Cut", "CTRL+X")) { }
					if (ImGui.MenuItem("Copy", "CTRL+C")) { }
					if (ImGui.MenuItem("Paste", "CTRL+V")) { }
					ImGui.EndMenu();
				}


				ImGui.EndMainMenuBar();
			}

			Debug.Log($"imgui");

#if !UIMGUI_REMOVE_IMNODES
			//if (ImGui.Begin("Nodes Window Sample"))
			//{
			//	ImGui.SetNextWindowSize(Vector2.one * 300, ImGuiCond.Once);
			//	imnodes.BeginNodeEditor();
			//	imnodes.BeginNode(1);

			//	imnodes.BeginNodeTitleBar();
			//	ImGui.TextUnformatted("simple node :)");
			//	imnodes.EndNodeTitleBar();

			//	imnodes.BeginInputAttribute(2);
			//	ImGui.Text("input");
			//	imnodes.EndInputAttribute();

			//	imnodes.BeginOutputAttribute(3);
			//	ImGui.Indent(40);
			//	ImGui.Text("output");
			//	imnodes.EndOutputAttribute();

			//	imnodes.EndNode();
			//	imnodes.EndNodeEditor();
			//	ImGui.End();
			//}
#endif

			ImGui.ShowDemoWindow();


			// ヒエラルキービュー
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

				//// カラー
				//ImGui.ColorEdit4("Color", ref _myColor);

				//// Plot some values
				//float[] my_values = new float[] { 0.2f, 0.1f, 1.0f, 0.5f, 0.9f, 2.2f };
				//ImGui.PlotLines("Hierarchy", ref my_values[0], my_values.Length);

				ImGui.BeginChild("GameObjects");
				{
					CreateHierarchy(gameObjects);
				}
				ImGui.EndChild();
			}
			ImGui.End();


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
					ImGui.Text($"{SelectObj.name}");

					//--- Tag
					// https://baba-s.hatenablog.com/entry/2014/02/25/000000
					int Tag = 0;
					string[] Tags = { $"{SelectObj.tag}" };
					ImGui.Combo("Tag", ref Tag, Tags, Tags.Length);

					ImGui.SameLine();   // 改行しない

					//--- Layer
					int layer = 0;
					string[] layers = { $"{SelectObj.layer.ToString()}" };
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
								Debug.LogWarning($"Type:{components[i]}");

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
									Debug.Log($"{member.Name}");

									//--- アクセス修飾子がPublicの場合
									if (member.IsPublic)
									{
										Debug.Log($"{member.Name}:public");
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
			Debug.Log($"SelectObj:{SelectObj}");
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

			//foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
			//{
			//	if (obj.activeInHierarchy /*&& obj.name != "AdaptivePerformanceManager"*/)
			//	{
			//		gameObjects.Add(obj);		// 最後
			//		//gameObjects.Insert(0, obj);	// 最初
			//	}
			//}

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
				if (ImGui.IsItemClicked())
					SelectObj = transform.gameObject;
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

#if TEST
		private void EditTransform(float cameraView, float cameraProjection, float matrix, bool editTransformDecomposition)
		{
			bool useSnap = false;
			Vector3 snap = Vector3.one;
			float[] bounds = { -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f };
			Vector3 boundsSnap = new Vector3(0.1f, 0.1f, 0.1f);
			bool boundSizing = false;
			bool boundSizingSnap = false;

			if (editTransformDecomposition)
			{
				if (Input.GetKeyDown(KeyCode.T))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.TRANSLATE;
				if (Input.GetKeyDown(KeyCode.E))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.ROTATE;
				if (Input.GetKeyDown(KeyCode.R))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.SCALE;

				if (ImGui.RadioButton("Translate", mCurrentGizmoOperation == ImGuizmoNET.OPERATION.TRANSLATE))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.TRANSLATE;
				ImGui.SameLine();
				if (ImGui.RadioButton("Rotate", mCurrentGizmoOperation == ImGuizmoNET.OPERATION.ROTATE))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.ROTATE;
				ImGui.SameLine();
				if (ImGui.RadioButton("Scale", mCurrentGizmoOperation == ImGuizmoNET.OPERATION.SCALE))
					mCurrentGizmoOperation = ImGuizmoNET.OPERATION.SCALE;
				//if (ImGui.RadioButton("Universal", mCurrentGizmoOperation == ImGuizmoNET.OPERATION.UNIVERSAL))
				//	mCurrentGizmoOperation = ImGuizmoNET.OPERATION.UNIVERSAL;

				//Vector3 matrixTranslation, matrixRotation, matrixScale;
				//ImGuizmo.DecomposeMatrixToComponents(ref matrix, ref matrixTranslation, ref matrixRotation, ref matrixScale);
				//ImGui.InputFloat3("Tr", ref matrixTranslation);
				//ImGui.InputFloat3("Rt", ref matrixRotation);
				//ImGui.InputFloat3("Sc", ref matrixScale);
				//ImGuizmo.RecomposeMatrixFromComponents(matrixTranslation, matrixRotation, matrixScale, matrix);

				if (mCurrentGizmoOperation != ImGuizmoNET.OPERATION.SCALE)
				{
					if (ImGui.RadioButton("Local", mCurrentGizmoMode == ImGuizmoNET.MODE.LOCAL))
						mCurrentGizmoMode = ImGuizmoNET.MODE.LOCAL;
					ImGui.SameLine();
					if (ImGui.RadioButton("World", mCurrentGizmoMode == ImGuizmoNET.MODE.WORLD))
						mCurrentGizmoMode = ImGuizmoNET.MODE.WORLD;
				}
				if (Input.GetKeyDown(KeyCode.S))
					useSnap = !useSnap;
				ImGui.Checkbox("##UseSnap", ref useSnap);
				ImGui.SameLine();

				switch (mCurrentGizmoOperation)
				{
					case ImGuizmoNET.OPERATION.TRANSLATE:
						ImGui.InputFloat3("Snap", ref snap);
						break;
					case ImGuizmoNET.OPERATION.ROTATE:
						ImGui.InputFloat("Angle Snap", ref snap.x);
						break;
					case ImGuizmoNET.OPERATION.SCALE:
						ImGui.InputFloat("Scale Snap", ref snap.x);
						break;
				}
				ImGui.Checkbox("Bound Sizing", ref boundSizing);
				if (boundSizing)
				{
					ImGui.PushID(3);
					ImGui.Checkbox("##BoundSizing", ref boundSizingSnap);
					ImGui.SameLine();
					ImGui.InputFloat3("Snap", ref boundsSnap);
					ImGui.PopID();
				}
			}

			ImGuiIOPtr ioPtr = ImGui.GetIO();
			float viewManipulateRight = ioPtr.DisplaySize.x;
			float viewManipulateTop = 0;
			ImGuizmo.SetRect(0, 0, ioPtr.DisplaySize.x, ioPtr.DisplaySize.y);

			float ide = Matrix4x4.identity.determinant;
			float objectMatrix = 0.0f;
			ImGuizmo.DrawGrid(ref cameraView, ref cameraProjection, ref ide, 100.0f);
			ImGuizmo.DrawCubes(ref cameraView, ref cameraProjection, ref objectMatrix, gizmoCount);
			//float? uSnap = useSnap ? snap.x : null;
			//float[] uBounds = boundSizing ? bounds : null;
			//Vector3? uBoundsSnap = boundSizingSnap ? boundsSnap : null;

			ImGuizmo.Manipulate(ref cameraView, ref cameraProjection, mCurrentGizmoOperation, mCurrentGizmoMode, ref matrix/*, null, ref uSnap, ref uBounds, ref uBoundsSnap*/);

			ImGuizmo.ViewManipulate(ref cameraView, mainCamera.farClipPlane, new Vector2(viewManipulateRight - 128, viewManipulateTop), new Vector2(128, 128), 0x10101010);

		}
#endif

	}
}