//=============================================================================
// @File	: [VolumeIcon.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: Sakai Ryotaro
// @Detail	: 
// 
// [Date]
// 2023/04/08	スクリプト作成
// 2023/05/15	音量調整機能実装
// 2023/05/28	開始時に数字が０になってしまうの直した
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VolumeIcon : MonoBehaviour
{
	[SerializeField] private RectTransform rt;
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private float interval = 50.0f;
	[SerializeField] private int maxVol = 9;
	[SerializeField] private int minVol = 0;
	private int currentVol = 0;
	[SerializeField,Tooltip("BGM/true   SE/false")] private bool bBGM = true;
	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Start()
	{
		//現在の音量を取得する
		if (bBGM)
		{
			//BGMの音量を取得
			currentVol = (int)(SoundVolumeManager.GetBGM() * maxVol);
		}
		else
		{
			//SEの音量を取得
			currentVol = (int)(SoundVolumeManager.GetSE() * maxVol);
		}

        //アイコン位置変更
        rt.localPosition = new Vector3(rt.localPosition.x + interval * currentVol, rt.localPosition.y, rt.localPosition.z);
        //テキスト変更
        text.text = string.Format("{0:0}", currentVol);
    }

#if false
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
#endif
	public bool AddVolume()
	{
		if (currentVol >= maxVol) return false;
		++currentVol;
		//位置変更
		rt.localPosition = new Vector3(rt.localPosition.x + interval, rt.localPosition.y, rt.localPosition.z);
		//テキスト変更
		text.text = string.Format("{0:0}", currentVol);

		//サウンドマネージャー？の音量設定の関数を呼ぶ
		if (bBGM)
		{
			SoundVolumeManager.SetBGM((float)currentVol / maxVol);
		}
		else
		{
			SoundVolumeManager.SetSE((float)currentVol / maxVol);
		}

		return true;
	}
	public bool DelVolume()
	{
		if (currentVol <= minVol) return false;
		--currentVol;
		//位置変更
		rt.localPosition = new Vector3(rt.localPosition.x - interval, rt.localPosition.y, rt.localPosition.z);
		//テキスト変更
		text.text = string.Format("{0:0}", currentVol);

		//サウンドマネージャー？の音量設定の関数を呼ぶ
		if (bBGM)
		{
			SoundVolumeManager.SetBGM((float)currentVol / maxVol);
		}
		else
		{
			SoundVolumeManager.SetSE((float)currentVol / maxVol);
		}

		return true;
	}
}
