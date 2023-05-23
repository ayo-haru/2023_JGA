//=============================================================================
// @File	: [Effect.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/24	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {
    //---客人数UI
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;


    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        //----- 客人数カウントUIの取得 -----
        guestNumUI = GameObject.Find("GuestNumUI");
        if (guestNumUI) {
            _GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
        } else {
            Debug.LogWarning("GuestNumUIがシーン上にありません");
        }
    }


    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        //----- ゲームクリア -----
        if (guestNumUI) {
            if (_GuestNumUI.isClear()) {
                gameObject.SetActive(false);    // クリアしたら非アクティブにして消しとく
            }
        }
    }
}