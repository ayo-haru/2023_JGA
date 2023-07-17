//=============================================================================
// @File	: [SoundVolumeManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/16	スクリプト作成
// 2023/06/29	BGMの音量変更を追加【小楠】
//=============================================================================
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundVolumeManager /* : MonoBehaviour*/
{
	public static GameVolume.Volume GetVolume()
	{
		return SoundManager.GameVolumeDatas.volume;
	}

	public static float GetBGM()
	{
		return SoundManager.GameVolumeDatas.volume.fBGM;
	}

	public static float GetSE()
	{
		return SoundManager.GameVolumeDatas.volume.fSE;
	}


	public static void SetVolume(GameVolume.Volume vol)
	{
		SoundManager.GameVolumeDatas.volume = vol;
	}

	public static void SetBGM(float vol)
	{
		SoundManager.GameVolumeDatas.volume.fBGM = vol;
		//再生中のBGMの音量を変える
		SoundManager.SetVolume();
	}

	public static void SetSE(float vol)
	{
		SoundManager.GameVolumeDatas.volume.fSE = vol;
	}

}
