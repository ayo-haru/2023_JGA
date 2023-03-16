//=============================================================================
// @File	: [SoundData.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/16	スクリプト作成
//=============================================================================
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SoundData")]
public class SoundData : ScriptableObject
{
	[System.Serializable]
	public class Sound
	{
		public AudioClip clip;     // 音源

		[Range(0.0f, 1.0f)]
		public float volume = 1;
	}

	// 表示するリスト
	[SerializeField]
	public Sound[] list = default;

}
