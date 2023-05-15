//=============================================================================
// @File	: [SoundVolumeManager.cs]
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

public class SoundVolumeManager : MonoBehaviour
{
	public static GameVolume.Volume GetVolume()
	{
		return MySceneManager.Volume.GameVolumeDatas.volume;
	}

	public static float GetBGM()
	{
		return MySceneManager.Volume.GameVolumeDatas.volume.fBGM;
	}

	public static float GetSE()
	{
		return MySceneManager.Volume.GameVolumeDatas.volume.fSE;
	}


	public static void SetVolume(GameVolume.Volume vol)
	{
		MySceneManager.Volume.GameVolumeDatas.volume = vol;
	}

	public static void SetBGM(float vol)
	{
		MySceneManager.Volume.GameVolumeDatas.volume.fBGM = vol;
	}

	public static void SetSE(float vol)
	{
		MySceneManager.Volume.GameVolumeDatas.volume.fSE = vol;
	}

}
