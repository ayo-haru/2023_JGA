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
	[MenuItem("Launcher/Sample001",priority = 0)]
	public static void OpeneSampleScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/Sample001.unity", OpenSceneMode.Single);
	}

    [MenuItem("Launcher/InitializeScene", priority = 1)]
	public static void OpenInitializeScene()
    {
		EditorSceneManager.OpenScene("Assets/Project/Scenes/InitializeScene.unity", OpenSceneMode.Single);
    }

	[MenuItem("Launcher/ProtoType", priority = 2)]
	public static void OpeneProtoTypeScene()
	{
		EditorSceneManager.OpenScene("Assets/Project/Scenes/ProtoType/ProtoType.unity", OpenSceneMode.Single);
	}
}
