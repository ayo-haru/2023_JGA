//=============================================================================
// @File	: [TestLoadScene.cs]
// @Brief	: シーンを複数ロードできるか検証
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestLoadScene
{
#if false

    private const string ActiveSceneName = "ProtoType";
    // ロードするシーンを検索
    private const string LoadSceneName = "TestGround";

    /// <summary>
    /// シーンを呼び出す。
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadGroundScene()
    {
        // 現在のアクティブシーン名を取得
        Scene activeSceneName = SceneManager.GetActiveScene();

        // 現在開いているシーンが"ProtoType"シーンの場合、ステージシーン(Dummy)を読み込む。
        if(activeSceneName.name == ActiveSceneName)
		{
            SceneManager.LoadScene(LoadSceneName, LoadSceneMode.Additive);
		}
		else{
			Debug.LogWarning(ActiveSceneName + "シーンではないので、ステージシーンを読み込みませんでした。");
		}
    }
    #endif


}
