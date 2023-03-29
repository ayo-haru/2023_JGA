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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CanObject : MonoBehaviour , IObjectSound
{
	private Rigidbody rb;
	private Player player;
	private Collider playerCollision;
	private AudioSource audioSource;
	private bool test;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		audioSource = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();	
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
		if(playerCollision == null)
		{
			playerCollision = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();

        }
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		if (player.IsInteract)
		{
			PlayPickUp();
		}
		if(player.IsHold)
		{
			PlayHold();
		}
		//if(!rb.IsSleeping())
		//{
		//	if (!test)
		//	{
  //              audioSource.loop = true;
  //              SoundManager.Play(audioSource, SoundManager.ESE.CAN_ROLL);
		//		test = true;
		//	}
  //      }
		//else
		//{
  //          test = false;
  //          audioSource.loop = false;
  //      }
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
    private void OnCollisionEnter(Collision collison)
    {
		if (collison.collider == playerCollision && !player.IsHold)
		{
			SoundManager.Play(audioSource, SoundManager.ESE.CAN_ROLL);
		}
	}

    public void PlayPickUp()
    {
		
		SoundManager.Play(audioSource, SoundManager.ESE.CAN_CATCH);
		
    }

    public void PlayHold()
    {
        //SoundManager.Play(audioSource, SoundManager.ESE.CAN_CATCH);
    }

    public void PlayRelease()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.CAN_RELEASE);
    }
}
