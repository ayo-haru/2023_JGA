//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/16	スポーン地点をPlayerRespwanに変更(吉原)
// 2023/03/20	飼育員自動生成(伊地田)
// 2023/03/21	飼育員自動生成バグとり(伊地田)
// 2023/03/30	ペンギンブースをリストに変更しました。【小楠】
// 2023/04/     客の自動生成
// 2023/04/24   クリア
// 2023/05/06   クリア画面を差し替え【小楠】
// 2023/05/12   ゲームオーバー画面を差し替え【小楠】
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class StageSceneManager : BaseSceneManager {
    //---各ブース
    [Header("それぞれのブースの場所を入れる(空でもOK)")]
    [NamedArrayAttribute(new string[] { "PENGUIN_N", "PENGUIN_S", "PENGUIN_W", "PENGUIN_E", "HORSE", "ELEPHANT", "LION", "POLARBEAR", "BIRD", "ENTRANCE" })]
    [SerializeField]
    private Transform[] rootPos;

    #region ゲームマネージャーに移動
    ////---時間UI
    //private GameObject timerUI;
    //private TimerSliderUI _TimerUI;

    ////---客人数UI
    //private GameObject guestNumUI;
    //private GuestNumUI _GuestNumUI;

    ////-----クリア画面
    //private GameObject clearPanel;
    //private ClearPanel _ClearPanel;
    //private ReactiveProperty<bool> isClear = new ReactiveProperty<bool>(false);

    ////-----ゲームオーバー画面
    //private GameObject gameOverPanel;
    //private GameOverPanel _GameOverPanel;
    //private ReactiveProperty<bool> isGameOver = new ReactiveProperty<bool>(false);

    ////---変数
    //private bool isOnce; // 一度だけ処理をするときに使う
    //AudioSource[] asList;
    #endregion

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();

        #region ゲームマネージャーに移動
#if UNITY_EDITOR
        if (GameData.nowScene == 0) { // 本来ならnowSceneneには現在のシーン番号が入るがエディタ上で実行した場合は0が入っているので初期化
            for (int i = 0; i < System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length; i++) {
                if (SceneManager.GetActiveScene().name == MySceneManager.sceneName[i]) {
                    GameData.nowScene = i;
                }
            }
            GameData.oldScene = GameData.nowScene;
        }
#endif
        if (GameData.isContinueGame && GameData.oldScene == (int)MySceneManager.SceneState.SCENE_TITLE) {
            /*
             * セーブデータが存在または初期化済みでタイトルシーンを通ってきた場合つづきから
             */
        } else {
            //BeginGame();
        }

        //isOnce = false;
        //// BGM再生用にオーディオソース取得
        //asList = GetComponents<AudioSource>();
        #endregion
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        #region ゲームマネージャーに移動
        //----- イベント登録 -----
        //// クリア
        //isClear.Subscribe(_ => { if (isClear.Value) OnClear(); }).AddTo(this);
        //// ゲームオーバー
        //isGameOver.Subscribe(_ => { if (isGameOver.Value) OnGameOver(); }).AddTo(this);
        #endregion
        //----- それぞれのブースの座標の取得 -----
        rootPos = new Transform[Enum.GetNames(typeof(GameData.eRoot)).Length];
        // 客のルート取得===================================================================
        #region 客ルート
        if (!rootPos[(int)GameData.eRoot.PENGUIN_N]) {
            rootPos[(int)GameData.eRoot.PENGUIN_N] = GameObject.Find("PenguinCagePos_N").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_S]) {
            rootPos[(int)GameData.eRoot.PENGUIN_S] = GameObject.Find("PenguinCagePos_S").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_W]) {
            rootPos[(int)GameData.eRoot.PENGUIN_W] = GameObject.Find("PenguinCagePos_W").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_E]) {
            rootPos[(int)GameData.eRoot.PENGUIN_E] = GameObject.Find("PenguinCagePos_E").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.RESTSPOT_01]) {
            rootPos[(int)GameData.eRoot.RESTSPOT_01] = GameObject.Find("RestSpotPos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.RESTSPOT_02]) {
            rootPos[(int)GameData.eRoot.RESTSPOT_02] = GameObject.Find("RestSpotPos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_01]) {
            rootPos[(int)GameData.eRoot.HORSE_01] = GameObject.Find("HorseCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_02]) {
            rootPos[(int)GameData.eRoot.HORSE_02] = GameObject.Find("HorseCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_03]) {
            rootPos[(int)GameData.eRoot.HORSE_03] = GameObject.Find("HorseCagePos03").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_01]) {
            rootPos[(int)GameData.eRoot.ZEBRA_01] = GameObject.Find("ZebraCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_02]) {
            rootPos[(int)GameData.eRoot.ZEBRA_02] = GameObject.Find("ZebraCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_03]) {
            rootPos[(int)GameData.eRoot.ZEBRA_03] = GameObject.Find("ZebraCagePos03").GetComponent<Transform>();
        }

        if (!rootPos[(int)GameData.eRoot.POLARBEAR]) {
            rootPos[(int)GameData.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_01]) {
            rootPos[(int)GameData.eRoot.BEAR_01] = GameObject.Find("BearCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_02]) {
            rootPos[(int)GameData.eRoot.BEAR_02] = GameObject.Find("BearCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA]) {
            rootPos[(int)GameData.eRoot.PANDA] = GameObject.Find("PandaCagePos").GetComponent<Transform>();
        }

        if (!rootPos[(int)GameData.eRoot.ENTRANCE]) {
            rootPos[(int)GameData.eRoot.ENTRANCE] = GameObject.Find("EntrancePos").GetComponent<Transform>();
        }
        #endregion
        //===============================================================================

        // 飼育員巡回ルート取得==============================================================
        #region 飼育員巡回ルール
        if (!rootPos[(int)GameData.eRoot.BELL_AREA]) {
            rootPos[(int)GameData.eRoot.BELL_AREA] = GameObject.Find("BellArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.POLAR_AREA]) {
            rootPos[(int)GameData.eRoot.POLAR_AREA] = GameObject.Find("PolarArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_AREA]) {
            rootPos[(int)GameData.eRoot.BEAR_AREA] = GameObject.Find("BearArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA_AREA_01]) {
            rootPos[(int)GameData.eRoot.PANDA_AREA_01] = GameObject.Find("PandaArea01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA_AREA_02]) {
            rootPos[(int)GameData.eRoot.PANDA_AREA_02] = GameObject.Find("PandaArea02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HOURSE_AREA]) {
            rootPos[(int)GameData.eRoot.HOURSE_AREA] = GameObject.Find("HourseArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_AREA_01]) {
            rootPos[(int)GameData.eRoot.ZEBRA_AREA_01] = GameObject.Find("ZebraArea01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_AREA_02]) {
            rootPos[(int)GameData.eRoot.ZEBRA_AREA_02] = GameObject.Find("ZebraArea02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.FOUNTAIN_AREA]) {
            rootPos[(int)GameData.eRoot.FOUNTAIN_AREA] = GameObject.Find("FountainArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.LAKE_AREA]) {
            rootPos[(int)GameData.eRoot.LAKE_AREA] = GameObject.Find("LakeArea").GetComponent<Transform>();
        }
        #endregion
        //===============================================================================

        #region ゲームマネージャーに移動
        //----- タイマーUIの取得 -----
        //timerUI = UIManager.UIManagerInstance.TimerUIObj;
        //if (timerUI) {
        //    _TimerUI = timerUI.GetComponent<TimerSliderUI>();

        //    _TimerUI.CountStart();
        //} else {
        //    Debug.LogWarning("TimerUIがシーン上にありません");
        //}

        //----- 客人数カウントUIの取得 -----
        //guestNumUI = UIManager.UIManagerInstance.TimerUIObj;
        //if (guestNumUI) {
        //    _GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
        //} else {
        //    Debug.LogWarning("GuestNumUIがシーン上にありません");
        //}

        //----- 音の処理 -----
        //// BGM再生
        //SoundManager.Play(asList[0], SoundManager.EBGM.GAME001);
        //SoundManager.Play(asList[1], SoundManager.EBGM.INZOO);
        //// フェード中だったら待機して音を止める
        //StartCoroutine(WaitFade());

        //----- セーブ -----
        //SaveManager.SaveAll();  // 一旦全セーブ
        #endregion
    }

    void Update() {
        #region ゲームマネージャーに移動
        //----- プレイヤーの座標保存 -----
        //GameData.playerPos = playerInstance.transform.position;
        //SaveSystem.SaveLastPlayerPos(GameData.playerPos);

        ////----- 時間系の処理 -----
        //if (timerUI) {
        //    isGameOver.Value = _TimerUI.IsFinish();
        //}

        ////----- ゲームクリア -----
        //if (guestNumUI) {
        //    isClear.Value = _GuestNumUI.isClear();
        //}

        //if (isClear.Value) {
        //    if (!isOnce) {   // 一度だけ処理
        //        //　クリア画面取得
        //        if (!clearPanel) clearPanel = GameObject.Find("ClearPanel");
        //        if (clearPanel && !_ClearPanel) _ClearPanel = clearPanel.GetComponent<ClearPanel>();

        //        int next = -1;
        //        if (_ClearPanel) next = _ClearPanel.GetNextScene();
        //        if (next != -1) {
        //            GameData.oldScene = GameData.nowScene;  // 今のシーンをひとつ前のシーンとして保存
        //            GameData.nowScene = next;
        //            SceneChange(next);  // シーン遷移
        //            isOnce = true;

        //            SaveManager.SaveAll();
        //        }
        //    }
        //}

        ////----- 制限時間のゲームオーバー -----
        //if (isGameOver.Value) {
        //    if (!isOnce) {
        //        // ゲームオーバー画面取得
        //        if (!gameOverPanel) gameOverPanel = GameObject.Find("GameOverPanel");
        //        if (gameOverPanel && !_GameOverPanel) _GameOverPanel = gameOverPanel.GetComponent<GameOverPanel>();

        //        int next = -1;
        //        if (_GameOverPanel) next = _GameOverPanel.GetNextScene();
        //        if (next != -1) {
        //            GameData.oldScene = GameData.nowScene;  // 今のシーンをひとつ前のシーンとして保存
        //            GameData.nowScene = next;
        //            SceneChange(next);  // シーン遷移
        //            isOnce = true;
        //        }
        //    }
        //}
        #endregion
    }

    private void LateUpdate() {
        #region ゲームマネージャーに移動
        ////----- 飼育員に捕まったフラグを下す -----
        ///*
        // * Updateで下ろすと各処理と同フレーム中にフラグが降りてしまうためLateに書いた
        // */
        //if (GameData.isCatchPenguin) {
        //    GameData.isCatchPenguin = false;
        //}
        #endregion
    }

    /// <summary>
    /// StageManagerで取得した各ブースの座標を返す
    /// </summary>
    /// <param name="_root"></param>
    /// <returns></returns>
    public Transform GetRootTransform(GameData.eRoot _root) {
        return rootPos[(int)_root];
    }

    #region ゲームマネージャーに移動
    ///// <summary>
    ///// ステージを初めの状態からやる
    ///// </summary>
    //private void BeginGame() {
    //    GameData.guestCnt = 0;
    //    GameData.timer = 0.0f;
    //    //playerRespawn = GameObject.Find("PlayerSpawn");
    //    //GameData.playerPos = playerRespawn.transform.position;

    //    // 一個前のシーンがタイトルかつ今のシーンがステージ１
    //    if (GameData.oldScene == (int)MySceneManager.SceneState.SCENE_TITLE &&
    //        GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
    //        TutorialManager.StartTutorial();
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //IEnumerator WaitFade() {
    //    asList[0].Pause();
    //    asList[1].Pause();
    //    yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.Default);
    //    asList[0].UnPause();
    //    asList[1].UnPause();
    //}

    ///// <summary>
    ///// クリアになった瞬間にやる処理
    ///// </summary>
    //private void OnClear() {
    //    // ポーズ
    //    if (!PauseManager.isPaused) {
    //        PauseManager.isPaused = true;
    //        PauseManager.NoMenu = true;
    //        PauseManager.Pause();
    //    }

    //    //----- 音の再生 -----
    //    // ポーズ後にやらないとポーズに消される
    //    SoundManager.Play(asList[2], SoundManager.ESE.GAMECLEAR);

    //    // セーブ
    //    SaveManager.SaveGuestCnt(0);
    //    SaveManager.SaveTimer(0.0f);
    //    if (System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length > GameData.nowScene) {  // 最大シーンではないとき
    //        SaveManager.SaveLastStageNum(GameData.nowScene + 1);
    //    } else {
    //        SaveManager.SaveLastStageNum(GameData.nowScene);
    //    }
    //    SaveManager.SaveLastPlayerPos(GameData.playerPos);
    //}

    ///// <summary>
    ///// ゲームオーバーになったらやる処理
    ///// </summary>
    //private void OnGameOver() {
    //    // ポーズ
    //    if (!PauseManager.isPaused) {
    //        PauseManager.isPaused = true;
    //        PauseManager.NoMenu = true;
    //        PauseManager.Pause();
    //    }

    //    //----- 音の再生 -----
    //    // ポーズ後にやらないとポーズに消される
    //    SoundManager.Play(asList[2], SoundManager.ESE.GAMEOVER);

    //    //----- セーブ -----
    //    // 初期値の値で保存。シーン番号は現在のシーン。
    //    SaveManager.SaveInitDataAll();
    //}
    #endregion
}
