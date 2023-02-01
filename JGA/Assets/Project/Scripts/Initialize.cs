using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 共通の初期化をするシーンの呼び出しを行う
/// URL:https://noracle.jp/unity-initialize-scene/
/// </summary>
public class Initialize
{
    // 初期化を行うシーンの名前を検索
    private const string InitializeSceneName = "InitializeScene";

    // 属性のリファレンス:https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
    // [RuntimeInitializeOnLoadMethod]ゲームがロードされた後に呼び出される。
    // ※現在のシーンのAwake()の後に初期用シーンが生成されて初期化シーンのAwake()が走る。
    // 　詳しくはsummryのURLへ
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RuntimeInitializeApplication()
    {

        if (!SceneManager.GetSceneByName(InitializeSceneName).IsValid()){
            SceneManager.LoadScene(InitializeSceneName,LoadSceneMode.Additive);
        }
        else{
            Debug.LogError("初期化用シーンの呼び出しが出来ませんでした。");
        }

    }

}
