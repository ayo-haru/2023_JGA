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
#if !UIMGUI_REMOVE_IMPLOT
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Reflection;
using System;
#endif
#if !UIMGUI_REMOVE_IMGUIZMO
using ImGuizmoNET;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;
using Quaternion = UnityEngine.Quaternion;
using Matrix4x4 = UnityEngine.Matrix4x4;

namespace UImGui
{
	public class ImGuiView : MonoBehaviour
	{
		[SerializeField]
		private bool bMode;

		[SerializeField]
		private UImGui uImGui;

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


		[SerializeField]
		private bool bGridDraw;





		private Camera mainCamera;
		private GameObject SelectObj;
		private string SelectName;
		private Rigidbody SelectRb;
		private Collider SelectColl;

		[SerializeField]
		private List<GameObject> gameObjects = new List<GameObject>();

		private OPERATION mCurrentGizmoOperation = OPERATION.TRANSLATE;
		private ImGuizmoNET.MODE mCurrentGizmoMode = ImGuizmoNET.MODE.LOCAL;

		/*unsafe*/
		public void AddIconFont(ImGuiIOPtr io)
		{
			//// @Application.dataPath => "... JGA\Assets"
			//string path = $"{Application.dataPath}\\Project\\Private\\SakaiRyotaro\\{FontAwesome6.FontIconFileNameFAR}";
			//int[] icons = { FontAwesome6.IconMin, FontAwesome6.IconMax, 0 };
			//ImFontConfig fontConfig = default;
			//ImFontConfigPtr fontConfigPtr = new ImFontConfigPtr(&fontConfig);
			//fontConfigPtr.OversampleH = fontConfigPtr.OversampleV = 1;
			//fontConfigPtr.MergeMode = true;
			//fontConfigPtr.FontBuilderFlags |= (uint)ImGuiFreeTypeBuilderFlags.LoadColor;
			//io.Fonts.AddFontDefault(fontConfigPtr);
			//ImFontPtr font;
			//fixed (void* iconsPtr = icons)
			//	font = io.Fonts.AddFontFromFileTTF(path, 16.0f, fontConfigPtr, (IntPtr)iconsPtr);
			//io.Fonts.Build();
		}

		/// <summary>
		/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
		/// </summary>
		void Awake()
		{
			if (mainCamera == null)
				mainCamera = Camera.main;

			uImGui.SetCamera(mainCamera);

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
				if (ImGui.BeginMenu("Option"))
				{
					if (ImGui.BeginMenu("Window"))
					{
						if (ImGui.MenuItem("Demo", null, bShowDemoWindow)) { bShowDemoWindow = !bShowDemoWindow; }
						if (ImGui.MenuItem("Hierarchy", null, bShowHierarchy)) { bShowHierarchy = !bShowHierarchy; }
						if (ImGui.MenuItem("Inspector", null, bShowInspector)) { bShowInspector = !bShowInspector; }
						ImGui.EndMenu();
					}
					if (ImGui.MenuItem("Draw Grid", null, bGridDraw)) { bGridDraw = !bGridDraw; }


					ImGui.EndMenu();
				}
				if (ImGui.BeginMenu(IconFonts.FontAwesome6.Fish))
				{
					ImGui.EndMenu();
				}

				ImGui.EndMainMenuBar();
			}

			if (bShowDemoWindow)
				ImGui.ShowDemoWindow();


