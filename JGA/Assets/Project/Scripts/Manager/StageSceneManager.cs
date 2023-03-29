//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/16	スポーン地点をPlayerRespwanに変更(吉原)
// 2023/03/20	飼育員自動生成(伊地田)
// 2023/03/21	飼育員自動生成バグとり(伊地田)
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StageSceneManager : BaseSceneManager {
    private GameObject playerObj;
    private GameObject playerInstance;
    [SerializeField] GameObject playerRespawn;

    [SerializeField]
    private Transform[] zooKeeperRootPos;

    GameObject clockUI;
    ClockUI _ClockUI;

    private bool isSceneChangeOnce;



    // デバッグ用チェック
    [Space(100)]
    [Header("---デバッグ用！スポーンさせるかどうか---\n"
        +"そのうちこの項目たちは消しちゃう。\n" +
        "スポーンさせないやつはチェック外してや")]
    [Header("プレイヤースポーン")]
    [SerializeField]
    private bool isPlayerSpawn = true;
    [Header("飼育員スポーン")]
    [SerializeField]
    private bool isZKSpawn = true;
    [Header("客スポーン")]
    [SerializeField]
    private bool isGuestSpawn = true;



    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();
        Application.targetFrameRate = 60;       // FPSを60に固定

        isSceneChangeOnce = false;
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        zooKeeperRootPos = new Transform[Enum.GetNames(typeof(MySceneManager.eRoot)).Length];
        if (isGuestSpawn == true || isZKSpawn == true) {  // デバッグ用エラー出ないように囲んどく
            zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN] = GameObject.Find("PenguinCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.BEAR] = GameObject.Find("BearCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.ELEPHANT] = GameObject.Find("ElephantCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.LION] = GameObject.Find("LionCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.BIRD] = GameObject.Find("BirdCagePos").GetComponent<Transform>();
        }

        //----- プレイヤーの生成 -----
        if (isPlayerSpawn) {
            playerRespawn = GameObject.Find("PlayerSpawn");
            playerObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player.prefab");
            playerInstance = Instantiate(playerObj, playerRespawn.transform.position, Quaternion.Euler(0.0f, 5.0f, 0.0f));
        }

        //----- 飼育員の生成 -----
        if (isZKSpawn) {
            ZooKeeperData.Data[] _zooKeeperList = MySceneManager.GameData.zooKeeperData.list;    // 設定された人数分を生成する
            GameObject zooKeeperObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "ZooKeeper.prefab");   // 生成するオブジェクト
            GameObject parent = GameObject.Find("ZooKeepers");  //  生成するときの親にするオブジェクト
            for (int i = 0; i < _zooKeeperList.Length; i++) {
                GameObject spawnPos = GameObject.Find(_zooKeeperList[i].name + "Spawn"); // 生成位置を名前で取得する
                if (spawnPos == null) { // 存在するか
                    Debug.LogWarning(_zooKeeperList[i].name + "のスポーン位置が見つかりませんでした。(StageSceneManager.cs)");    // 存在しないのでメッセージ出す
                } else {
                    _zooKeeperList[i].respawnTF = spawnPos.GetComponent<Transform>();    // 存在してたので位置を取得
                }

                // 設定された定数から実際のpositionを入れる
                _zooKeeperList[i].rootTransforms = new List<Transform>();
                for (int j = 0; j < _zooKeeperList[i].roots.Length; j++) {
                    _zooKeeperList[i].rootTransforms.Add(zooKeeperRootPos[(int)_zooKeeperList[i].roots[j]]);
                }

                // 生成
                if (spawnPos) {
                    GameObject zooKeeperInstace = Instantiate(zooKeeperObj, spawnPos.transform.position, Quaternion.identity);
                    if (parent) {
                        zooKeeperInstace.transform.parent = parent.transform;   // 親を設定
                    }
                    zooKeeperInstace.name = _zooKeeperList[i].name; // 表示名変更
                    // データをZooKeeperAI.csに流し込む
                    zooKeeperInstace.GetComponent<ZooKeeperAI>().SetData(_zooKeeperList[i]);
                }
            }
        }


        //----- 客の生成 -----
        if (isGuestSpawn) {
            GuestData.Data[] _guestList = MySceneManager.GameData.guestData.dataList;   // 設定された人数分生成する
            GameObject guestObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest.prefab");   // 生成するオブジェクト
            GameObject parent = GameObject.Find("Guests");  // 生成するときの親にするオブジェクト
            for (int i = 0; i < _guestList.Length; i++) {
                // 指定された列挙定数から目的地のブースの実際のpositionを設定
                _guestList[i].rootTransforms = new List<Transform>();
                for (int j = 0; j < _guestList[i].roots.Length; j++) {
                    _guestList[i].rootTransforms.Add(zooKeeperRootPos[(int)_guestList[i].roots[j]]);
                }
                // ペンギンブースの座標を入れる
                _guestList[i].penguinTF = zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN];

                // 生成
                GameObject guestInstace = Instantiate(guestObj, _guestList[i].rootTransforms[0].position, Quaternion.identity);
                if (parent) {
                    guestInstace.transform.parent = parent.transform;   // 親にする
                }
                guestInstace.name = _guestList[i].name; // 表示名変更

                // データの流し込み
                guestInstace.GetComponent<AIManager>().SetGuestData(_guestList[i]);
            }
        }


        clockUI = GameObject.Find("ClockUI");
        if (clockUI) {
            _ClockUI = clockUI.GetComponent<ClockUI>();

            _ClockUI.CountStart();
        } else {
            Debug.LogWarning("ClockUIがシーン上にありません");
        }

        //SoundManager.Play(GetComponent<AudioSource>(),SoundManager.EBGM.TITLE_001);
    }

    //void FixedUpdate() {

    //}

    void Update() {
        if (clockUI) {
            if (_ClockUI.IsFinish()) {
                if (!isSceneChangeOnce) {
                    SceneChange(MySceneManager.SceneState.SCENE_TITLE);
                    isSceneChangeOnce = true;
                }
            }
        }


//        // プロトタイプ用
//        if (Input.GetKeyDown(KeyCode.Escape)){
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
//#else
//    Application.Quit();//ゲームプレイ終了
//#endif
//        }
    }

    private void LateUpdate() {
        if (MySceneManager.GameData.isCatchPenguin) {
            MySceneManager.GameData.isCatchPenguin = false;
        }
    }

    /// <summary>
    /// StageManagerで取得した各ブースの座標を返す
    /// </summary>
    /// <param name="_root"></param>
    /// <returns></returns>
    public Transform GetRootTransform(MySceneManager.eRoot _root) {
        return zooKeeperRootPos[(int)_root];
    }
}
