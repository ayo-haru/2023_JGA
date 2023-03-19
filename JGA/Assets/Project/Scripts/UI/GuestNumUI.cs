//=============================================================================
// @File	: [GuestNumUI.cs]
// @Brief	: ゲスト人数表示用UI
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/19	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuestNumUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guestNum;
    //目標人数
    [SerializeField,Range(1,99)] private int clearNum = 10;
    //現在の人数
    private int currentNum = 0;
#if false
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
		
	}
#endif
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        guestNum.text = string.Format("{0:00} / {1:00}", currentNum, clearNum);
    }
#if false
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

    public void Add()
    {
        if (currentNum >= 99) return;
        ++currentNum;
        guestNum.text = string.Format("{0:00} / {1:00}", currentNum, clearNum);
    }
}
