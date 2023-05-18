//=============================================================================
// @File	: [IObjectSound.cs]
// @Brief	: オブジェクトの音のインターフェース
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/28	スクリプト作成
// 2023/03/28   どのタイミングで再生するかを作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectSound
{
    void PlayPickUp();
    void PlayHold();
    void PlayRelease();
}
