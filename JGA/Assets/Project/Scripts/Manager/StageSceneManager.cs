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

    private Transform[] zooKeeperRootPos;
    private List<Transform> GuestRootPos;

    GameObject countUI;
    ClockUI _ClockUI;

    private bool isSceneChangeOnce;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();
        Application.targetFrameRate = 60;       // FPSを60に固定

        zooKeeperRootPos = new Transform[Enum.GetNames(typeof(MySceneManager.eRoot)).Length];
        zooKeeperRootPos[(int)MySceneManager.eRoot.PENGUIN] = GameObject.Find("PenguinCagePos").GetComponent<Transform>();
        zooKeeperRootPos[(int)MySceneManager.eRoot.BEAR] = GameObject.Find("BearCagePos").GetComponent<Transform>();
        zooKeeperRootPos[(int)MySceneManager.eRoot.ELEPHANT] = GameObject.Find("ElephantCagePos").GetComponent<Transform>();
        zooKeeperRootPos[(int)MySceneManager.eRoot.LION] = GameObject.Find("LionCagePos").GetComponent<Transform>();
        zooKeeperRootPos[(int)MySceneManager.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
        zooKeeperRootPos[(int)MySceneManager.eRoot.BIRD] = GameObject.Find("birdCagePos").GetComponent<Transform>();

        isSceneChangeOnce = false;
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        //----- プレイヤーの生成 -----
        playerRespawn = GameObject.Find("PlayerSpawn");
        playerObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "Player.prefab");
        playerInstance = Instantiate(playerObj,playerRespawn.transform.position,Quaternion.Euler(0.0f,5.0f,0.0f));

        //----- 飼育員の生成 -----
        //ZooKeeperData.Data[] _list = MySceneManager.GameData.zooKeeperData.list;
        //GameObject zooKeeperObj = PrefabContainerFinder.Find(MySceneManager.GameData.characterDatas, "ZooKeeper.prefab");
        //GameObject parent = GameObject.Find("ZooKeepers");
        //for (int i = 0;i < _list.Length; i++) {
        //    GameObject spawnPos = GameObject.Find(_list[i].name +"Spawn");
        //    if(spawnPos == null) {
        //        Debug.LogError(_list[i].name + "のスポーン位置が見つかりませんでした。(StageSceneManager.cs)");
        //    } else {
        //        _list[i].respawnTF = spawnPos.GetComponent<Transform>();
        //    }

        //    GameObject zooKeeperInstace = Instantiate(zooKeeperObj, spawnPos.transform.position, Quaternion.identity);
        //    for(int j = 0;j < _list[i].roots.Length;j++) {
        //        _list[i].rootTransforms.Add(zooKeeperRootPos[(int)_list[i].roots[j]]);
        //    }
            
        //    zooKeeperInstace.GetComponent<ZooKeeperAI>().SetData(_list[i]);
        //    zooKeeperInstace.transform.parent = parent.transform;
        //}


        //----- 客の生成 -----

        countUI = GameObject.Find("ClockUI");
        _ClockUI = countUI.GetComponent<ClockUI>();

        _ClockUI.CountStart();
    }

    //void FixedUpdate() {

    //}

    void Update() {
        /*
         * ・リスタートがかかったら各オブジェクトをリスタート(初期化)させる
         */

        if (_ClockUI.IsFinish()) {
            if (!isSceneChangeOnce) {
                SceneChange(MySceneManager.SceneState.SCENE_TITLE);
                isSceneChangeOnce= true;
            }
        }


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
