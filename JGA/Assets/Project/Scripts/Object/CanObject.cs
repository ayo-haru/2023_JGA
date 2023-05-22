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

	protected override void Awake()
	{
		Init();
		objType = ObjType.HIT_HOLD;
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	private void OnWillRenderObject()
	{
		Debug.Log("映った");
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
		PlaySoundChecker(1);
	}
	protected override void OnCollisionEnter(Collision collison)
	{

		// ポーズ処理
		if (PauseManager.isPaused) { return; }

		if (collison.gameObject.tag == "Player"  && !fallFlg)
		{
			SoundManager.Play(audioSourcesList[1], SoundManager.ESE.CAN_ROLL);
		}
		
		if (collison.gameObject.tag == "Ground")
		{
			
			if (fallFlg)
			{
				PlayRelease();
				fallFlg = false;
			}
			if(flyFlg)
			{
				SoundManager.Play(audioSourcesList[1], SoundManager.ESE.CAN_ROLL);
				flyFlg = false;
			}
		}
	}

	protected override void OnTriggerStay(Collider other)
	{
		// ポーズ処理
		if (PauseManager.isPaused) { return; }

		if (player.IsHit && other.tag == "Player")
		{
			SoundManager.Play(audioSourcesList[0], SoundManager.ESE.OBJECT_HIT);
			flyFlg = true;
		}

		if (other.tag == "Player")
		{
			if (player.IsHold && !fallFlg)
			{
				PlayPickUp();
				fallFlg = true;
			}

		}

	}

	public void PlayPickUp()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.CAN_CATCH);
	}

	public void PlayHold()
	{

	}

	public void PlayRelease()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.CAN_RELEASE);
	}


}
