//=============================================================================
// @File	: [FadeManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Sakai Ryotaro Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/08	スクリプト作成
// 2023/04/04	フェード時にポーズ画面を表示させない処理を追加
// 2023/05/22   フェード中の時だけraycastTargetをtrueにした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
    public enum eFade {
        Default,
        FadeIn,
        FadeOut,
        WAIT
    }

    [Header("α値に加算減算する値")]
    [SerializeField]
    private float speed = 0.01f;

    private GameObject fadePanel;
    private Image image;
    private static float alpha;

    [System.NonSerialized]
    public static eFade fadeMode;
    private static eFade oldFadeMode;

    private static AudioSource audioSource;

    private void Awake() {
        oldFadeMode = fadeMode = eFade.Default;

        alpha = 0.0f;
        fadePanel = GameObject.Find("FadePanel");
        image = fadePanel.GetComponent<Image>();
        image.color = new Color(0.0f, 0.0f, 0.0f, alpha);

        audioSource = gameObject.AddComponent<AudioSource>();   // オーディオソースの追加と保存
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        // シーンが切り替わった瞬間フェードイン開始するイベント
        SceneManager.activeSceneChanged += StartFadeIn;
    }

    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        if (GameData.isCatchPenguin) {   // ペンギンを捕まえたらブラックアウトからフェードイン開始
            StartFadeIn();
        }

        // 何かしらのフェードが始まったら音が鳴り終わるまで待つ処理を入れる
        if (oldFadeMode == eFade.Default && fadeMode != eFade.Default) {
            if (fadeMode == eFade.FadeIn) {
                StartCoroutine(WaitChangeFadeMode(eFade.FadeIn));
            }
        }

        if (fadeMode != eFade.Default) {
            if (fadeMode == eFade.FadeOut) {
                if (alpha < 1.0f) {
                    alpha += speed * 2;
                } else {
                    oldFadeMode = fadeMode;
                    fadeMode = eFade.FadeIn;
                }
            } else if (fadeMode == eFade.FadeIn) {
                if (alpha > 0.1f) {
                    alpha -= speed;
                } else {
                    oldFadeMode = fadeMode;
                    fadeMode = eFade.Default;

                    alpha = 0.0f;
                }
            }
        }

        image.color = new Color(0.0f, 0.0f, 0.0f, alpha);

        image.raycastTarget = (fadeMode != eFade.Default);
    }

    /// <summary>
    /// フェードアウト開始。フェードアウトが終わったらフェードインする。
    /// </summary>
    public static void StartFadeOut() {
        alpha = 0.0f;

        oldFadeMode = fadeMode;
        fadeMode = eFade.FadeOut;
    }

    /// <summary>
    /// フェードイン開始。フェードインさせたいときに書く
    /// </summary>
    public static void StartFadeIn() {
        alpha = 1.0f;

        oldFadeMode = fadeMode;
        fadeMode = eFade.FadeIn;
    }
    /// <summary>
    /// フェードイン開始。シーン切り替わった瞬間のイベント用関数。
    /// </summary>
    /// <param name="thisScene"></param>
    /// <param name="nextScene"></param>
    public void StartFadeIn(Scene thisScene, Scene nextScene) {
        alpha = 1.0f;

        oldFadeMode = fadeMode;
        fadeMode = eFade.FadeIn;
    }

    /// <summary>
    /// 現在のフェードのステートを取る
    /// </summary>
    /// <returns></returns>
    public static eFade GetState() {
        return fadeMode;
    }


    /// <summary>
    /// フェードのモードを変える
    /// </summary>
    /// <param name="_FadeMode"></param>
    /// <returns></returns>
    public IEnumerator WaitChangeFadeMode(eFade _FadeMode) {
        oldFadeMode = fadeMode = eFade.WAIT;
        yield return StartCoroutine(WaitSound(SoundManager.ESE.STEALDOOR_OPEN));

        yield return StartCoroutine(WaitSound(SoundManager.ESE.STEALDOOR_CLOSE));

        oldFadeMode = fadeMode = _FadeMode;
    }


    /// <summary>
    /// 音が鳴っている間処理を待つ
    /// </summary>
    /// <param name="_se"></param>
    /// <returns></returns>
    public IEnumerator WaitSound(SoundManager.ESE _se) {
        if (!audioSource.isPlaying) {
            SoundManager.Play(audioSource, _se);
        }
        while (audioSource.isPlaying) {
            yield return new WaitForEndOfFrame();
        }
    }
}
