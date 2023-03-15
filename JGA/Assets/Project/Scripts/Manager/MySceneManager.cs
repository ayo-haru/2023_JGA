//=============================================================================
// @File	: [MySceneManager]
// @Brief	: オリジナルのシーンマネージャー
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail  : 
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : SingletonMonoBehaviour<MySceneManager> {
    // ゲーム内で使うデータを持つクラス
    public static class GameData {
        public static PrefabContainer characterDatas;
        public static PrefabContainer UIDatas;
        public static bool isCatchPenguin;
    }

    // このクラスを持つオブジェクトは消えない
    protected override bool dontDestroyOnLoad { get { return true; } }

    //----- シーン遷移で使用 -----
    public enum SceneState {    // シーン定数
        SCENE_TITLE  =0,
        SCENE_GAME
    };

    private static string[] sceneName = {   // シーン名(実際に作ったシーンの名前入れてね)
        "Title",
        "ProtoType"
    };





    private void Awake() {
        Application.targetFrameRate = 60;       // FPSを60に固定
        GameData.characterDatas = AddressableLoader<PrefabContainer>.Load("CharacterData");
        GameData.UIDatas = AddressableLoader<PrefabContainer>.Load("UIData");

        GameData.isCatchPenguin = false;
    }

    private void Start() {

    }

    /// <summary>
    /// 呼ばれたらシーン遷移する
    /// </summary>
    /// <param name="_nextScene">遷移先のシーンの定数</param>
    public static void SceneChange(SceneState _nextScene) {
        SceneManager.LoadScene(sceneName[(int)_nextScene]);
    }


    /// <summary>
    /// 呼ばれたらシーン読み込み直す
    /// </summary>
    public static void SceneReload() {
        /*
         * 一応作ってみたんだけどInitializeSceneは再読み込みされないのでちょっとどうしようかなって感じ
         * 本格的に使うならつくろかなーなんて
         */


        // 現在のSceneを取得
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene(loadScene.name);
    }
}
