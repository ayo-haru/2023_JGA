//=============================================================================
// @File	: [MySceneManager]
// @Brief	: オリジナルのシーンマネージャー
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail  : 
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySceneManager : SingletonMonoBehaviour<MySceneManager> {
    protected override bool dontDestroyOnLoad { get { return true; } }


    public static class GameData {
        public static PrefabContainer characterDatas;
        public static bool isCatchPenguin;
    }




    private void Awake() {
        Application.targetFrameRate = 60;       // FPSを60に固定
        GameData.characterDatas = AddressableLoader<PrefabContainer>.Load("CharacterData");

        GameData.isCatchPenguin = false;
    }

    private void Start() {
    }

}
