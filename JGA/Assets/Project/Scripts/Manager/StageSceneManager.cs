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
// 2023/04/     客の自動生成
// 2023/04/24   クリア
// 2023/05/06   クリア画面を差し替え【小楠】
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSceneManager : BaseSceneManager {
    //---プレイヤー
    private GameObject playerObj;
    private GameObject playerInstance;
    [SerializeField] GameObject playerRespawn;

    //---イベント
    [Header("タイマーの丸のそれぞれのイベント")]
    [SerializeField]
    private EventParam[] events = default;
    private string[] eventChar = new string[] { "エントランスに向かおう", "鐘を鳴らそう", "しろくまブースへ向かおう", "うまブースへ向かおう", "メガホンを使ってみよう", "もうすぐ、時間切れ" };
    [SerializeField]
    [Header("イベントのテキスト")]
    private GameObject eventText;
    private GameObject eventTextInstance;
    private Text eventTextChar;

    //---各ブース
    [Header("それぞれのブースの場所を入れる(空でもOK)")]
    [NamedArrayAttribute(new string[] { "PENGUIN_N", "PENGUIN_S", "PENGUIN_W", "PENGUIN_E", "HORSE", "ELEPHANT", "LION", "POLARBEAR", "BIRD", "ENTRANCE" })]
    [SerializeField]
    private Transform[] rootPos;

    //---客
    [Header("ランダム生成させる客のルートの最大数\n(2～ペンギンとエントランス以外のブースの合計数)")]
    [SerializeField]
    [Range(2, (int)MySceneManager.eRoot.ENTRANCE - 4)]
    private int guestRootMax = 5;
    [Header("ランダム生成する間隔(秒)")]
    [SerializeField]
    [Range(1, 30)]
    private int guestSpawnTime = 1;
    private GameObject guestParent; // 客を生成したときに親にするオブジェクト
    private GameObject[] guestObj;  // 生成する客のプレハブ
    private int guestSum;           // 生成した数(連番振るのに使う)
    private int guestSpawnTimer;    // ランダム生成のカウントに使う

    //---時間UI
    private GameObject timerUI;
    private TimerSliderUI _TimerUI;

    //---客人数UI
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

    //-----クリア画面
    private GameObject clearPanel;
    private ClearPanel _ClearPanel;


    //---変数
    private bool isOnce; // 一度だけ処理をするときに使う

    MyContorller inputAction;

    // デバッグ用チェック
    [Space(100)]
    [Header("---デバッグ用！スポーンさせるかどうか---\n"
        + "そのうちこの項目たちは消しちゃう。\n" +
        "スポーンさせないやつはチェック外してや")]
    [Header("プレイヤースポーン")]
    [SerializeField]
    private bool isPlayerSpawn = true;
    private enum PlayerMode {
        Penguin = 0,
        UnityChan = 1
    }
    [Header("プレイヤーのモデルどっちにするか選んでね")]
    [SerializeField]
    private PlayerMode playerMode = PlayerMode.Penguin;
    [Header("飼育員スポーン")]
    [SerializeField]
    private bool isZKSpawn = true;
    private enum ZookeeperMode {
        OLD = 0,
        NEW = 1
    }
    [Header("飼育員のモデルどっちにするか選んでね")]
    [SerializeField]
    private ZookeeperMode zookeeperMode = ZookeeperMode.NEW;

    [Header("客スポーン")]
    [SerializeField]
    private bool isGuestSpawn = true;
    private enum GuestMode {
        OLD = 0,
        NEW = 1
    }
    [Header("客のモデルどっちにするか選んでね")]
    [SerializeField]
    private GuestMode guestMode = GuestMode.NEW;
    private GameObject guestObj_old;    // 生成する客のプレハブ




    //-0510デバッグ用---------------------------
    int stateCnt = 0;
    //----------------------------



    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();
        Application.targetFrameRate = 60;       // FPSを60に固定

        isOnce = false;

        inputAction = new MyContorller();
        inputAction.Enable();

        //----- イベントを静的クラスに保存 -----
        MySceneManager.GameData.events = new EventParam[this.events.Length];
        for (int i = 0; i < events.Length; i++) {
            /*
             * 上の ”new EventParam[this.events.Length]"ではサイズを確保しているだけで初期化はされていないので
             * 下の ”new EventParam()”というように ”()” をつけコンストラクタを呼び出し初期化する
             */
            MySceneManager.GameData.events[i] = new EventParam();
            MySceneManager.GameData.events[i].eventState = this.events[i].eventState;
            MySceneManager.GameData.events[i].percent = this.events[i].percent;
        }

        //---デバッグ用------------------------------------------------------------------------------
        /*
         * デバッグ用ちゃんとシーンが出来たらこれは消す
         * クリア作るのに次のシーンへいくのやりたかた
         * 現在のシーン名からシーン番号を取得する
         */
        for (int i = 0; i < System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length; i++) {
            if (SceneManager.GetActiveScene().name == MySceneManager.sceneName[i]) {
                MySceneManager.GameData.nowScene = i;
            }
        }
        //-------------------------------------------------------------------------------------------
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        //----- 変数初期化 -----
        // 客の生成する親オブジェクトの取得
        guestParent = GameObject.Find("Guests");

        // 生成する客のプレハブ
        if (guestMode == GuestMode.OLD) {
            guestObj_old = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest(old).prefab");
        } else {
            guestObj_old = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest(old).prefab");

            guestObj = new GameObject[3];
            guestObj[0] = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest001.prefab");
            guestObj[1] = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest002.prefab");
            guestObj[2] = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Guest003.prefab");
        }

        // 客ランダム生成の変数初期化
        guestSpawnTimer = guestSpawnTime * 60;

        //----- それぞれのブースの座標の取得 -----
        rootPos = new Transform[Enum.GetNames(typeof(MySceneManager.eRoot)).Length];
        if (isGuestSpawn == true || isZKSpawn == true) {  // デバッグ用エラー出ないように囲んどく
            if (!rootPos[(int)MySceneManager.eRoot.PENGUIN_N]) {
                rootPos[(int)MySceneManager.eRoot.PENGUIN_N] = GameObject.Find("PenguinCagePos_N").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.PENGUIN_S]) {
                rootPos[(int)MySceneManager.eRoot.PENGUIN_S] = GameObject.Find("PenguinCagePos_S").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.PENGUIN_W]) {
                rootPos[(int)MySceneManager.eRoot.PENGUIN_W] = GameObject.Find("PenguinCagePos_W").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.PENGUIN_E]) {
                rootPos[(int)MySceneManager.eRoot.PENGUIN_E] = GameObject.Find("PenguinCagePos_E").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.HORSE]) {
                rootPos[(int)MySceneManager.eRoot.HORSE] = GameObject.Find("HorseCagePos").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.ELEPHANT]) {
                rootPos[(int)MySceneManager.eRoot.ELEPHANT] = GameObject.Find("ElephantCagePos").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.LION]) {
                rootPos[(int)MySceneManager.eRoot.LION] = GameObject.Find("LionCagePos").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.POLARBEAR]) {
                rootPos[(int)MySceneManager.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.BIRD]) {
                rootPos[(int)MySceneManager.eRoot.BIRD] = GameObject.Find("BirdCagePos").GetComponent<Transform>();
            }
            if (!rootPos[(int)MySceneManager.eRoot.ENTRANCE]) {
                rootPos[(int)MySceneManager.eRoot.ENTRANCE] = GameObject.Find("EntrancePos").GetComponent<Transform>();
            }
        }

        //----- プレイヤーの生成 -----
        if (isPlayerSpawn) {
            playerRespawn = GameObject.Find("PlayerSpawn");
            if (playerMode == PlayerMode.Penguin) {
                playerObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player.prefab");
            } else {
                playerObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player(old).prefab");
            }
            playerInstance = Instantiate(playerObj, playerRespawn.transform.position, Quaternion.Euler(0.0f, 5.0f, 0.0f));
            playerInstance.name = "Player";
        }

        //----- 飼育員の生成 -----
        if (isZKSpawn) {
            SpawnZookeeper();
        }


        //----- 客の生成 -----
        if (isGuestSpawn) {
            //----- データで決められた客 -----
            SpawnFixGuest();
        }

        //----- タイマーUIの取得 -----
        timerUI = GameObject.Find("TimerSlider");
        if (timerUI) {
            _TimerUI = timerUI.GetComponent<TimerSliderUI>();

            _TimerUI.CountStart();
        } else {
            Debug.LogWarning("TimerUIがシーン上にありません");
        }

        //----- 客人数カウントUIの取得 -----
        guestNumUI = GameObject.Find("GuestNumUI");
        if (guestNumUI) {
            _GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
        } else {
            Debug.LogWarning("GuestNumUIがシーン上にありません");
        }

        //eventTextInstance = Instantiate(eventText, canvas.GetComponent<RectTransform>());
        //eventTextChar = eventTextInstance.GetComponent<Text>();
    }

    void Update() {
        //----- 時間系の処理 -----
        if (timerUI) {
            //----- 制限時間のゲームオーバー -----
            if (_TimerUI.IsFinish()) {
                if (!isOnce) {
                    if (inputAction.Menu.Decision.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint) { // 入力があったら
                        SceneChange(MySceneManager.SceneState.SCENE_TITLE);
                        isOnce = true;
                    }
                }

                if (!PauseManager.isPaused) {
                    PauseManager.isPaused = true;
                    PauseManager.NoMenu = true;
                    PauseManager.Pause();
                }

            }

            //----- ランダムで生成する客 -----
            if (isGuestSpawn) {
                if (_TimerUI.CurrentTimerPoint() - 1 > -1) {    // タイマーはスタート0イベント1個目は1。イベントは0番目から格納されているので帳尻合わせの条件分岐

                    guestSpawnTimer--;
                    if (guestSpawnTimer <= 0) { // 間隔開けて生成
                        if (events[_TimerUI.CurrentTimerPoint() - 1].eventState == MySceneManager.eEvent.GUEST_ENTER) {  // 現在のイベントがエントランスに入るだったら
                            // 生成
                            SpawnRondomGuest();
                            // カウントリセット
                            guestSpawnTimer = guestSpawnTime * 60;
                        }
                    }

                    // テキスト
                    //switch (events[_TimerUI.CurrentTimerPoint() - 1].eventState) {
                    //    case MySceneManager.eEvent.GUEST_ENTER:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GUEST_ENTER];
                    //        break;
                    //    case MySceneManager.eEvent.SOUND_BELL:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.SOUND_BELL];
                    //        break;
                    //    case MySceneManager.eEvent.GO_POLARBEAR:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GO_POLARBEAR];
                    //        break;
                    //    case MySceneManager.eEvent.GO_HOURSE:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GO_HOURSE];

                    //        break;
                    //    case MySceneManager.eEvent.OBJ_MEGAPHON:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.OBJ_MEGAPHON];

                    //        break;
                    //    case MySceneManager.eEvent.TIMEOUT_ALERT:
                    //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.TIMEOUT_ALERT];

                    //        break;
                    //    default:
                    //        Debug.LogWarning("イベント番号が無効な値を受け取りました");
                    //        break;
                    //};
                }
            }
        }

        //----- ゲームクリア -----
        if (guestNumUI) {
            if (_GuestNumUI.isClear()) {

                //----クリア画面取得
                if (!clearPanel) clearPanel = GameObject.Find("ClearPanel");
                if (clearPanel && !_ClearPanel) _ClearPanel = clearPanel.GetComponent<ClearPanel>();

                if (!isOnce) {   // 一度だけ処理
                    int next = -1;
                    if (_ClearPanel) next = _ClearPanel.GetNextScene();
                    if (next != -1) {
                        MySceneManager.GameData.nowScene = next;
                        SceneChange(next);  // シーン遷移
                        isOnce = true;
                    }
                }
            }
        }

#if UNITY_EDITOR
        // デバッグ用イベントのカウント上がります
        if (Input.GetKeyDown(KeyCode.F2)) {
            stateCnt++;
        }
#endif

    }

    private void LateUpdate() {
        //----- 飼育員に捕まったフラグを下す -----
        /*
         * Updateで下ろすと各処理と同フレーム中にフラグが降りてしまうためLateに書いた
         */
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
        return rootPos[(int)_root];
    }

    /// <summary>
    /// 固定客生成
    /// </summary>
    private void SpawnFixGuest() {
        GuestData.Data[] _guestList = MySceneManager.GameData.guestData.dataList;   // 設定された人数分生成する
        for (int i = 0; i < _guestList.Length; i++) {
            // 指定された列挙定数から目的地のブースの実際のpositionを設定
            _guestList[i].rootTransforms = new List<Transform>();
            for (int j = 0; j < _guestList[i].roots.Length; j++) {
                _guestList[i].rootTransforms.Add(rootPos[(int)_guestList[i].roots[j]]);
            }
            // ペンギンブースの座標を入れる
            _guestList[i].penguinTF = new List<Transform>();
            _guestList[i].penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_N]);
            _guestList[i].penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_S]);
            _guestList[i].penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_W]);
            _guestList[i].penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_E]);

            // エントランスの座標を入れる
            _guestList[i].entranceTF = rootPos[(int)MySceneManager.eRoot.ENTRANCE];

            // 生成
            GameObject guestInstace;
            if (guestMode == GuestMode.OLD) {
                guestInstace = Instantiate(guestObj_old, _guestList[i].rootTransforms[0].position, Quaternion.identity);
            } else {
                int GuestIndex = UnityEngine.Random.Range(0, 3);
                guestInstace = Instantiate(guestObj[GuestIndex], _guestList[i].rootTransforms[0].position, Quaternion.identity);
            }
            if (guestParent) {
                guestInstace.transform.parent = guestParent.transform;   // 親にする
            }
            guestInstace.name = _guestList[i].name; // 表示名変更

            // データの流し込み
            guestInstace.GetComponent<AIManager>().SetGuestData(_guestList[i]);
        }
    }

    /// <summary>
    /// ランダム生成客(これ１回呼び出すと一体生成)
    /// </summary>
    private void SpawnRondomGuest() {
        GuestData.Data guestData = new GuestData.Data { // 初期化
            name = "FGuest",
            entranceTF = rootPos[(int)MySceneManager.eRoot.ENTRANCE],
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
        guestData.penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_N]);
        guestData.penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_S]);
        guestData.penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_W]);
        guestData.penguinTF.Add(rootPos[(int)MySceneManager.eRoot.PENGUIN_E]);

        //----- 目的地のブースをランダムで設定(固定客は作ってない) -----
        int randomRootSum, randomRootNum;

        // ルートをいくつ設定するかをランダムで決定
        randomRootSum = UnityEngine.Random.Range(2, guestRootMax + 1);    // ランダムの最大値は1大きくしないと設定した数が含まれない

        guestData.rootTransforms = new List<Transform>();
        for (int j = 0; j < randomRootSum; j++) {   //　乱数で算出したルートの数だけルートを決める
                                                    // ルートをランダムで設定
            do {
                randomRootNum = UnityEngine.Random.Range(4, (int)MySceneManager.eRoot.ENTRANCE);  // ペンギンのルートが入っているところより大きく、エントランスより小さく
            } while (guestData.rootTransforms.Contains(rootPos[randomRootNum]));
            guestData.rootTransforms.Add(rootPos[randomRootNum]);
        }

        //----- 生成 -----
        // それぞれのカウントを加算
        guestSum++;
        MySceneManager.GameData.randomGuestCnt++;

        GameObject guestInstace;
        if (guestMode == GuestMode.OLD) {
            guestInstace = Instantiate(guestObj_old, rootPos[(int)MySceneManager.eRoot.ENTRANCE].position, Quaternion.identity);
        } else {
            int GuestIndex = UnityEngine.Random.Range(0, 3);
            guestInstace = Instantiate(guestObj[GuestIndex], rootPos[(int)MySceneManager.eRoot.ENTRANCE].position, Quaternion.identity);
        }

        if (guestParent) {
            guestInstace.transform.parent = guestParent.transform;   // 親にする
        }
        guestInstace.name = guestData.name + String.Format("{0:D3}", guestSum); // 表示名変更
                                                                                // データの流し込み
        guestInstace.GetComponent<AIManager>().SetGuestData(guestData);
    }

    /// <summary>
    /// 飼育員生成
    /// </summary>
    private void SpawnZookeeper() {
        ZooKeeperData.Data[] _zooKeeperList = MySceneManager.GameData.zooKeeperData.list;    // 設定された人数分を生成する
        GameObject zooKeeperObj;
        if (zookeeperMode == ZookeeperMode.OLD) {
            zooKeeperObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "ZooKeeper(old).prefab");   // 生成するオブジェクト
        } else {
            zooKeeperObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "ZooKeeper.prefab");   // 生成するオブジェクト
        }
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
                _zooKeeperList[i].rootTransforms.Add(rootPos[(int)_zooKeeperList[i].roots[j]]);
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
}
