//=============================================================================
// @File	: [TestMySceneManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TestMySceneManager
{
    [SerializeField]
    private static string[] SetSceneName =
    {
        "TestAddScene1",
        "TestAddScene2",
        //"TestManagerScene",
    };


    public enum SCENE
    { 
        SCENE_TEST01,
        SCENE_TEST02,
        SCENE_TEST03,
    }

    /// <summary>
    /// 指定したシーンの加算
    /// </summary>
    /// <param name="scene"></param>
    public static void AddScene(SCENE scene)
    {
        for(int i = 0; i < SetSceneName.Length; ++i){
            if(SceneManager.GetActiveScene().name == SetSceneName[i]){
                Debug.LogWarning(SetSceneName[i] + "はaすでに追加されたシーンです");
                return;
            }
        }
        // 同じシーンの追加を回避
        SceneManager.LoadScene(SetSceneName[(int)scene],LoadSceneMode.Additive);
    }

    /// <summary>
    /// 指定したシーンの減算
    /// </summary>
    /// <param name="scene"></param>
    public static void SubtractScene(SCENE scene)
    {
        SceneManager.UnloadScene(SetSceneName[(int)scene]);
    }


}
