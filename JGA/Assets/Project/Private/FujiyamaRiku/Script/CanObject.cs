//=============================================================================
// @File	: [CanObject.cs]
// @Brief	: 缶の挙動
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/28	スクリプト作成
// 2023/03/28	音の実装
// 2023/04/02   音が多数でなってしまっていたのの解消、ポーズの処理の書き込み
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UniRx;


public class CanObject : BaseObj , IObjectSound
{
	private bool fallFlg;
	private bool flyFlg;



	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		Init();
		objType = ObjType.HIT_HOLD;

		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		if (player == null)
		{
			player = GameObject.FindWithTag("Player").GetComponent<Player>();
		}
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
		if(audioSource.isPlaying)
		{
			isPlaySound = true;
		}
		else
		{
			isPlaySound = false;
		}

	}
	private void OnCollisionEnter(Collision collison)
	{
		if (collison.gameObject.tag == "Player"  && !fallFlg)
		{
			Debug.Log("蹴ったとき時の音");
			SoundManager.Play(audioSource, SoundManager.ESE.CAN_ROLL);
		}
		if(collison.gameObject.tag == "Ground")
		{
			if (fallFlg)
			{
				PlayRelease();
				fallFlg = false;
			}
			if(flyFlg)
			{
				Debug.Log("転がる音再生");
				SoundManager.Play(audioSource, SoundManager.ESE.CAN_ROLL);
				flyFlg = false;
			}
		}
	}



	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if(player.IsHold && !fallFlg)
			{
				PlayPickUp();
				fallFlg = true;
			}
			
		}
	}
	private void OnTriggerStay(Collider other)
	{
		if (player.IsHit && other.tag == "Player")
		{
			Debug.Log("はたいて当たった時の音再生");
			SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
			flyFlg = true;
		}
	}

	public void PlayPickUp()
	{
		Debug.Log("つかんだ音の再生");
		SoundManager.Play(audioSource, SoundManager.ESE.CAN_CATCH);
	}

	public void PlayHold()
	{

	}

	public void PlayRelease()
	{
		Debug.Log("離した音再生");
		SoundManager.Play(audioSource, SoundManager.ESE.CAN_RELEASE);
	}

	private void Pause()
	{
		audioSource.Pause();
		
	}

	private void Resumed()
	{
		audioSource.Play();
	}

}
