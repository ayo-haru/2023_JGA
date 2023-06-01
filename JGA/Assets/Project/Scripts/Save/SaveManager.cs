//=============================================================================
// @File	: [SaveManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/22	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager {

    public static void SaveAll() {
        SaveSystem.SaveGuestCnt(GameData.guestCnt);
        SaveSystem.SaveTimer(GameData.timer);
        SaveSystem.SaveLastStageNum(GameData.nowScene);
        SaveSystem.SaveLastPlayerPos(GameData.playerPos);
    }

    public static void SaveInitDataAll() {
        SaveSystem.SaveGuestCnt(0);
        SaveSystem.SaveTimer(0.0f);
        SaveSystem.SaveLastStageNum(GameData.nowScene);
        SaveSystem.SaveLastPlayerPos(Vector3.zero);
    }

    public static void SaveGuestCnt(int _guestCnt) {
        SaveSystem.SaveGuestCnt(_guestCnt);
    }
    public static void SaveTimer(float _timer) {
        SaveSystem.SaveTimer(_timer);
    }
    public static void SaveLastStageNum(int _nowScene) {
        SaveSystem.SaveLastStageNum(_nowScene);
    }
    public static void SaveLastPlayerPos(Vector3 _playerPos) {
        SaveSystem.SaveLastPlayerPos(_playerPos);
    }

    public static void LoadAll() {
        GameData.guestCnt = SaveSystem.sd.guestCnt;
        GameData.timer = SaveSystem.sd.timer;
        GameData.nowScene = SaveSystem.sd.lastStageNum;
        GameData.playerPos = SaveSystem.sd.lastPlayerPos;
    }
}
