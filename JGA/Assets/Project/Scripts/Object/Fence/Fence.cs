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
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		Init();
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		rb.isKinematic = true;		// 柵ではrigidbodyを使用しない。

		// オブジェクトと当たった時の音を変更
		sounds[(int)SoundType.TOUCH] = (int)SoundManager.ESE.STEALFENCE;
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject){
			PlayHit(_audioSource, sounds[(int)SoundType.TOUCH]);
		}
	}


	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		CheckIsPlaySound();
	}
}