			// ヒエラルキービュー
			//if (bShowHierarchy)
			{
				ImGui.Begin("Hierarchy", ref bShowHierarchy, ImGuiWindowFlags.MenuBar);
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

			//if (bShowInspector)
			{
				// インスペクタビュー
				ImGui.Begin("Inspector", ref bShowInspector, ImGuiWindowFlags.MenuBar);
				{
					if (SelectObj != null)
					{
						//--- アクティブ チェックボックス
						if (SelectObj.name.Contains("ImGui"))
						{
							bool active = true;
							ImGui.Checkbox("", ref active);
						}
						else
						{
							bool active = SelectObj.activeSelf;
							if (ImGui.Checkbox("", ref active))
								SelectObj.SetActive(!SelectObj.activeSelf);
						}

						ImGui.SameLine();   // 改行しない

						//--- Name
						if (ImGui.InputText($"##ObjectName", ref SelectName, uint.MaxValue, ImGuiInputTextFlags.AlwaysOverwrite))
							SelectObj.name = SelectName;

						//--- Tag
						ImGui.PushItemWidth(-ImGui.GetContentRegionAvail().x * 0.5f);
						InspectorTag();
						ImGui.PopItemWidth();

						ImGui.SameLine();   // 改行しない


						//--- Layer
						ImGui.PushItemWidth(-ImGui.GetContentRegionAvail().x * 0.5f);
						InspectorLayer();
						ImGui.PopItemWidth();

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
									FieldInfo[] members = t.GetFields(
										BindingFlags.Public |       // パブリックメンバを検索の対象に加える。
										BindingFlags.NonPublic |    // パブリックでないメンバを検索の対象に加える。
										BindingFlags.Instance |     // 非静的メンバ（インスタンスメンバ）を検索の対象に加える。
										BindingFlags.Static |       // 静的メンバを検索の対象に加える。
										BindingFlags.DeclaredOnly   // 継承されたメンバを検索の対象にしない。
										);

									foreach (FieldInfo member in members)
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

#if UNITY_EDITOR
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
#endif

		private void InspectorTag()
		{
#if UNITY_EDITOR
			int Tag = 0;

			string[] Tags = InternalEditorUtility.tags;
			for (int i = 0; i < Tags.Length; i++)
			{
				if (Tags[i].Contains(SelectObj.tag))
					Tag = i;
			}
			if (ImGui.Combo("Tag", ref Tag, Tags, Tags.Length))
				SelectObj.tag = Tags[Tag];
#else
			string tag = SelectObj.tag;
			if (ImGui.InputText("Tag", ref tag, uint.MaxValue, ImGuiInputTextFlags.AlwaysOverwrite))
				SelectObj.tag = tag;
#endif
		}

		private void InspectorLayer()
		{
#if UNITY_EDITOR
			int layer = 0;

			string[] layers = InternalEditorUtility.layers;
			for (int i = 0; i < layers.Length; i++)
			{
				if (layers[i].Contains(SelectObj.layer.ToString()))
					layer = i;
			}
			if (ImGui.Combo("Layer", ref layer, layers, layers.Length))
				SelectObj.layer = layer;
#else
			string layer = Convert.ToString(SelectObj.layer);
			if (ImGui.InputText("Tag", ref layer, uint.MaxValue, ImGuiInputTextFlags.AlwaysOverwrite | ImGuiInputTextFlags.CharsDecimal))
				SelectObj.layer = int.Parse(layer);
#endif
		}

		// シーン上のゲームオブジェクト一覧取得
		private void GetGameObject()
		{
			gameObjects.Clear();
			foreach (GameObject obj in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(GameObject)))
			{
				if (obj.scene.name == null || obj.scene.name.Contains($"CustomLightsScene-SceneView"))
					continue;
				gameObjects.Add(obj);       // 最後
			}
		}

		// ヒエラルキーリスト作成
		private void CreateHierarchy(List<GameObject> Objs)
		{
			List<GameObject> Hierarchy = new List<GameObject>();
			Scene current = SceneManager.GetActiveScene();

			for (int i = 0; i < Objs.Count; i++)
			{
				if (Objs[i].transform.parent == null)
				{
					Hierarchy.Add(Objs[i]);
				}
			}

			while (Hierarchy.Count > 0)
			{
				bool open = ImGui.CollapsingHeader($"{current.name}", ImGuiTreeNodeFlags.DefaultOpen);
				{
					for (int i = 0; i < Hierarchy.Count; i++)
					{
						if (Hierarchy[i].scene != current)
							continue;
						if (Hierarchy[i].name == "AdaptivePerformanceManager")
							continue;

						if (open)
							GetChild(Hierarchy[i].transform);

						Hierarchy[i] = null;
					}
					Hierarchy.RemoveAll(x => x == null);
				}

				if (Hierarchy.Count > 0)
				{
					current = Hierarchy[0].scene;
				}
			}
		}

		// 子オブジェクト取得
		private void GetChild(Transform transform)
		{
			ImGuiTreeNodeFlags node_flags =
				ImGuiTreeNodeFlags.SpanAvailWidth;

			// 子オブジェクトを持っているか
			if (transform.childCount > 0)
				node_flags |= ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;
			else
				node_flags |= ImGuiTreeNodeFlags.Leaf;

			// オブジェクトが非アクティブか
			if (!transform.gameObject.activeSelf)
				ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

			if (ImGui.TreeNodeEx($"{transform.name}##{transform.GetInstanceID()}", node_flags))     // ##transform.GetInstanceID()でオブジェクトの名前が被っても大丈夫になる
			{
				// クリックされた時
				if (ImGui.IsItemActive() && SelectObj != transform.gameObject)
				{
					Debug.Log($"ItemClicked:{transform.name}");
					SelectObj = transform.gameObject;
					SelectName = transform.name;
					if (SelectObj.TryGetComponent(out Rigidbody rigidbody))
						SelectRb = rigidbody;
					if (SelectObj.TryGetComponent(out Collider collider))
						SelectColl = collider;
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
			else
			{
				// クリックされた時
				if (ImGui.IsItemActive() && SelectObj != transform.gameObject)
				{
					Debug.Log($"ItemClicked:{transform.name}");
					SelectObj = transform.gameObject;
					SelectName = transform.name;
					if (SelectObj.TryGetComponent(out Rigidbody rigidbody))
						SelectRb = rigidbody;
					if (SelectObj.TryGetComponent(out Collider collider))
						SelectColl = collider;
				}
			}
			if (!transform.gameObject.activeSelf)
				ImGui.PopStyleColor(1);
		}

		//private unsafe ImGuiIO* io = ImGuiNative.igGetIO();
		private /*unsafe*/ void ImGuizmoDemo(GameObject gameObject)
		{
			// Gizmo設定ウィンドウ
			{
				ImGuiWindowFlags window_flags =
					ImGuiWindowFlags.NoDecoration |
					ImGuiWindowFlags.NoDocking |
					ImGuiWindowFlags.AlwaysAutoResize |
					ImGuiWindowFlags.NoSavedSettings |
					ImGuiWindowFlags.NoFocusOnAppearing |
					ImGuiWindowFlags.NoNav;
				ImGui.SetNextWindowBgAlpha(0.35f); // Transparent background
				bool b = true;
				if (ImGui.Begin("GizmoTools", ref b, window_flags))
				{
					ImGui.Text($"GizmoTools");

					if (ImGui.Button("T"))
						mCurrentGizmoOperation = ImGuizmoNET.OPERATION.TRANSLATE;
					ImGui.SameLine();

					if (ImGui.Button("R"))
						mCurrentGizmoOperation = ImGuizmoNET.OPERATION.ROTATE;
					ImGui.SameLine();

					if (ImGui.Button("S"))
						mCurrentGizmoOperation = ImGuizmoNET.OPERATION.SCALE;


					if (ImGui.Button("LOCAL"))
						mCurrentGizmoMode = MODE.LOCAL;
					ImGui.SameLine();

					if (ImGui.Button("WORLD"))
						mCurrentGizmoMode = MODE.WORLD;


					//ImGui.Text($"Simple overlay\nin the corner of the screen.\n(right-click to change position)");
					//if (ImGui.BeginPopupContextWindow())
					//{
					//	if (ImGui.MenuItem("Custom", null, corner == -1)) corner = -1;
					//	if (ImGui.MenuItem("Top-left", null, corner == 0)) corner = 0;
					//	if (ImGui.MenuItem("Top-right", null, corner == 1)) corner = 1;
					//	if (ImGui.MenuItem("Bottom-left", null, corner == 2)) corner = 2;
					//	if (ImGui.MenuItem("Bottom-right", null, corner == 3)) corner = 3;
					//	if (b && ImGui.MenuItem("Close")) b = false;
					//	ImGui.EndPopup();
					//}
				}
				ImGui.End();
			}

			ImGuizmo.SetRect(0, 0, Screen.width, Screen.height);

			ImGuizmoNET.ImGuizmo.IsUsing();

			//--- Gizmoの大きさを設定
			ImGuizmo.SetGizmoSizeClipSpace(0.15f);

			Matrix4x4 matrix = gameObject.transform.localToWorldMatrix;
			Matrix4x4 view = mainCamera.worldToCameraMatrix;
			Matrix4x4 projection = mainCamera.projectionMatrix;
			Matrix4x4 origin = Matrix4x4.identity;

			//Vector3 matrixTranslation, matrixRotation, matrixScale;
			//ImGuizmo.DecomposeMatrixToComponents(ref matrix.m00, ref matrixTranslation, ref matrixRotation, ref matrixScale);
			//ImGui.InputFloat3("Tr", ref matrixTranslation);
			//ImGui.InputFloat3("Rt", ref matrixRotation);
			//ImGui.InputFloat3("Sc", ref matrixScale);
			//ImGuizmo.RecomposeMatrixFromComponents(matrixTranslation, matrixRotation, matrixScale, matrix);

			//ImGuizmoNET.ImGuizmo.AllowAxisFlip(SelectObj != null);
			//ImGuizmoNET.ImGuizmo.DrawGrid(ref view.m00, ref projection.m00, ref matrix.m00, 50f);
			//ImGuizmoNET.ImGuizmo.DrawCubes(ref view.m00, ref projection.m00, ref matrix.m00, 1); //(Debug)

			//--- Gizmo本体
			ImGuizmo.Manipulate(ref view.m00, ref projection.m00, mCurrentGizmoOperation, mCurrentGizmoMode, ref matrix.m00);

			//--- グリッド描画
			if (bGridDraw)
				ImGuizmo.DrawGrid(ref view.m00, ref projection.m00, ref origin.m00, 50f);

			//--- ドラッグ中
			if (ImGuizmo.IsUsing())
			{
				//if (SelectRb != null)
				//	SelectRb.
				//if (SelectColl != null)
				//	SelectColl = collider;
			}

			//--- 右上のカメラの角度ビュー
			ImGuiIOPtr ioPtr = ImGui.GetIO();
			float viewManipulateRight = ioPtr.DisplaySize.x;
			float viewManipulateTop = 0;
			ImGuizmo.ViewManipulate(ref view.m00, 2, new Vector2(viewManipulateRight - 128, viewManipulateTop), new Vector2(128, 128), 0x10101010);

			//gameObject.transform.localToWorldMatrix = matrix;

			gameObject.transform.position = new Vector3(matrix.m03, matrix.m13, matrix.m23);
		}

		// コンポーネント別(UnityEngine)処理
		private void ShowUnityComponents(UnityEngine.Component compo)
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
		private void ShowComponents(FieldInfo info, object type, bool IsSerializable)
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
			}

		}



		private static void ImCombo<T>(T @enum) where T : IComparable
		{
			//if (typeof(T) == typeof(Enum))
			{
				int i = Convert.ToInt32(@enum);
				if (ImGui.Combo($"{@enum.GetType().Name}", ref i, Enum.GetNames(@enum.GetType()), Enum.GetValues(@enum.GetType()).Length))
					@enum = i.ConvertTo<T>();
			}
		}

		private static T ImVector<T>(string lavel, T vec, float? speed = 0.01f)
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

		private static T ImCheckBox<T>(string lavel, T @boolean)
		{
			if (typeof(T) == typeof(bool))
			{
				bool b = @boolean.ConvertTo<bool>();
				if (ImGui.Checkbox(lavel, ref b))
					return b.ConvertTo<T>();
			}

			return @boolean;
		}



		private void ShowTransform(Transform t)
		{
			// pos
			t.localPosition = ImVector("Position", t.localPosition);

			// rot
			t.localEulerAngles = ImVector("Rotation", t.localEulerAngles);

			// scale
			t.localScale = ImVector("Scale", t.localScale);
		}

		private void ShowCamera(Camera cam)
		{
			cam.enabled = ImCheckBox($"##Component{cam.GetType().Name}", cam.enabled);

			int i;
			i = (int)cam.clearFlags;
			if (ImGui.Combo("Clear Flags(alpha)", ref i, Enum.GetNames(typeof(CameraClearFlags)), Enum.GetValues(typeof(CameraClearFlags)).Length))
				cam.clearFlags = (CameraClearFlags)i;
		}

		private void ShowMeshFilter(MeshFilter f)
		{
			ImGui.Text($"mesh:{f.mesh}");
		}

		private void ShowRigidBody(Rigidbody rb)
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

		private void ShowCollider(UnityEngine.Collider collider)
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

		private void ShowCollider2D(Collider2D collider)
		{

		}

	}
}