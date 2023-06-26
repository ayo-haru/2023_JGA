//=============================================================================
// @File	: [Door.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/10	スクリプト作成
// 2023/06/26	OnEnabaleのオーバライド追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseObj
{
	[SerializeField] private Animator _animator;

	//protected override void OnEnable()
	//{
	//	/* Baseの処理にListに入れる処理があるので、このオブジェクトでは
	//	  リストに入れる処理をしたくないため、ここでオーバライドする。(吉原)
	//	   Fence同様
	//	*/
	//}

	/// <summary>
	/// Startで行いたい処理を記載
	/// </summary>
	public override void OnStart()

	{
		Init();

		_animator = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// Updateで行いたい処理を記載
	/// </summary>
	public override void OnUpdate()
	{

	}



	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);

		if (collision.gameObject.tag == "Player"){
			_animator.SetTrigger("Open");
		}
	}

}
