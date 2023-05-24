//=============================================================================
// @File	: [SoundManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/16	スクリプト作成
//=============================================================================
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	public enum EBGM
	{
		TITLE_001,
		GAME001,
		GAMECLEAR,
		GAMEOVER,

	}
	public enum ESE
	{
		//　※追加、削除するときはSEData.assetとのリストの数、順番を完全一致させること！
		// ○○関連に合わせて追加するのでリストの順番も同様に！

		// ペンギン関連-------
		PENGUIN_VOICE,
		PENGUIN_MEGAVOICE,
		PENGUIN_CATCH,
		PENGUIN_HIT_001,
		PENGUIN_WALK_001,
		PENGUIN_WALK_002,
		//PENGUIN_RUN_001,
		//------------------

		// 動物関連---------
		// ウマ
		HOURCE_WALK_001,
		HOURCE_RUN_001,

		//-----------------

		// ギミック関連------

		// 共通
		OBJECT_HIT,
		OBJECT_DROP,

		// 段ボール
		CARDBOARDBOX_001,
		CARDBOARDBOX_002,

		// ドア
		DOOR_CLOSE,
		DOOR_OPEN,
		STEALDOOR_OPEN,
		STEALDOOR_CLOSE,

		// 水しぶき
		INTO_WATER,

		// 空き缶
		CAN_CATCH,              // つかむとき
		CAN_RELEASE,            // 叩いた時＆離して地面についた時
		CAN_ROLL,               // 地面にある時に転がる音1
		CAN_ROLLING,            // 地面にある時に転がる音2

		// ラジオ
		RADIO_ON,				// ラジオつかむとき
		RADIO_PLAY,             // ラジオプレイ
		RADIO_OFF,				// ラジオ離した時


		// フェンス
		STEALFENCE,             // 鉄柵の音

		// プラスチックが落ちる音
		PLASTIC_FALL,

		// ゴミ箱音
		TRASHBOX_GASAGASA,

		//----------------

		// 人関連----------
		HUMAN_WALK_001,
		HUMAN_WALK_002,
		HUMAN_WALK_003,
		HUMAN_RUN_001,
		HUMAN_RUN_002,
		HUMAN_RUN_003,
		//----------------


		// システム音-------
		SELECT_001,             // 選択中
		DECISION_001,           // 決定
		CANCEL_001,             // 戻る
		SLIDE_001,              // スライド
		COUNTDOWN_001,          // カウントダウン
								//---------------

	}

	private static HashSet<AudioSource> Source = new HashSet<AudioSource>();
	private static AudioClip BGM;
	private static List<AudioClip> SEs = new List<AudioClip>();
	private static GameVolume.Volume Volume;

	private void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		SceneManager.activeSceneChanged += ActiveSceneChanged;
	}

	private void Start()
	{
		Volume = SoundVolumeManager.GetVolume();
	}


	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="clip">音</param>
	public static void Play( AudioSource audioSource, AudioClip clip)
	{
		if (audioSource == null || clip == null)
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		Source.Add(audioSource);
		var _SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < _SEs.Length; i++)
		{
			if (_SEs[i].clip == clip)
			{
				SEs.Add(_SEs[i].clip);
				audioSource.volume = _SEs[i].volume * Volume.fSE;
				audioSource.PlayOneShot(_SEs[i].clip);
				return;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip == clip)
			{
				BGM = BGMs[i].clip;
				audioSource.clip = BGMs[i].clip;
				audioSource.volume = BGMs[i].volume * Volume.fBGM;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color>AudioSource:[{audioSource}], AudioClip:[{clip}]\n");
	}

	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="id">ID</param>
	public static void Play( AudioSource audioSource, EBGM eBGM)
	{
		if (audioSource == null)
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		Source.Add(audioSource);

		if (MySceneManager.Sound.BGMDatas.list.Length <= (int)eBGM ||
			MySceneManager.Sound.BGMDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eBGM})");
			return;
		}

		SoundData.Sound _BGM = MySceneManager.Sound.BGMDatas.list[((int)eBGM)];
		BGM = _BGM.clip;
		audioSource.clip = _BGM.clip;
		audioSource.volume = _BGM.volume * Volume.fBGM;
		audioSource.Play();
	}

	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="id">ID</param>
	public static void Play( AudioSource audioSource, ESE eSE)
	{
		if (audioSource == null)
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		Source.Add(audioSource);

		if (MySceneManager.Sound.SEDatas.list.Length <= (int)eSE ||
			MySceneManager.Sound.SEDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eSE})");
			return;
		}

		SoundData.Sound _SE = MySceneManager.Sound.SEDatas.list[((int)eSE)];
		SEs.Add(_SE.clip);
		audioSource.volume = _SE.volume * Volume.fSE;
		audioSource.PlayOneShot(_SE.clip);
	}

	/// <summary>
	/// 名前から再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="name">音源ファイル名</param>
	public static void Play( AudioSource audioSource, string name)
	{
		if (audioSource == null || string.IsNullOrEmpty(name))
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		Source.Add(audioSource);
		var _SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < _SEs.Length; i++)
		{
			if (_SEs[i].clip.name == name)
			{
				SEs.Add(_SEs[i].clip);
				audioSource.volume = _SEs[i].volume * Volume.fSE;
				audioSource.PlayOneShot(_SEs[i].clip);
				return;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip.name == name)
			{
				BGM = BGMs[i].clip;
				audioSource.clip = BGMs[i].clip;
				audioSource.volume = BGMs[i].volume * Volume.fBGM;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError("<color=red>指定されたオブジェクトが見つかりません</color>(SoundManager.Play)\n");
	}



	/// <summary>
	/// 音量を取得
	/// </summary>
	/// <param name="clip">音</param>
	/// <returns><strong>成功時</strong> 0.0f ~ 1.0fの値<br/><strong>失敗時</strong> -1.0f</returns>
	public static float GetVolume(AudioClip clip)
	{
		if (clip == null)
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		var _SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < _SEs.Length; i++)
		{
			if (_SEs[i].clip == clip)
			{
				return _SEs[i].volume;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip == clip)
			{
				return BGMs[i].volume;
			}
		}

		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color> AudioClip:[{clip}]\n");
		return -1f;
	}

	/// <summary>
	/// 音量を取得
	/// </summary>
	/// <param name="clip">音</param>
	/// <returns><strong>成功時</strong> 0.0f ~ 1.0fの値<br/><strong>失敗時</strong> -1.0f</returns>
	public static float GetVolume(EBGM eBGM)
	{
		if (MySceneManager.Sound.BGMDatas.list.Length <= (int)eBGM ||
			MySceneManager.Sound.BGMDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eBGM})");
			return -1f;
		}

		return MySceneManager.Sound.BGMDatas.list[((int)eBGM)].volume;
	}

	/// <summary>
	/// 音量を取得
	/// </summary>
	/// <param name="clip">音</param>
	/// <returns><strong>成功時</strong> 0.0f ~ 1.0fの値<br/><strong>失敗時</strong> -1.0f</returns>
	public static float GetVolume(ESE eSE)
	{
		if (MySceneManager.Sound.SEDatas.list.Length <= (int)eSE ||
			MySceneManager.Sound.SEDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eSE})");
			return -1f;
		}

		return MySceneManager.Sound.BGMDatas.list[((int)eSE)].volume;
	}



	/// <summary>
	/// 音量を設定
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="clip">音</param>
	/// <param name="value">値(0.0f~1.0df)</param>
	public static void SetVolume(AudioSource audioSource, AudioClip clip, float value)
	{
		if (audioSource == null || clip == null)
			Debug.LogError($"<color=red>指定されたオブジェクトがNULLです</color>\n");

		var _SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < _SEs.Length; i++)
		{
			if (_SEs[i].clip == clip)
			{
				_SEs[i].volume = value;
				audioSource.volume = value;
				return;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip == clip)
			{
				BGMs[i].volume = value;
				audioSource.volume = value;
				return;
			}
		}
		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color> AudioClip:[{clip}]\n");
	}

	/// <summary>
	/// 音量を設定
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="eBGM">ID</param>
	/// <param name="value">値(0.0f~1.0df)</param>
	public static void SetVolume(AudioSource audioSource, EBGM eBGM, float value)
	{
		if (MySceneManager.Sound.BGMDatas.list.Length <= (int)eBGM ||
			MySceneManager.Sound.BGMDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eBGM})");
			return;
		}

		MySceneManager.Sound.BGMDatas.list[((int)eBGM)].volume = value;
		audioSource.volume = value;
	}

	/// <summary>
	/// 音量を設定
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="eSE">ID</param>
	/// <param name="value">値(0.0f~1.0df)</param>
	public static void SetVolume(AudioSource audioSource, ESE eSE, float value)
	{
		if (MySceneManager.Sound.SEDatas.list.Length <= (int)eSE ||
			MySceneManager.Sound.SEDatas.list.Length == 0)
		{
			Debug.LogError($"無効な値です。({eSE})");
			return;
		}

		MySceneManager.Sound.BGMDatas.list[((int)eSE)].volume = value;
		audioSource.volume = value;
	}




	public static void Stop(AudioSource audioSource)
	{
		audioSource.Stop();
	}

	void Pause()
	{
		if (Source == null || Source.Count == 0)
			return;

		foreach (var item in Source)
		{
			item.Pause();
		}
	}

	void Resumed()
	{
		if (Source == null || Source.Count == 0)
			return;

		foreach (var item in Source)
		{
			item.Play();
		}
	}

	void ActiveSceneChanged(Scene thisScene, Scene nextScene)
	{
		Source.Clear();
	}
}
