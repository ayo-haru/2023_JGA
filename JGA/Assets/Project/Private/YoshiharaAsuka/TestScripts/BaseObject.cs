//=============================================================================
// @File	: [BaseObject.cs]
// @Brief	: オブジェクトベースの改訂版を作成してみる
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/01	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{

	//---- オブジェクトステート ----
	public enum OBJState
	{
		NONE = 0,			// 通常
		HIT = 1,            // 叩く
		HOLD = 2,           // つかむ
		HITANDHOLD = 3,     // 叩くしつかむ
		RETURN = 4,         // 元に戻す。
	}


	//---- 変数宣言 ----
	protected Rigidbody rb;						// リジッドボディ
	protected AudioSource _audioSource;			// オーディオソース
	protected int objState;                     // オブジェクトのステート

	protected bool isPlaySound;                 // サウンド再生中フラグ


	
	/// <summary>
	/// 初期化関数
	/// </summary>
	/// <returns></returns>
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
		objState = (int)OBJState.NONE;		// 初期値はNONE
	}


	protected virtual void OnCollisionEnter(Collision collision) { }
	protected virtual void OnCollisionStay(Collision collision) { }
	protected virtual void OnCollisionExit(Collision collision) { }


	protected virtual void OnTriggerEnter(Collider other) { }
	protected virtual void OnTriggerStay(Collider other) { }
	protected virtual void OnTriggerExit(Collider other) { }


	/// <summary>
	/// 音の再生中フラグの取得

	public bool GetIsPlaySound()
	{
		return isPlaySound;
	}



}
