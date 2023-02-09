//=============================================================================
// @File	: [MySceneManager]
// @Brief	: オリジナルのシーンマネージャー
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail  : 
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySceneManager : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;       // FPSを60に固定
    }

}
