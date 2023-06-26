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
// 2023/06/26	OnEnabaleのオーバライド追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : BaseObj
{

	//protected override void OnEnable()
	//{
	//	/* Baseの処理にListに入れる処理があるので、このオブジェクトでは
	//	  リストに入れる処理をしたくないため、ここでオーバライドする。(吉原)
	//	*/
	//}

	/// <summary>
	/// Startで行いたい処理を記載
	/// </summary>
	public override void OnStart()
	{
		Init();

		rb.isKinematic = true;      // 柵ではrigidbodyを使用しない。
	}


	/// <summary>
	/// Updateで行いたい処理を記載
	/// </summary>
	public override void OnUpdate()
	{
		PlaySoundChecker();
	}

	// 当たり判定の処理==========================================================
	protected override void OnCollisionEnter(Collision collision)
	{
		/* 
		 * BaseObjの方にエラーチェックの処理を記載しているため、必ずbaseの処理を最初に記載すること!(吉原)
		*/
		base.OnCollisionEnter(collision);

		if (collision.gameObject){
			PlayHit(audioSourcesList[0],SoundManager.ESE.STEALFENCE);
		}
	}
	//=======================================================================



	protected override void Pause() {
		base.Pause();
		audioSourcesList[0].Pause();
	}

	protected override void Resumed() {
		/*
		 * フェンスはフェンス同士で当たり判定させないため常にisKinematicがtrue
		 * そのためほかのオブジェクトと違い、別でポーズ解除を用意
		 */
		rb.isKinematic = true;
		audioSourcesList[0].UnPause();
	}
}
