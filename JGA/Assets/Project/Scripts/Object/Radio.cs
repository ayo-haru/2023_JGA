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

public class Radio : BaseObj
{
	//------ 追加(このオブジェクトのみで使用)変数 ------
	private bool isSwitch;                                      // スイッチフラグ(true → ON / false → OFF)
	private bool isOncePlaySound;                               // 複数回音を鳴らすのを回避するためのフラグ
	private bool isRadioSound;

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();

		objType = ObjType.HIT_HOLD;
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		//rayDistance = 0.8f;

		isSwitch = false;
		isOncePlaySound = false;
		isRadioSound = false;

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		if (PauseManager.isPaused) return;              // ポーズ中処理

		//CheckisGround();								// 地面との接触判定
		PlaySoundChecker();
	}
	

	//　当たり判定処理===============================================
	protected override void OnCollisionEnter(Collision collision)
	{
		// ポーズ処理
		if (PauseManager.isPaused) { return; }

		if (collision.gameObject.tag == "Ground") { PlayDrop(); }		// 地面と当たった時

	}

	protected override void OnTriggerStay(Collider other)
	{
		// ポーズ処理
		if (PauseManager.isPaused) { return; }

		// ペンギンをはたいたときの処理
		if (other.gameObject.tag == "Player" && player.IsHit){

			ToggleSwitch();				// スイッチの切り替え
			PlayRadioSound(isSwitch);	// スイッチの状態事の処理
		}

		// ペンギンが掴んだ時の処理
		if(other.gameObject.tag == "Player" && player.IsHold){
			if (!isOncePlaySound) {
				SoundManager.Play(audioSourcesList[0], SoundManager.ESE.PENGUIN_CATCH);
				isOncePlaySound = true;
			}
		}

		if (!player.IsHold){
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
		if (isSwitch) { SoundManager.Play(audioSourcesList[1], SoundManager.ESE.RADIO_ON); }
		else { SoundManager.Play(audioSourcesList[1], audioSourcesList[1].clip); }

	}

	/// <summary>
	/// スイッチのオンオフに応じての処理
	/// </summary>
	/// <param name="checkSwitch"></param>
	public void PlayRadioSound(bool checkSwitch)
	{
		if (checkSwitch) {
			StartCoroutine("PlayRadio");
		}
		else{audioSourcesList[1].Stop();}
	}

	/// <summary>
	/// 時間を開けてからホワイトノイズを再生するためのコルーチン
	/// </summary>
	/// <returns></returns>
	private IEnumerator PlayRadio()
	{
		yield return new WaitForSeconds(1.0f);

		// ポーズ処理
		if (!PauseManager.isPaused) {
			audioSourcesList[1].Play();
		}
	}

	public bool GetPlayRadio()
	{
		if (audioSourcesList[1].isPlaying)
		{
			isRadioSound = true;
		}
		else{
			isRadioSound = false;
		}

		return isRadioSound;
	}

}

