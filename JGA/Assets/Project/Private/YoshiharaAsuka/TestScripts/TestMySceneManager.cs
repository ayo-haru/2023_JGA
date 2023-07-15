//=============================================================================
// @File	: [TestMySceneManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
// 2023/06/09	すでに読み込まれたシーンの例外処理追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TestMySceneManager
{
	/* 使用するシーンの名前を登録 */
	[SerializeField]
	private static string[] SetSceneName =
	{
		"CommonScene",
		"TestAddScene1",
		"TestAddScene2",
		"TestUIScene",
		"TestGimickObjectScene",
		"TestGuestSpawonScene",
		"TestZooKeeperSpawonScene",
        "Title",
        "Stage_001",
        "Stage_002"
    };

	/* シーン名の列挙を定義 */
	public enum SCENE
	{ 
		SCENE_TESTMANAGAERSCENE,
		SCENE_TEST01,
		SCENE_TEST02,
		SCENE_TESTUI,
		SCENE_TESTGIMICK,
		SCENE_TESTGUESTSPAWON,
		SCENE_TESTZOOKEEPERSPAWON,
        SCENE_TITLE,
        SCENE_GAME_001,
        SCENE_GAME_002,
    };


/// <summary>
/// 指定したシーンの加算
/// </summary>
/// <param name="scene"></param>
public static AsyncOperation AddScene(SCENE scene)
	{
		if (CheckLoadScene(SetSceneName[(int)scene])){
			Debug.Log(SetSceneName[(int)scene] + "はすでに追加されたシーンです");
			return null;
		}

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SetSceneName[(int)scene],LoadSceneMode.Additive);

		return asyncLoad;
	}


    /// <summary>
    /// 指定したシーンの減算
    /// </summary>
    /// <param name="scene"></param>
    public static void SubtractScene(SCENE scene)
	{
		if (!CheckLoadScene(SetSceneName[(int)scene])){
			Debug.Log(SetSceneName[(int)scene] + "は読み込まれていないシーンです");
			return;
		}

		SceneManager.UnloadScene(SetSceneName[(int)scene]);
		// 不要なリソースを削除
		Resources.UnloadUnusedAssets();
	}

	/// <summary>
	/// 読み込まれているシーンかどうかを判定
	/// </summary>
	/// <param name="name"></param>
	/// <returns>読み込まれている = true, 読み込まれていない = false</returns>
	public static bool CheckLoadScene(string name)
	{
		return SceneManager.GetAllScenes().Any(Scene => Scene.name == name);
	}


}
