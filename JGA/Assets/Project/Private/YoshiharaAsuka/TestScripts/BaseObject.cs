//=============================================================================
// @File	: [BaseObject.cs]
// @Brief	: オブジェクトベースの改訂版を作成してみる
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/01	スクリプト作成
// 2023/04/02	衝突時の判定で音を鳴らすvirtual関数の作成。
//				音の登録をint型の配列で用意
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public abstract class BaseObject : MonoBehaviour,IPlayObjectSound
{

	//----- オブジェクトステート -----
	public enum OBJState
	{
		NONE = 0,			// 通常
		HIT = 1,            // 叩く
		HOLD = 2,           // つかむ
		HITANDHOLD = 3,     // 叩くしつかむ
		RETURN = 4,         // 元に戻す。
	}

	// ----- SEの鳴らす音の種類 -----
	public enum SoundType
	{ 
		TOUCH	= 0,		// 触れた時
		HIT		= 1,		// 叩くとき
		DROP	= 2,		// 落とした時
	}


	
	//---- 変数宣言 ----
	protected Rigidbody rb;						// リジッドボディ
	protected AudioSource _audioSource;			// オーディオソース
	protected int[] sounds;						// 鳴らす音の登録
	protected int objState;                     // オブジェクトのステート

	protected bool isPlaySound;                 // サウンド再生中フラグ


	
	/// <summary>
	/// 初期化関数
	/// </summary>
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();

		// 音のデフォルトの登録をここで行う。
		// 継承先でそれぞれの音に変更する
		sounds = new int[]
		{
			(int)SoundManager.ESE.OBJECT_HIT,		// 初期値はOBJECT_HITを登録
			(int)SoundManager.ESE.OBJECT_HIT,		// 初期値はOBJECT_HITを登録
			(int)SoundManager.ESE.OBJECT_DROP,		// 初期値はOBJECT_DROPを登録
		};
		objState = (int)OBJState.NONE;				// 初期値はNONE
		isPlaySound = false;
	}




	protected virtual void OnCollisionEnter(Collision collision) 
	{
		PlayHit(_audioSource, sounds[(int)SoundType.TOUCH]);
	}

	protected virtual void OnCollisionStay(Collision collision) { }
	protected virtual void OnCollisionExit(Collision collision) { }


	protected virtual void OnTriggerEnter(Collider other) { }
	protected virtual void OnTriggerStay(Collider other) { }
	protected virtual void OnTriggerExit(Collider other) { }



	public bool GetIsPlaySound()
	{
		return isPlaySound;
	}

	/// <summary>
	/// 再生フラグの切り替え
	/// </summary>
	protected bool CheckIsPlaySound()
    {
        if (_audioSource.isPlaying){
			return isPlaySound = true;
        }
        else{
			return isPlaySound = false;
        }
    }




	// ===================== インターフェースメソッド =========================
	public void PlayHit(AudioSource audioSource, int soundNumber)
	{
		SoundManager.Play(audioSource,(SoundManager.ESE)soundNumber);
	}

	public void PlayPick(AudioSource audioSource, int soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}
	//=====================================================================
}
