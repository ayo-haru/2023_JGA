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
	GameObject canvas;
	GameObject fadePanel;

    GameObject clearUI;
    GameObject clearUIInstance;

    GameObject failedUI;
    GameObject failedUIInstance;


    //---時間UI
    private GameObject timerUI;
    private TimerUI _TimerUI;

    //---客人数UI
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
        canvas = GameObject.Find("Canvas");


        // GameObject _fadePanel = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "FadePanel.prefab");
        // fadePanel = Instantiate(_fadePanel);

        //fadePanel.transform.parent =  canvas.transform;
        if (SceneManager.GetActiveScene().name != MySceneManager.sceneName[(int)MySceneManager.SceneState.SCENE_TITLE]) {

            //----- クリアのUI読み込みと出現 ------
            clearUI = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "ClearUI.prefab");
            clearUIInstance = Instantiate(clearUI, canvas.GetComponent<RectTransform>());
            clearUIInstance.name = clearUI.name;

            //----- ゲームオーバーのUI読み込みと出現 -----
            failedUI = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "FailedUI.prefab");
            failedUIInstance = Instantiate(failedUI, canvas.GetComponent<RectTransform>());
            failedUIInstance.name = failedUI.name;

            //----- タイマーUIの取得 -----
            timerUI = GameObject.Find("TimerUI");
            if (timerUI) {
                _TimerUI = timerUI.GetComponent<TimerUI>();

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
        }
    }

    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        //----- ゲームクリア -----
        if (guestNumUI) {
            if (_GuestNumUI.isClear()) {
                clearUIInstance.SetActive(true);
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
