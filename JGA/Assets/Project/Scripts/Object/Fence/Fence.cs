//=============================================================================
// @File	: [Fence.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/04/03	スクリプト作成
// 2023/04/18	ポーズ解除変更
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

    protected override void Resumed() {
		/*
		 * フェンスはフェンス同士で当たり判定させないため常にisKinematicがtrue
		 * そのためほかのオブジェクトと違い、別でポーズ解除を用意
		 */
		rb.isKinematic = true;
    }
}
