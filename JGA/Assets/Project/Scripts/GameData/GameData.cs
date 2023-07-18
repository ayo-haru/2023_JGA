//=============================================================================
// @File	: [GameData.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/01	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    //---プレハブの登録
    public static PrefabContainer characterDatas;
    public static PrefabContainer UIDatas;
    public static PrefabContainer animalDatas;
    //public static PrefabContainer gimmickDatas;
    public static PrefabContainer stageObjDatas;
    public static PrefabContainer managerObjDatas;

    //---データの登録
    public static ZooKeeperData[] zooKeeperData;
    public static GuestData[] guestData;

    //---フラグ
    public static bool isCatchPenguin;

    //---データ
    public static int randomGuestCnt;   // ランダム生成させる客の今の数
    public static int oldScene;         // ひとつ前のシーン番号
    public static int nowScene;         // 現在のシーン番号
    public static EventParam[] events;  // 各シーンのイベント
    public static int guestCnt;         // 客のカウント
    public static float timer;          // ゲームのタイマー 
    public static Vector3 playerPos;    // プレイヤーの座標
    public static bool isContinueGame;  // ゲーム続きからやってる？セーブデータがあるならture

    //----- 飼育員、客のルートに使用 -----
    public enum eRoot {
        // 客用
        PENGUIN_N = 0,      // ペンギンブース北
        PENGUIN_S,          // ペンギンブース南
        PENGUIN_W,          // ペンギンブース西
        PENGUIN_E,          // ペンギンブース東
        RESTSPOT_01,        // 休憩スペース1
        RESTSPOT_02,        // 休憩スペース2
        HORSE_01,           // ウマ1
        HORSE_02,           // ウマ2
        HORSE_03,           // ウマ3
        ZEBRA_01,           // シマウマ1
        ZEBRA_02,           // シマウマ2
        ZEBRA_03,           // シマウマ3
        POLARBEAR,          // シロクマ
        BEAR_01,            // クマ1
        BEAR_02,            // クマ2
        PANDA,              // パンダ


        ENTRANCE,

        // 飼育員巡回用
        BELL_AREA,          // 鐘周辺
        POLAR_AREA,         // シロクマ周辺
        BEAR_AREA,          // クマ周辺
        PANDA_AREA_01,      // パンダ周辺
        PANDA_AREA_02,      // パンダ周辺
        HOURSE_AREA,        // ウマ
        ZEBRA_AREA_01,      // シマウマ周辺1
        ZEBRA_AREA_02,      // シマウマ周辺2
        FOUNTAIN_AREA,      // 噴水周辺
        LAKE_AREA,          // 池周辺






    }


    //----- イベント -----
    public enum eEvent {
        GUEST_ENTER,    // 客入場
        SOUND_BELL,     // 鐘
        GO_POLARBEAR,   // しろくま
        GO_HOURSE,      // うま
        OBJ_MEGAPHON,   // メガホン使え

        TIMEOUT_ALERT   // HURRYのUI 
    }

}
