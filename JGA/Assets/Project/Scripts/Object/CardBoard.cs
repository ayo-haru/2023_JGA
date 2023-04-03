//=============================================================================
// @File	: [CardBoard.cs]
// @Brief	: 段ボールの処理を記載
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/30	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoard : BaseObject
{
	[SerializeField] private Animator _animator;

	private bool isPlay = false;


	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();
		_animator = GetComponent<Animator>();
		playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		objState = OBJState.HITANDHOLD;

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		// ポーズ処理
		if (PauseManager.isPaused) { return; }

		CheckIsPlaySound();
	}

	protected override void OnCollisionEnter(Collision collision)
	{

		// 段ボールが地面に当たった時の音を変更
		if(collision.gameObject.tag == "Ground"){
			PlayDrop(_audioSource,SoundManager.ESE.CARDBOARDBOX_002);
		}
		else{
			PlayHit();
		}
	}

    protected override void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.tag == "Player"){
				_animator.SetTrigger("Broken");

        }
    }

}
