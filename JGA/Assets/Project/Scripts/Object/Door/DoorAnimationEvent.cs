//=============================================================================
// @File	: [DoorAnimationEvent.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/10	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationEvent : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        // 親の持っているAudioSourceを参照
        _audioSource = GetComponentInParent<AudioSource>();
    }

    public void DoorOpenSound()
    {
        SoundManager.Play(_audioSource,SoundManager.ESE.STEALDOOR_OPEN);
    }

    public void DoorCLoseSound()
    {
        SoundManager.Play(_audioSource, SoundManager.ESE.STEALDOOR_CLOSE);

    }
}
