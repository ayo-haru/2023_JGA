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
	private void Awake()
	{
		//SceneManager.activeSceneChanged += ActiveSceneChanged;
	}

	public void GetVolume()
	{
		//MySceneManager.Volume.GameVolumeDatas;
	}
}
