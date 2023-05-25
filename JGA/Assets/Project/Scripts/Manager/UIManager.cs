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
    GameObject clearUI;
    GameObject clearUIInstance;

    //---ゲームオーバー
    GameObject failedUI;
    GameObject failedUIInstance;

    //---操作方法
    GameObject operationUI;
    GameObject operationUIInstance;

    //---時間UI
    private GameObject timerUI;
    private TimerSliderUI _TimerUI;

    //---客人数UI
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;


    void Start()
	{
        canvasObj = GameObject.Find("Canvas");
        canvas = canvasObj.GetComponent<Canvas>();

        if (SceneManager.GetActiveScene().name != MySceneManager.sceneName[(int)MySceneManager.SceneState.SCENE_TITLE]) {
            RectTransform _canvasRT = canvasObj.GetComponent<RectTransform>();

            fade = GameObject.Find("FadePanel");

            //----- 操作方法のUIの読み込みと出現 -----
            operationUI = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "OperationUI.prefab");
            operationUIInstance = Instantiate(operationUI, _canvasRT);
            operationUIInstance.name = operationUI.name;    // 名前を変更
            operationUIInstance.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする

            //----- クリアのUI読み込みと出現 ------
            clearUI = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "ClearUI.prefab");
            clearUIInstance = Instantiate(clearUI, _canvasRT);
            clearUIInstance.name = clearUI.name;    // 名前を変更
            clearUIInstance.transform.SetSiblingIndex(fade.transform.GetSiblingIndex());    // フェードの裏側に来るようにする

            //----- ゲームオーバーのUI読み込みと出現 -----
            failedUI = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "FailedUI.prefab");
            failedUIInstance = Instantiate(failedUI, _canvasRT);
            failedUIInstance.name = failedUI.name; // 名前を変更
            failedUIInstance.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする


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
        }
    }

    void Update() {
        //----- ゲームクリア -----
        if (guestNumUI) {
            if (_GuestNumUI.isClear()) {
                Debug.Log(canvas);
                canvas.worldCamera = resultCamera;
                if (_ResultCamera.rotateFlg) {
                    clearUIInstance.SetActive(true);
                }
            } else {
                clearUIInstance.SetActive(false);
            }
        }

        //----- ゲームオーバー -----
        if (timerUI) {
            if (_TimerUI.IsFinish()) {
                failedUIInstance.SetActive(true);
            } else {
                failedUIInstance.SetActive(false);
            }
        }
    }
}
