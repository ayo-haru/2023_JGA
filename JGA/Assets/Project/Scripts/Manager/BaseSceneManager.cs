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
    protected void Init() {
        // AudioSourceをつける
        gameObject.AddComponent<AudioSource>();
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
