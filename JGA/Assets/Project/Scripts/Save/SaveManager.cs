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
        SaveSystem.SaveGuestCnt(MySceneManager.GameData.guestCnt);
        SaveSystem.SaveTimer(MySceneManager.GameData.timer);
        SaveSystem.SaveLastStageNum(MySceneManager.GameData.nowScene);
        SaveSystem.SaveLastPlayerPos(MySceneManager.GameData.playerPos);
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
        MySceneManager.GameData.guestCnt = SaveSystem.sd.guestCnt;
        MySceneManager.GameData.timer = SaveSystem.sd.timer;
        MySceneManager.GameData.nowScene = SaveSystem.sd.lastStageNum;
        MySceneManager.GameData.playerPos = SaveSystem.sd.lastPlayerPos;
    }
}
