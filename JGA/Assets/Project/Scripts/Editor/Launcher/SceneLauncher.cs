//=============================================================================
// @File	: [SceneLauncher]
// @Brief	: シーン切り替えを行えるランチャーの作成
// @Author	: YOSHIHARA ASUKA
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/07 スクリプト作成
//=============================================================================
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneLauncher
{

	[MenuItem("Launcher/Title", priority = 1)]
	public static void OpenTitleScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/Title/Title.unity", OpenSceneMode.Single);
	}


	[MenuItem("Launcher/InitializeScene", priority = 2)]
	public static void OpenInitializeScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/InitializeScene.unity", OpenSceneMode.Single);
	}

	[MenuItem("Launcher/ProtoType", priority = 3)]
	public static void OpeneProtoTypeScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/ProtoType/ProtoType.unity", OpenSceneMode.Single);
	}

	[MenuItem("Launcher/Stage001", priority = 4)]
	public static void OpeneStage001Scene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/Game/Stage_001.unity", OpenSceneMode.Single);
	}

	[MenuItem("Launcher/Stage002", priority = 5)]
	public static void OpeneStage002Scene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/Game/Stage_002.unity", OpenSceneMode.Single);
	}

	[MenuItem("Launcher/CommonScene", priority = 6)]
	public static void OpenCommonScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Private/YoshiharaAsuka/AddScene/CommonScene.unity", OpenSceneMode.Single);
	}

	[MenuItem("Launcher/GimickObjectScene", priority = 6)]
	public static void OpenGimickObjectScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Private/YoshiharaAsuka/AddScene/GimickObjectScene.unity", OpenSceneMode.Single);
	}



}
