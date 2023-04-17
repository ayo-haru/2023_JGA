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
// 2023/04/03	ポーズの処理追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]
public abstract class BaseObject : MonoBehaviour,IPlayObjectSound
{

	//----- オブジェクトステート -----
	public enum OBJState
	{
		NONE		= 0,	// 通常
		HIT			= 1,	// 叩く
		HOLD		= 2,	// つかむ
		HITANDHOLD	= 3,	// 叩くしつかむ
		RETURN		= 4,	// 元に戻す。
	}


	//---- 変数宣言 ----
	protected Rigidbody rb;						// リジッドボディ
	protected AudioSource _audioSource;			// オーディオソース
	protected Player playerRef;					// プレイヤースクリプト取得

	public OBJState objState;					// オブジェクトのステート

	protected bool isPlaySound;                 // サウンド再生中フラグ

	//----- ポーズ用変数 ----
	protected Vector3 pauseVelocity = Vector3.zero;
	protected Vector3 pauseAngleVelocity = Vector3.zero;
	//---------------------

	protected virtual void Awake()
	{
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);
	}


	/// <summary>
	/// 初期化関数
	/// </summary>
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
		playerRef = null;

		objState = OBJState.NONE;				// 初期値はNONE
		isPlaySound = false;

	}

	protected virtual void OnCollisionEnter(Collision collision) 
	{
		// 地面と当たった時の音
		if(collision.gameObject.tag == "Ground"){
			PlayHit();
		}

		// それ以外のオブジェクトと当たった時
		else{
			PlayHit();
		}
	}
	protected virtual void OnCollisionStay(Collision collision) { }
	protected virtual void OnCollisionExit(Collision collision) { }


	protected virtual void OnTriggerEnter(Collider other) {}
	protected virtual void OnTriggerStay(Collider other) { }
	protected virtual void OnTriggerExit(Collider other) { }

	/// <summary>
	/// isPlaySoundフラグの取得
	/// </summary>
	/// <returns></returns>

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

	/// <summary>
	/// ポーズの処理
	/// </summary>
	protected virtual void Pause()
	{
		// 物理挙動停止
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngleVelocity;
		rb.isKinematic = false;
	}

	protected virtual void Resumed()
	{
		// 物理挙動開始
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngleVelocity;
		rb.isKinematic = true;

	}

	// ===================== インターフェースメソッド =========================
	public void PlayHit()
	{
		SoundManager.Play(_audioSource,SoundManager.ESE.OBJECT_HIT);
	}
	public void PlayHit(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}

	public void PlayDrop()
	{
		SoundManager.Play(_audioSource, SoundManager.ESE.OBJECT_DROP);
	}

	public void PlayDrop(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}
	//=====================================================================
}
