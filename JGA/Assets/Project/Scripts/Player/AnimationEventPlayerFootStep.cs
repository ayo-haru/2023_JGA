//=============================================================================
// @File	: [AnimationEventPlayerFootsStep.cs]
// @Brief	: アニメーションイベントに登録して足音を再生するスクリプト
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventPlayerFootStep : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void PlayFootStep()
    {
        SoundManager.Play(audioSource,SoundManager.ESE.HUMAN_WALK_002);
    }

}
