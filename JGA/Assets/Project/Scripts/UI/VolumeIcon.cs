//=============================================================================
// @File	: [VolumeIcon.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumeIcon : MonoBehaviour
{
    [SerializeField] private RectTransform rt;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float interval = 50.0f;
    [SerializeField] private int maxVol = 9;
    [SerializeField] private int minVol = 0;
    private int currentVol = 0;
#if false
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
		
	}

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
    public void AddVolume()
    {
        if (currentVol >= maxVol) return;
        ++currentVol;
        //位置変更
        rt.localPosition = new Vector3(rt.localPosition.x + interval, rt.localPosition.y, rt.localPosition.z);
        //テキスト変更
        text.text = string.Format("{0:0}", currentVol);
    }
    public void DelVolume()
    {
        if (currentVol <= minVol) return;
        --currentVol;
        //位置変更
        rt.localPosition = new Vector3(rt.localPosition.x - interval, rt.localPosition.y, rt.localPosition.z);
        //テキスト変更
        text.text = string.Format("{0:0}", currentVol);
    }
}
