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
    // ロードするシーンを検索
    private const string LoadSceneName = "TestGround";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadGroundScene()
    {
        if (!SceneManager.GetSceneByName(LoadSceneName).IsValid()){
            SceneManager.LoadScene(LoadSceneName, LoadSceneMode.Additive);

        }
        else{
            Debug.LogError(LoadSceneName+"が見つかりませんでした。");
        }
    }


}
