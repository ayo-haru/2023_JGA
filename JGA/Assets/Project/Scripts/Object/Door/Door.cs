//=============================================================================
// @File	: [Door.cs]
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

public class Door : BaseObject
{
    [SerializeField] private Animator _animator;

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();

		_animator = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		// ポーズ中は処理しない
		if (PauseManager.isPaused){
			return;
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player"){
			_animator.SetTrigger("Open");
		}
	}

}
