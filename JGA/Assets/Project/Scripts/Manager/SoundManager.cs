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
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static AudioClip BGM;
	private static List<AudioClip> SEs = new List<AudioClip>();


	/// <summary>
	/// AudioClipから再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="clip">音</param>
	public static void Play(AudioSource audioSource, AudioClip clip)
	{
		var SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < SEs.Length; i++)
		{
			if (SEs[i].clip == clip)
			{
				audioSource.volume = SEs[i].volume;
				audioSource.PlayOneShot(SEs[i].clip);
				return;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip == clip)
			{
				//BGM = BGMs[i].clip;
				audioSource.clip = BGMs[i].clip;
				audioSource.volume = BGMs[i].volume;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color>({audioSource})\n");
	}

	/// <summary>
	/// 名前から再生
	/// </summary>
	/// <param name="audioSource">音の再生元</param>
	/// <param name="name">音源ファイル名</param>
	public static void Play(AudioSource audioSource, string name)
	{
		var SEs = MySceneManager.Sound.SEDatas.list;
		// SEの場合
		for (int i = 0; i < SEs.Length; i++)
		{
			if (SEs[i].clip.name == name)
			{
				audioSource.volume = SEs[i].volume;
				audioSource.PlayOneShot(SEs[i].clip);
				return;
			}
		}

		var BGMs = MySceneManager.Sound.BGMDatas.list;
		// BGMの場合
		for (int i = 0; i < BGMs.Length; i++)
		{
			if (BGMs[i].clip.name == name)
			{
				audioSource.clip = BGMs[i].clip;
				audioSource.volume = BGMs[i].volume;
				audioSource.Play();
				return;
			}
		}
		Debug.LogError("<color=red>指定されたオブジェクトが見つかりません</color>(SoundManager.Play)\n");
	}




}
