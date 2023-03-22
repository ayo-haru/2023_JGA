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

public class SoundManager : MonoBehaviour
{
	public enum EBGM
	{
		TITLE_001,

	}
	public enum ESE
	{
		PENGUIN_VOICE,
		PENGUIN_CATCH,
		PENGUIN_HIT_001,
		PENGUIN_WALK_001,
		PENGUIN_RUN_001,

		HOURCE_WALK_001,
		HOURCE_RUN_001,

		CARDBOARDBOX_001,
		CARDBOARDBOX_002,
		DOOR_CLOSE,
		DOOR_OPEN,
		INTO_WATER,

		HUMAN_WALK_001,
		HUMAN_WALK_002,
		HUMAN_WALK_003,
		HUMAN_RUN_001,
		HUMAN_RUN_002,
		HUMAN_RUN_003,

		SELECT_001,
		SELECT_002,
	}

	private static HashSet<AudioSource> Source = new HashSet<AudioSource>();
	private static AudioClip BGM;
	private static List<AudioClip> SEs = new List<AudioClip>();

	private void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

	}


	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="clip">音</param>
	public static void Play(AudioSource audioSource, AudioClip clip)
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
				audioSource.volume = _SEs[i].volume;
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
				audioSource.volume = BGMs[i].volume;
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
	public static void Play(AudioSource audioSource, EBGM eBGM)
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
		audioSource.volume = _BGM.volume;
		audioSource.Play();
	}

	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="id">ID</param>
	public static void Play(AudioSource audioSource, ESE eSE)
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
		audioSource.volume = _SE.volume;
		audioSource.PlayOneShot(_SE.clip);
	}

	/// <summary>
	/// 名前から再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="name">音源ファイル名</param>
	public static void Play(AudioSource audioSource, string name)
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
				audioSource.volume = _SEs[i].volume;
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
				audioSource.volume = BGMs[i].volume;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError("<color=red>指定されたオブジェクトが見つかりません</color>(SoundManager.Play)\n");
	}

	void Pause()
	{
		foreach (var item in Source)
		{
			item.Pause();
		}
	}

	void Resumed()
	{
		foreach (var item in Source)
		{
			item.Play();
		}
	}
}
