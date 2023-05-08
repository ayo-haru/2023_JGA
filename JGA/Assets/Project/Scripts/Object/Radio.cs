//=============================================================================
// @File	: [Radio.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/03	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : BaseObject
{

	[SerializeField] private AudioSource radioAudioSource;      // ラジオ音を再生するため追加で用意
	private bool isSwitch;                                      // スイッチフラグ(true → ON / false → OFF)
	private bool isOncePlaySound;								// 複数回音を鳴らすのを回避するためのフラグ

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();

		objState = OBJState.HITANDHOLD;
		playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		rayDistance = 0.8f;

		isSwitch = false;
		isOncePlaySound = false;

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		if (PauseManager.isPaused) return;      // ポーズ中処理
		Debug.Log(radioAudioSource);

		CheckisGround();                        // 地面との接触判定
		CheckIsPlaySound(radioAudioSource.isPlaying);     // 再生中判定
	}
	

	//　当たり判定処理===============================================
	protected override void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ground"　&& isGround == true) { PlayDrop(); }		// 地面と当たった時

	}

	protected override void OnTriggerStay(Collider other)
	{
		// ペンギンをはたいたときの処理
		if(other.gameObject.tag == "Player" && playerRef.IsHit){

			ToggleSwitch();				// スイッチの切り替え
			PlayRadioSound(isSwitch);	// スイッチの状態事の処理
		}

		// ペンギンが掴んだ時の処理
		if(other.gameObject.tag == "Player" && playerRef.IsHold){
			if (!isOncePlaySound) { 
				SoundManager.Play(_audioSource, SoundManager.ESE.PENGUIN_CATCH);
				isOncePlaySound = true;
			}
		}

		if (!playerRef.IsHold){
			isOncePlaySound = false;
		}
	}
	//========================================================

	/// <summary>
	/// スイッチのトグル処理
	/// </summary>
	private void ToggleSwitch()
	{
		isSwitch = !isSwitch;

		// スイッチの切り替えと同時にそれぞれ音を鳴らす。
		if (isSwitch) { SoundManager.Play(_audioSource, SoundManager.ESE.RADIO_ON); }
		else { SoundManager.Play(_audioSource, SoundManager.ESE.RADIO_OFF); }

	}

	/// <summary>
	/// スイッチのオンオフに応じての処理
	/// </summary>
	/// <param name="checkSwitch"></param>
	private void PlayRadioSound(bool checkSwitch)
	{
		if (checkSwitch) {
			StartCoroutine("PlayRadio");

		}
		else{radioAudioSource.Stop();}
	}

	/// <summary>
	/// 時間を開けてからホワイトノイズを再生するためのコルーチン
	/// </summary>
	/// <returns></returns>
	private IEnumerator PlayRadio()
	{
		yield return new WaitForSeconds(1.5f);
		radioAudioSource.Play();

	}

}

