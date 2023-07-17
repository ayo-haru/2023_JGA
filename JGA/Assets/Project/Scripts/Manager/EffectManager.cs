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
using UniRx;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class EffectManager : MonoBehaviour
{
	public static class EffectData {
		public static EffectDataContainer effectDatas;
	}

	private static HashSet<ParticleSystem> Particles = new HashSet<ParticleSystem>();

	private void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		//---エフェクト
		EffectData.effectDatas = AddressableLoader<EffectDataContainer>.Load("EffectData");
	}

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

		var list = EffectData.effectDatas.list;

		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].name == displayName)
			{
				var obj = Instantiate(list[i], pos, quaternion.Value);
				Particles.Add(obj.GetComponent<ParticleSystem>());
				return obj;
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

		var list = EffectData.effectDatas.list;

		if (list.Length >= ID && ID < 0)
		{
			Debug.LogError($"<color=red>無効な値です。</color>(ID:{ID})\n");
			return null;
		}

		var obj = Instantiate(list[ID], pos, quaternion.Value);
		Particles.Add(obj.GetComponent<ParticleSystem>());
		return obj;
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

		var list = EffectData.effectDatas.list;

		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] == prefab)
			{
				var obj = Instantiate(list[i], pos, quaternion.Value);
				Particles.Add(obj.GetComponent<ParticleSystem>());
				return obj;
			}
		}
		Debug.LogError($"<color=red>指定されたオブジェクトが見つかりません</color>({prefab})\n");
		return null;
	}

	void Pause()
	{
		foreach (var item in Particles)
		{
			item.Pause();
		}
	}

	void Resumed()
	{
		foreach (var item in Particles)
		{
			item.Play();
		}
	}

}
