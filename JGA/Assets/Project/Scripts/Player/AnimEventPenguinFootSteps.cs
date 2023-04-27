//=============================================================================
// @File	: [AnimEventPenguinFootSteps.cs]
// @Brief	: ペンギンの足音を再生するアニメーションイベント
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventPenguinFootSteps : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audioSource;

	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public void PlayFootStepsSound()
	{
		SoundManager.Play(_audioSource, SoundManager.ESE.PENGUIN_WALK_002);
	}

}
