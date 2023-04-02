//=============================================================================
// @File	: [Radio.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/03	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : BaseObject
{

    [SerializeField] private AudioSource radioAudioSource;      // ラジオ音を再生するため追加で用意
    private bool isSwitchON;                                    // スイッチフラグ


    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        Init();
        objState = OBJState.HITANDHOLD;
        radioAudioSource = GetComponent<AudioSource>();
        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        isSwitchON = false;
    }

    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
    {
        // 再生中判定
        CheckIsPlaySound();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayHit();
            ToggleSwitch();
        }
    }

    /// <summary>
    /// ラジオの再生
    /// </summary>
    private void PlayRadioSound()
    {
        // スイッチがオンの時のみ再生
        if (isSwitchON)
        {
            SoundManager.Play(radioAudioSource, SoundManager.ESE.RADIO_PLAY);
        }
    }

    private void ToggleSwitch()
    {
        isSwitchON = !isSwitchON;
    }
}
