//=============================================================================
// @File	: [BaseSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/13	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseSceneManager : MonoBehaviour
{
    protected GameObject canvasObj;
    protected Canvas canvas;

    private GameObject fadePanel;

    protected void Init() {
        //----- キャンバスが見つからなかったらキャンバスを作成する -----
        canvasObj = GameObject.Find("Canvas");
        if (canvasObj) {
            canvas = canvasObj.GetComponent<Canvas>();
        } else {
            canvasObj = new GameObject();
            canvasObj.name = "Canvas";
            canvasObj.AddComponent<Canvas>();

            canvas = canvasObj.GetComponent<Canvas>();
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        // キャンバスの設定
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // UIのマネージャーを設定する
        gameObject.AddComponent<UIManager>();
    }


    protected void SceneChange(MySceneManager.SceneState _nextScene) {
        FadeManager.StartFade();
        //if (FadeManager.GetState() != FadeManager.eFade.FadeIn && FadeManager.GetState() != FadeManager.eFade.FadeOut) {
            MySceneManager.SceneChange(_nextScene);
        //}
    }
}
