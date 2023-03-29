//=============================================================================
// @File	: [AIMoveSound.cs]
// @Brief	: 音
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/29	スクリプト作成
// 2023/03/29	歩く時の足音追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveSound : MonoBehaviour
{
    private AudioSource audioSource;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
    }

    /// <summary>
    /// 歩く時の足音
    /// </summary>
    public void WalkSound()
    {
        // 足音を鳴らす
        SoundManager.Play(audioSource, SoundManager.ESE.HUMAN_WALK_002);
    }

}
