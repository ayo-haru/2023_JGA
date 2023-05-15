//=============================================================================
// @File	: [GameVolume.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/14	スクリプト作成
//=============================================================================
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameVolume")]
public class GameVolume : ScriptableObject
{
	[System.Serializable]
	public class Volume
	{
		[Range(0.0f, 1.0f)]
		public float fBGM = 1;

		[Range(0.0f, 1.0f)]
		public float fSE = 1;
	}

	// 表示するリスト
	[SerializeField]
	public Volume volume = default;
}
