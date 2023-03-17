//=============================================================================
// @File	: [EffectManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/17	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	/// <summary>
	/// エフェクト生成 - 名前
	/// </summary>
	/// <param name="pos">生成位置</param>
	/// <param name="displayName">エフェクト名</param>
	/// <param name="quaternion">回転</param>
	/// <returns>生成したエフェクト</returns>
	public static GameObject Create(Vector3 pos, string displayName, Quaternion? quaternion = null)
	{
		// もしnullの場合はidentity代入
		if (!quaternion.HasValue)
			quaternion = Quaternion.identity;

		var list = MySceneManager.Effect.effectDatas.list;

		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].displayName == displayName)
			{
				return Instantiate(list[i].prefab, pos, quaternion.Value);
			}
		}

		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color>({displayName})\n");
		return null;
	}

	/// <summary>
	/// エフェクト生成 - ID
	/// </summary>
	/// <param name="pos">生成位置</param>
	/// <param name="ID">エフェクト番号</param>
	/// <param name="quaternion">回転</param>
	/// <returns>生成したエフェクト</returns>
	public static GameObject Create(Vector3 pos, int ID, Quaternion? quaternion = null)
	{
		if (!quaternion.HasValue)
			quaternion = Quaternion.identity;

		if (ID < 0)
		{
			Debug.LogError($"<color=red>無効な値です。</color>({ID})\n");
			return null;
		}

		var list = MySceneManager.Effect.effectDatas.list;

		if (list.Length >= ID)
		{
			Debug.LogError($"<color=red>無効な値です。</color>({ID} >= list.Length)\n");
			return null;
		}

		return Instantiate(list[ID].prefab, pos, quaternion.Value);
	}

	/// <summary>
	/// エフェクト生成 - Prefab
	/// </summary>
	/// <param name="pos">生成位置</param>
	/// <param name="prefab">Prefab</param>
	/// <param name="quaternion">回転</param>
	/// <returns>生成したエフェクト</returns>
	public static GameObject Create(Vector3 pos, GameObject prefab, Quaternion? quaternion = null)
	{
		if (!quaternion.HasValue)
			quaternion = Quaternion.identity;

		var list = MySceneManager.Effect.effectDatas.list;

		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].prefab == prefab)
			{
				return Instantiate(list[i].prefab, pos, quaternion.Value);
			}
		}
		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color>({prefab})\n");
		return null;
	}
}
