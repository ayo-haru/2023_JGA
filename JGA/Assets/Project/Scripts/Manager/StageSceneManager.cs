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

public class StageSceneManager : BaseSceneManager {
    private GameObject playerObj;
    private GameObject playerInstance;
    private Player _Player;

    private bool isOnce;    // 実行されたら一回だけ呼ぶ処理の一回だけの判定に使う

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();
        Application.targetFrameRate = 60;       // FPSを60に固定
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        playerObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player.prefab");
        playerInstance = Instantiate(playerObj, new Vector3(-102.0f,1.5f,-83.0f),Quaternion.Euler(0.0f,5.0f,0.0f));
    }

    //void FixedUpdate() {

    //}

    void Update() {
        /*
         * ・リスタートがかかったら各オブジェクトをリスタート(初期化)させる
         */

        // プロトタイプ用
        if (Input.GetKeyDown(KeyCode.Escape)){
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }

    private void LateUpdate() {
        if (MySceneManager.GameData.isCatchPenguin) {
            MySceneManager.GameData.isCatchPenguin = false;
        }
    }
}
