//=============================================================================
// @File	: [IPlayObjectSound.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/01	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayObjectSound
{
    void PlayHit(AudioSource audioSource,int soundNumber);
    void PlayPick(AudioSource audioSource,int soundNumber);

}
