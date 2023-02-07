//=============================================================================
// @File    : [MySceneManager]
// @Brief   : オリジナルのシーンマネージャー
// @Author  : YOSHIHARA ASUKA
// @Editer  : 
// @Detail  : 参考URL:https://noracle.jp/unity-initialize-scene/
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
//=============================================================================
using UnityEngine;
using UnityEngine.SceneManagement;


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
