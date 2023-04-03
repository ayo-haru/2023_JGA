//=============================================================================
// @File	: [Fence.cs]
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

public class Fence : BaseObject
{



	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();

		rb.isKinematic = true;		// 柵ではrigidbodyを使用しない。

	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject){
			PlayHit(_audioSource,SoundManager.ESE.STEALFENCE);
		}
	}


	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		CheckIsPlaySound();
	}

    protected override void Pause()
    {
		// 物理挙動停止
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngleVelocity;

	}
}
