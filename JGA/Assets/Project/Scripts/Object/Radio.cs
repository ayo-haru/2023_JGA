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
	private bool isSwitch = false;                          // スイッチフラグ(true → ON / false → OFF)
	private bool isOncePlaySound = false;                   // 複数回音を鳴らすのを回避するためのフラグ
	private bool isRadioSound = false;

	private GameObject effect;								// ガヤガヤエフェクト
	private GameObject effectPoint;                         // エフェクト再生位置



	/// <summary>
	/// Startで行いたい処理を記載
	/// </summary>
	public override void OnStart()
	{
		Init(ObjType.HIT_HOLD);

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		// 子にあるエフェクト再生オブジェクトを検索
		effectPoint = GameObject.Find("EffectPoint");
	}

	/// <summary>
	/// Updateで行いたい処理を記載
	/// </summary>
	public override void OnUpdate()
	{
		if (PauseManager.isPaused) return;              // ポーズ中処理

		PlaySoundChecker();
	}


#if false
	//　当たり判定処理===============================================
	protected override void OnCollisionEnter(Collision collision)
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null){
			Debug.LogError("<color=#fd7e00>GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズ中なら処理を行わない */
		if (PauseManager.isPaused) { return; }

		if (collision.gameObject.tag == "Ground") { PlayDrop(); }       // 地面と当たった時


	}

	protected override void OnTriggerStay(Collider other)
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null) {
			Debug.LogError("<color=#fd7e00>GimickObjectManagerがありません</color>");
			return; 
		}

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

#endif

	/// <summary>
	/// スイッチのトグル処理
	/// </summary>
	private void ToggleSwitch()
	{
		isSwitch = !isSwitch;

		// スイッチの切り替えと同時にそれぞれ音を鳴らす。
		if (isSwitch) { SoundManager.Play(audioSourcesList[0], SoundManager.ESE.RADIO_ON); }
		else { SoundManager.Play(audioSourcesList[0], SoundManager.ESE.RADIO_OFF); }

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
		else{
			audioSourcesList[1].Stop();
			Destroy(effect);
		}
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
			SetEffect();
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

	private void SetEffect()
	{
		if(effect != null) Destroy(effect);

		effect = EffectManager.Create(effectPoint.transform.position,6);
		effect.transform.parent = effectPoint.transform;
	}
}

