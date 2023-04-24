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

        // AudioSourceをつける
        gameObject.AddComponent<AudioSource>();

        // UIのマネージャーを設定する
        gameObject.AddComponent<UIManager>();
    }


    protected void SceneChange(MySceneManager.SceneState _nextScene) {
        FadeManager.StartFadeOut();

        StartCoroutine(DelaySceneChange(_nextScene));
    }
    protected void SceneChange(int _nextScene) {
        FadeManager.StartFadeOut();

        StartCoroutine(DelaySceneChange(_nextScene));
    }


    IEnumerator DelaySceneChange(MySceneManager.SceneState _nextScene) {
        yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.FadeIn);
        MySceneManager.SceneChange(_nextScene);
    }
    IEnumerator DelaySceneChange(int _nextScene) {
        yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.FadeIn);
        MySceneManager.SceneChange(_nextScene);
    }

}
