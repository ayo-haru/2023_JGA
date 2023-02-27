//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSceneManager : MonoBehaviour {
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Application.targetFrameRate = 60;       // FPSを60に固定
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {

    }

    //void FixedUpdate() {

    //}

    void Update() {
        /*
         * ・ペンギンが飼育員に捕まったらリスタート
         * ・リスタートがかかったら各オブジェクトをリスタート(初期化)させる
         */
    }
}