//=============================================================================
// @File	: [Fence.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/04/03	スクリプト作成
// 2023/04/18	ポーズ解除変更
// 2023/06/26	OnEnabaleのオーバライド追加
// 2023/07/18	BaseObjの呪縛を解放
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]

public class Fence:MonoBehaviour,IPlayObjectSound
{

	protected Rigidbody rb = null;                              // リジッドボディ使用

	// オーディオソースをリストで格納
	[SerializeField]
	protected List<AudioSource> audioSourcesList = new List<AudioSource>();

	protected Player player = null;                             // プレイヤー取得
	protected bool isPlaySound = false;                         // 音が鳴っているか

	//--- ポーズ用変数 ---
	protected Vector3 pauseVelocity = Vector3.zero;
	protected Vector3 pauseAngleVelocity = Vector3.zero;
	//------------------

	private void Awake()
	{
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

	}

	//protected override void OnEnable()
	//{
	//	/* Baseの処理にListに入れる処理があるので、このオブジェクトでは
	//	  リストに入れる処理をしたくないため、ここでオーバライドする。(吉原)
	//	*/
	//}

	/// <summary>
	/// Startで行いたい処理を記載
	/// </summary>
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		GetComponents<AudioSource>(audioSourcesList);
		isPlaySound = false;

		rb.isKinematic = true;      // 柵ではrigidbodyを使用しない。
	}


	/// <summary>
	/// Updateで行いたい処理を記載
	/// </summary>
	private void Update()
	{
		PlaySoundChecker();
	}

	// 当たり判定の処理==========================================================
	public  void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject){
			PlayHit(audioSourcesList[0],SoundManager.ESE.STEALFENCE);
		}
	}
	//=======================================================================

	/// <summary>
	/// 引数無しの場合はリストの一番最初にあるオーディオソースを判定
	/// </summary>
	protected void PlaySoundChecker()
	{
		if (audioSourcesList[0].isPlaying) isPlaySound = true;
		else isPlaySound = false;
	}


	private void Pause() 
	{
		/*
		  全てのオーディオソースを止める
		 */
		for (int i = 0; i < audioSourcesList.Count; i++)
		{
			audioSourcesList[i].Pause();
		}

		pauseVelocity = rb.velocity;
		pauseAngleVelocity = rb.angularVelocity;
	}

	protected void Resumed() 
	{
		/*
		 * フェンスはフェンス同士で当たり判定させないため常にisKinematicがtrue
		 * そのためほかのオブジェクトと違い、別でポーズ解除を用意
		 */
		rb.isKinematic = true;
		audioSourcesList[0].UnPause();
	}

	// ===================== インターフェースメソッド =========================
	public void PlayHit()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.OBJECT_HIT);
	}

	public void PlayHit(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}

	public void PlayDrop()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.OBJECT_DROP);
	}

	public void PlayDrop(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}
	//=====================================================================
}
