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
// 2023/05/12   ゲームオーバー画面を差し替え【小楠】
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class StageSceneManager : BaseSceneManager {
    //---各ブース
    [Header("それぞれのブースの場所を入れる(空でもOK)")]
    [NamedArrayAttribute(new string[] { "PENGUIN_N", "PENGUIN_S", "PENGUIN_W", "PENGUIN_E", "HORSE", "ELEPHANT", "LION", "POLARBEAR", "BIRD", "ENTRANCE" })]
    [SerializeField]
    private Transform[] rootPos;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake() {
        Init();

        TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_GAME_001);
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        //----- それぞれのブースの座標の取得 -----
        rootPos = new Transform[Enum.GetNames(typeof(GameData.eRoot)).Length];
        // 客のルート取得===================================================================
        #region 客ルート
        if (!rootPos[(int)GameData.eRoot.PENGUIN_N]) {
            rootPos[(int)GameData.eRoot.PENGUIN_N] = GameObject.Find("PenguinCagePos_N").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_S]) {
            rootPos[(int)GameData.eRoot.PENGUIN_S] = GameObject.Find("PenguinCagePos_S").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_W]) {
            rootPos[(int)GameData.eRoot.PENGUIN_W] = GameObject.Find("PenguinCagePos_W").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PENGUIN_E]) {
            rootPos[(int)GameData.eRoot.PENGUIN_E] = GameObject.Find("PenguinCagePos_E").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.RESTSPOT_01]) {
            rootPos[(int)GameData.eRoot.RESTSPOT_01] = GameObject.Find("RestSpotPos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.RESTSPOT_02]) {
            rootPos[(int)GameData.eRoot.RESTSPOT_02] = GameObject.Find("RestSpotPos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_01]) {
            rootPos[(int)GameData.eRoot.HORSE_01] = GameObject.Find("HorseCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_02]) {
            rootPos[(int)GameData.eRoot.HORSE_02] = GameObject.Find("HorseCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HORSE_03]) {
            rootPos[(int)GameData.eRoot.HORSE_03] = GameObject.Find("HorseCagePos03").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_01]) {
            rootPos[(int)GameData.eRoot.ZEBRA_01] = GameObject.Find("ZebraCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_02]) {
            rootPos[(int)GameData.eRoot.ZEBRA_02] = GameObject.Find("ZebraCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_03]) {
            rootPos[(int)GameData.eRoot.ZEBRA_03] = GameObject.Find("ZebraCagePos03").GetComponent<Transform>();
        }

        if (!rootPos[(int)GameData.eRoot.POLARBEAR]) {
            rootPos[(int)GameData.eRoot.POLARBEAR] = GameObject.Find("PolarBearCagePos").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_01]) {
            rootPos[(int)GameData.eRoot.BEAR_01] = GameObject.Find("BearCagePos01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_02]) {
            rootPos[(int)GameData.eRoot.BEAR_02] = GameObject.Find("BearCagePos02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA]) {
            rootPos[(int)GameData.eRoot.PANDA] = GameObject.Find("PandaCagePos").GetComponent<Transform>();
        }

        if (!rootPos[(int)GameData.eRoot.ENTRANCE]) {
            rootPos[(int)GameData.eRoot.ENTRANCE] = GameObject.Find("EntrancePos").GetComponent<Transform>();
        }
        #endregion
        //===============================================================================

        // 飼育員巡回ルート取得==============================================================
        #region 飼育員巡回ルール
        if (!rootPos[(int)GameData.eRoot.BELL_AREA]) {
            rootPos[(int)GameData.eRoot.BELL_AREA] = GameObject.Find("BellArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.POLAR_AREA]) {
            rootPos[(int)GameData.eRoot.POLAR_AREA] = GameObject.Find("PolarArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.BEAR_AREA]) {
            rootPos[(int)GameData.eRoot.BEAR_AREA] = GameObject.Find("BearArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA_AREA_01]) {
            rootPos[(int)GameData.eRoot.PANDA_AREA_01] = GameObject.Find("PandaArea01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.PANDA_AREA_02]) {
            rootPos[(int)GameData.eRoot.PANDA_AREA_02] = GameObject.Find("PandaArea02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.HOURSE_AREA]) {
            rootPos[(int)GameData.eRoot.HOURSE_AREA] = GameObject.Find("HourseArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_AREA_01]) {
            rootPos[(int)GameData.eRoot.ZEBRA_AREA_01] = GameObject.Find("ZebraArea01").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.ZEBRA_AREA_02]) {
            rootPos[(int)GameData.eRoot.ZEBRA_AREA_02] = GameObject.Find("ZebraArea02").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.FOUNTAIN_AREA]) {
            rootPos[(int)GameData.eRoot.FOUNTAIN_AREA] = GameObject.Find("FountainArea").GetComponent<Transform>();
        }
        if (!rootPos[(int)GameData.eRoot.LAKE_AREA]) {
            rootPos[(int)GameData.eRoot.LAKE_AREA] = GameObject.Find("LakeArea").GetComponent<Transform>();
        }
        #endregion
        //===============================================================================
    }

    void Update() {
    }

    private void LateUpdate() {
    }

    /// <summary>
    /// StageManagerで取得した各ブースの座標を返す
    /// </summary>
    /// <param name="_root"></param>
    /// <returns></returns>
    public Transform GetRootTransform(GameData.eRoot _root) {
        return rootPos[(int)_root];
    }
}
