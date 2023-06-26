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

    public override void OnStart()
	{
		Init(ObjType.HIT_HOLD);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public override void OnUpdate()
    {
        if (PauseManager.isPaused) return;              // ポーズ中処理

        PlaySoundChecker();
    }

    private void OnWillRenderObject()
	{
		Debug.Log("映った");
	}
	
	protected override void OnCollisionEnter(Collision collision)
	{
        base.OnCollisionEnter(collision);
        // ポーズ処理
        if (PauseManager.isPaused) { return; }

		if (collision.gameObject.tag == "Player"  && !fallFlg)
		{
			SoundManager.Play(audioSourcesList[1], SoundManager.ESE.CAN_ROLL);
		}
		
		if (collision.gameObject.tag == "Ground")
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
        base.OnTriggerStay(other);
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
