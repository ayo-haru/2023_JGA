//=============================================================================
// @File	: [OperationUI.cs]
// @Brief	: 操作方法のUI
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationUI : MonoBehaviour {
    //---プレイヤー
    private GameObject player;      // プレイヤーのオブジェクト
    private Player _Player;         // 移動中か取得用プレイヤーのスクリプト

    private bool NowPlayerMove;     // 現在移動中か
    private bool OldPlayerMove;     // 前フレームで移動中だったか

    //---画像
    private List<Image> images;     // 子オブジェクト全ての画像

    private float alpha;            // アルファ値
        
    private float alphaRate = 0.05f; // アルファ値に加算減算用定数
    private float fadeInTime = 1.0f; // フェードインするのに待機する時間

    private enum FadeState {        // フェードのステート 
        NONE = 0,
        FADE_OUT,
        FADE_IN
    }
    private FadeState state;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        // プレイヤー取得
        player = GameObject.FindGameObjectWithTag("Player");

        if (player) {
            _Player = player.GetComponent<Player>();
            NowPlayerMove = OldPlayerMove = _Player.IsMove;

        } else {
            Debug.LogError("プレイヤーが見つかりませんでした");
        }

        // 子オブジェクト全ての画像を取得
        images = new List<Image>();
        GetComponentsInChildren<Image>(images);

        alpha = 1.0f;　// アルファ値

        state = FadeState.NONE; // フェードのステート
    }

    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        // 移動中か取得
        NowPlayerMove = _Player.IsMove;

        //----- フェードのステート切り替え -----
        /*
		 * 止まり→動く　：　表示する
		 * 動いている→止まった　：　消す
		 */
        if (NowPlayerMove == true) {
            if (OldPlayerMove == false) {   // 動き始めた
                state = FadeState.FADE_OUT;
                OldPlayerMove = NowPlayerMove;
            }
        } else {
            if (OldPlayerMove == true) { // 止まった
                StartCoroutine(DelayStartFadeIn());
                OldPlayerMove = NowPlayerMove;
            }
        }

        //----- アルファ値の加算減算処理 -----
        if (state == FadeState.FADE_OUT) {
            if (alpha >= 0.0f) {
                alpha -= alphaRate;
            } else {
                state = FadeState.NONE;
            }
        } else if (state == FadeState.FADE_IN) {
            if (alpha <= 1.0f) {
                alpha += alphaRate;
            } else {
                state = FadeState.NONE;
            }
        }

        // 持っている画像全てのアルファ値を設定
        for (int i = 0; i < images.Count; i++) {
            images[i].color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    /// <summary>
    /// フェードインを少し待機
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayStartFadeIn() {
        yield return new WaitForSeconds(fadeInTime);
        if (NowPlayerMove == false) {   // コルーチンで待機中の間に移動した場合は表示させない
            state = FadeState.FADE_IN;
        }
    }
}