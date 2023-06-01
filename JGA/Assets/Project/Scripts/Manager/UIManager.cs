//=============================================================================
// @File	: [UIManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //---キャンバス
	GameObject canvasObj;
    Canvas canvas;

    //---カメラ
    Camera resultCamera;
    ResultCamera _ResultCamera;

    //---フェード
    GameObject fade;

    //---クリア
    GameObject clearUIPrefab;
    GameObject clearUI;

    //---ゲームオーバー
    GameObject failedUIPrefab;
    GameObject failedUI;

    //---操作方法
    GameObject operationUIPrefab;
    GameObject operationUI;

    //---時間UI
    private GameObject timerUI;
    private TimerSliderUI _TimerUI;

    //---客人数UI
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

    //---チュートリアル
    private TutorialManager _TutorialManager;

    void Start()
	{
        canvasObj = GameObject.Find("Canvas");
        canvas = canvasObj.GetComponent<Canvas>();

        if (GameData.nowScene != (int)MySceneManager.SceneState.SCENE_TITLE) {
            InitGameUI();
        }
    }

    void Update() {
        if (GameData.nowScene != (int)MySceneManager.SceneState.SCENE_TITLE) {
            UpdateGameUI();
        }
    }

    /// <summary>
    /// ゲームUIの初期化
    /// </summary>
    private void InitGameUI() {
    #region
        RectTransform _canvasRT = canvasObj.GetComponent<RectTransform>();

        fade = GameObject.Find("FadePanel");

        //----- 操作方法のUIの読み込みと出現 -----
        operationUIPrefab = PrefabContainerFinder.Find(GameData.UIDatas, "OperationUI.prefab");
        operationUI = Instantiate(operationUIPrefab, _canvasRT);
        operationUI.name = operationUIPrefab.name;    // 名前を変更
        operationUI.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする

        //----- クリアのUI読み込みと出現 ------
        clearUIPrefab = PrefabContainerFinder.Find(GameData.UIDatas, "ClearUI.prefab");
        clearUI = Instantiate(clearUIPrefab, _canvasRT);
        clearUI.name = clearUIPrefab.name;    // 名前を変更
        clearUI.transform.SetSiblingIndex(fade.transform.GetSiblingIndex());    // フェードの裏側に来るようにする

        //----- ゲームオーバーのUI読み込みと出現 -----
        failedUIPrefab = PrefabContainerFinder.Find(GameData.UIDatas, "FailedUI.prefab");
        failedUI = Instantiate(failedUIPrefab, _canvasRT);
        failedUI.name = failedUIPrefab.name; // 名前を変更
        failedUI.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする


        //----- タイマーUIの取得 -----
        timerUI = GameObject.Find("TimerSlider");
        if (timerUI) {
            _TimerUI = timerUI.GetComponent<TimerSliderUI>();

            _TimerUI.CountStart();
        } else {
            Debug.LogWarning("TimerUIがシーン上にありません");
        }

        //----- 客人数カウントUIの取得 -----
        guestNumUI = GameObject.Find("GuestNumUI");
        if (guestNumUI) {
            _GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
        } else {
            Debug.LogWarning("GuestNumUIがシーン上にありません");
        }

        //----- リザルトカメラの取得 -----
        GameObject _cameraManagerObj;
        _cameraManagerObj = GameObject.Find("CameraManager");
        if (_cameraManagerObj) {
            _ResultCamera = _cameraManagerObj.GetComponent<ResultCamera>();
        } else {
            Debug.LogWarning("CameraManagerがシーン上にありません");
        }
        resultCamera = GameObject.Find("ResultCamera").GetComponent<Camera>();

        //----- チュートリアルマネージャーの取得 -----
        if (GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
            GameObject _tutotialManagerObj;
            _tutotialManagerObj = GameObject.Find("TutorialManager");
            if (_tutotialManagerObj) {
                _TutorialManager = _tutotialManagerObj.GetComponent<TutorialManager>();
            }
        }
    #endregion
    }

    private void UpdateGameUI() {
        //----- ゲームクリア -----
        if (guestNumUI) {
            if (_GuestNumUI.isClear()) {
                canvas.worldCamera = resultCamera;
                if (_ResultCamera.rotateFlg) {
                    clearUI.SetActive(true);
                }
            } else {
                clearUI.SetActive(false);
            }
        }

        //----- ゲームオーバー -----
        if (timerUI) {
            if (_TimerUI.IsFinish()) {
                failedUI.SetActive(true);
            } else {
                failedUI.SetActive(false);
            }
        }

        //-----  チュートリアル中かで変わるUI -----
        if (GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
            // タイマーの表示非表示
            if (timerUI) {
                var currentTutorialTask = _TutorialManager.GetCurrentTask();
                switch (currentTutorialTask) {
                    case TutorialTask001:           // チュートリアル開始時
                        _TimerUI.CountStop();
                        timerUI.SetActive(false);
                        break;
                    case TutorialTask028:           // タイマーチュートリアル開始
                        timerUI.SetActive(true);
                        break;
                    case TutorialTask032:           // チュートリアル終了
                        _TimerUI.CountStart();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
