//=============================================================================
// @File	: [EffectData.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/17	スクリプト作成
//=============================================================================
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EffectData")]
public class EffectData : ScriptableObject
{
	[System.Serializable]
	public class Effect
	{
		// 表示名
		public string displayName;

		// 生成するオブジェクト
		public GameObject prefab;
	}

	// 表示するリスト
	[SerializeField]
	public Effect[] list = default;
}
