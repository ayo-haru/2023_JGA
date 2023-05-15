//=============================================================================
// @File	: [BaseObj.cs]
// @Brief	: ギミックオブジェクトのベース
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/29	スクリプト作成
// 2023/04/04   OutLineスクリプトを追加するようにattributeつけました(吉原)
// 2023/04/18   ポーズ処理変更
// 2023/05/12   改良しようとしている。
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]
public class BaseObj : MonoBehaviour, IPlayObjectSound
{
	//----- オブジェクトのステート -----
	public enum ObjType
	{
		NONE		= 0,    // 通常
		HIT			= 1,    // 叩く
		HOLD		= 2,    // 掴む
		DRAG		= 3,    // ひきずる 
		HIT_HOLD	= 4,    // 叩く&掴む
		HIT_DRAG	= 5,    // 叩く&ひきずる
		RETURN		= 6,    // 元に戻す(飼育員)
	}

	//-----オブジェクトとの距離別のステート -----
	public enum ObjDistanceType
	{
		NONE	= 0,		// 測定不能
		NEAR,				// 近い(真横)
		MID,				// 普通(カメラ範囲に収まる範囲)
		FAR,				// 遠い(カメラに見えない)
	}


	//---共通変数宣言---
	protected Rigidbody rb;						// リジッドボディ使用
	protected AudioSource audioSource;		// オーディオソース
	protected Player player;					// プレイヤー取得

	public ObjType objType;						// オブジェクトのタイプ
	public ObjDistanceType　objDistanceType;		// 距離のタイプ
	protected bool isPlaySound;					// 音が鳴っているか
	protected float distance;					// オブジェクトとの距離		
	//-------------

	// 地面との接触判定に使用する変数

	//

	//--- ポーズ用変数 ---
	protected Vector3 pauseVelocity = Vector3.zero;
	protected Vector3 pauseAngleVelocity = Vector3.zero;
	//-----------------

	protected virtual void Awake()
	{
		/*
		 このクラスを継承した物は必ず、ポーズの処理を登録を行いため、ここに記載
		*/
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

	}


	protected virtual void OnCollisionEnter(Collision collision) { }
	protected virtual void OnCollisionStay(Collision collision) { }
	protected virtual void OnCollisionExit(Collision collision) { }

	protected virtual void OnTriggerEnter(Collider other) { }
	protected virtual void OnTriggerStay(Collider other) { }
	protected virtual void OnTriggerExit(Collider other) { }



	protected void Init()
	{
		rb  = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

		objType = ObjType.NONE;
		isPlaySound = false;
		distance = -1;

	}

	protected void PlaySoundChecker()
	{
		if (audioSource.isPlaying)
		{
			isPlaySound = true;
		}
		else
		{
			isPlaySound = false; 
		}
	}

	/// <summary>
	/// 音が鳴ってるかフラグの取得
	/// </summary>
	/// <returns></returns>
	public bool GetisPlaySound()
	{
		return isPlaySound;
	}



	/// <summary>
	/// 自分とオブジェクトの距離を計算し、返す処理
	/// </summary>
	protected int CalculateDistance(GameObject gameObject)
	{
		// オブジェクトが取得できない場合は負の値(測定不能とする)を返す。
		if (gameObject == null) return -1;


		// 自分と相手がどれだけ離れているか = 指定のオブジェクト(引数に代入) - 自分のポジション
		distance = Vector3.Distance(gameObject.transform.position, this.gameObject.transform.position);

		// 小数点以下はいらないので、無理やりキャストしちゃいます。
		return (int)distance;
	}

	/// <summary>
	/// distanceの値に応じて距離タイプの列挙を定義
	/// </summary>
	/// <returns></returns>
	protected ObjDistanceType GetDistanceType()
	{
		/*
		 * 数字は適当です
		 */
		if (distance < 0) objDistanceType = ObjDistanceType.NONE;
		if (50 < distance) objDistanceType = ObjDistanceType.NEAR;
		if (150 < distance) objDistanceType = ObjDistanceType.FAR;


		return objDistanceType;

	}
	/// <summary>
	/// ポーズの処理
	/// </summary>
	protected virtual void Pause()
	{
		audioSource.Pause();


		pauseVelocity = rb.velocity;
		pauseAngleVelocity = rb.angularVelocity;
		rb.isKinematic = true;
	}

	/// <summary>
	/// ポーズ解除の処理
	/// </summary>
	protected virtual void Resumed()
	{
		//audioSource.Play();		←これのせいでポーズ解除後音が一回なるのでは？？
		//							 コメントアウト(2023.05.12吉原)
		// 物理挙動開始
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngleVelocity;
		rb.isKinematic = false;
	}


	// ===================== インターフェースメソッド =========================
	public void PlayHit()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
	}

	public void PlayHit(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}

	public void PlayDrop()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_DROP);
	}

	public void PlayDrop(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}
	//=====================================================================


}
