//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/16	スポーン地点をPlayerRespwanに変更(吉原)
// 2023/03/20	飼育員自動生成(伊地田)
// 2023/03/21	飼育員自動生成バグとり(伊地田)
// 2023/03/30	ペンギンブースをリストに変更しました。【小楠】
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StageSceneManager : BaseSceneManager {
    //---プレイヤー
    private GameObject playerObj;
    private GameObject playerInstance;
    [SerializeField] GameObject playerRespawn;

    //---各ブース
    [NamedArrayAttribute(new string[] { "PENGUIN_N", "PENGUIN_S", "PENGUIN_W", "PENGUIN_E", "HORSE", "ELEPHANT", "LION", "POLARBEAR", "BIRD","ENTRANCE" })]
    [SerializeField]
    [Header("それぞれのブースの場所を入れる(空でもOK)")]
    private Transform[] zooKeeperRootPos;

    //---客
    [SerializeField]
    [Header("ランダム生成させる客の合計の数")]
    private int randomGuestMax = 1;
    [SerializeField]
    [Header("ランダム生成させる客のルートの最大数")]
    private int guestRootMax = 5;
    private GameObject guestParent; // 客を生成したときに親にするオブジェクト
    private GameObject guestObj;    // 生成する客のプレハブ
    private int guestSum;           // 生成した数(連番振るのに使う)

    //---時間UI
    private GameObject timerUI;
    private TimerUI _TimerUI;


    //---変数
    private bool isSceneChangeOnce; // 一度だけ処理をするときに使う



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
        //----- 変数初期化 -----
        // 実際に使うデータとデバッグ用に登録していたデータが違ったら変更
        if (this.randomGuestMax != MySceneManager.GameData.randomGuestMax) {
            MySceneManager.GameData.randomGuestMax = this.randomGuestMax;
        }

        // 客の生成する親オブジェクトの取得
        guestParent = GameObject.Find("Guests");

        // 生成する客のプレハブ
        guestObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest.prefab");   
        
        
        //----- それぞれのブースの座標の取得 -----
        zooKeeperRootPos = new Transform[Enum.GetNames(typeof(MySceneManager.eRoot)).Length];
        if (isGuestSpawn == true || isZKSpawn == true) {  // デバッグ用エラー出ないように囲んどく
            zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_N] = GameObject.Find("PenguinCagePos_N").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_S] = GameObject.Find("PenguinCagePos_S").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_W] = GameObject.Find("PenguinCagePos_W").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_E] = GameObject.Find("PenguinCagePos_E").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.HORSE] = GameObject.Find("HorseCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.ELEPHANT] = GameObject.Find("ElephantCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.LION] = GameObject.Find("LionCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.BIRD] = GameObject.Find("BirdCagePos").GetComponent<Transform>();
            zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE] = GameObject.Find("EntrancePos").GetComponent<Transform>();
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
            //----- データで決められた客 -----
            GuestData.Data[] _guestList = MySceneManager.GameData.guestData.dataList;   // 設定された人数分生成する
            for (int i = 0; i < _guestList.Length; i++) {
                // 指定された列挙定数から目的地のブースの実際のpositionを設定
                _guestList[i].rootTransforms = new List<Transform>();
                for (int j = 0; j < _guestList[i].roots.Length; j++) {
                    _guestList[i].rootTransforms.Add(zooKeeperRootPos[(int)_guestList[i].roots[j]]);
                }
                // ペンギンブースの座標を入れる
                _guestList[i].penguinTF = new List<Transform>();
                _guestList[i].penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_N]);
                _guestList[i].penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_S]);
                _guestList[i].penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_W]);
                _guestList[i].penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_E]);

                // エントランスの座標を入れる
                _guestList[i].entranceTF = zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE];

                // 生成
                GameObject guestInstace = Instantiate(guestObj, _guestList[i].rootTransforms[0].position, Quaternion.identity);
                if (guestParent) {
                    guestInstace.transform.parent = guestParent.transform;   // 親にする
                }
                guestInstace.name = _guestList[i].name; // 表示名変更

                // データの流し込み
                guestInstace.GetComponent<AIManager>().SetGuestData(_guestList[i]);
            }

            //----- ランダムで生成する客 -----
            GuestData.Data guestData = new GuestData.Data { // 初期化
                name = "FGuest",
                entranceTF = zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE],
                isRandom = true,
                speed = 3,
                followSpeed = 0.7f,
                inBoothSpeed = 0.5f,
                rayLength = 10.0f,
                viewAngle = 60.0f,
                reactionArea = 25,
                distance = 2,
                firstCoolDownTime = 3.0f,
                secondCoolDownTime = 5.0f,
                waitTime = 0,
                cageDistance = 10.0f
            };
            // ペンギンブースの座標を入れる
            guestData.penguinTF = new List<Transform>();
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_N]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_S]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_W]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_E]);

            for (int i = MySceneManager.GameData.randomGuestCnt; i < randomGuestMax; i++) {
                //----- 目的地のブースをランダムで設定(固定客は作ってない) -----
                int randomRootSum, randomRootNum;

                // ルートをいくつ設定するかをランダムで決定
                randomRootSum = UnityEngine.Random.Range(2, guestRootMax + 1);    // ランダムの最大値は1大きくしないと設定した数が含まれない

                guestData.rootTransforms = new List<Transform>();
                for (int j = 0; j < randomRootSum; j++) {   //　乱数で算出したルートの数だけルートを決める
                    // ルートをランダムで設定
                    randomRootNum = UnityEngine.Random.Range(4, (int)MySceneManager.eRoot.ENTRANCE);  // ペンギンのルートが入っているところより大きく、エントランスより小さく
                    guestData.rootTransforms.Add(zooKeeperRootPos[randomRootNum]);
                    Debug.Log(zooKeeperRootPos[randomRootNum]);
                }


                // 生成
                guestSum = MySceneManager.GameData.randomGuestCnt = MySceneManager.GameData.randomGuestCnt + 1;
                GameObject guestInstace = Instantiate(guestObj, zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE].position, Quaternion.identity);
                if (guestParent) {
                    guestInstace.transform.parent = guestParent.transform;   // 親にする
                }
                guestInstace.name = guestData.name + String.Format("{0:D3}", guestSum); // 表示名変更
                // データの流し込み
                guestInstace.GetComponent<AIManager>().SetGuestData(guestData);
            }

        }


        timerUI = GameObject.Find("TimerUI");
        if (timerUI) {
            _TimerUI = timerUI.GetComponent<TimerUI>();

            _TimerUI.CountStart();
        } else {
            Debug.LogWarning("TimerUIがシーン上にありません");
        }
    }

    void Update() {
        if (timerUI) {
            if (_TimerUI.IsFinish()) {
                if (!isSceneChangeOnce) {
                    SceneChange(MySceneManager.SceneState.SCENE_TITLE);
                    isSceneChangeOnce = true;
                }
            }
        }

        if (isGuestSpawn) {
            GuestData.Data guestData = new GuestData.Data { // 初期化
                name = "FGuest",
                entranceTF = zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE],
                isRandom = true,
                speed = 3,
                followSpeed = 0.7f,
                inBoothSpeed = 0.5f,
                rayLength = 10.0f,
                viewAngle = 60.0f,
                reactionArea = 25,
                distance = 2,
                firstCoolDownTime = 3.0f,
                secondCoolDownTime = 5.0f,
                waitTime = 0,
                cageDistance = 10.0f
            };
            // ペンギンブースの座標を入れる
            guestData.penguinTF = new List<Transform>();
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_N]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_S]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_W]);
            guestData.penguinTF.Add(zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN_E]);

            for (int i = MySceneManager.GameData.randomGuestCnt; i <= randomGuestMax; i++) {
                //----- 目的地のブースをランダムで設定(固定客は作ってない) -----
                int randomRootSum,randomRootNum;
                
                // ルートをいくつ設定するかをランダムで決定
                randomRootSum = UnityEngine.Random.Range(2, guestRootMax+1);    // ランダムの最大値は1大きくしないと設定した数が含まれない

                guestData.rootTransforms = new List<Transform>();
                for (int j = 0; j < randomRootSum; j++) {   //　乱数で算出したルートの数だけルートを決める
                    // ルートをランダムで設定
                    randomRootNum = UnityEngine.Random.Range(5, (int)MySceneManager.eRoot.ENTRANCE);  // ペンギンのルートが入っているところより大きく、エントランスより小さく
                    guestData.rootTransforms.Add(zooKeeperRootPos[randomRootNum]);
                }


                // 生成
                guestSum = MySceneManager.GameData.randomGuestCnt = MySceneManager.GameData.randomGuestCnt + 1;
                GameObject guestInstace = Instantiate(guestObj, zooKeeperRootPos[(int)MySceneManager.eRoot.ENTRANCE].position, Quaternion.identity);
                if (guestParent) {
                    guestInstace.transform.parent = guestParent.transform;   // 親にする
                }
                guestInstace.name = guestData.name + String.Format("{0:D3}",guestSum); // 表示名変更

                // データの流し込み
                guestInstace.GetComponent<AIManager>().SetGuestData(guestData);
            }
        }

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
