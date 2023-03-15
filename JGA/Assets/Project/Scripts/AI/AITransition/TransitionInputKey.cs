//=============================================================================
// @File	: [TransitionInputKey.cs]
// @Brief	: 遷移条件　指定されたキーが押されたら
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInputKey : AITransition
{

    [SerializeField] private KeyCode key = KeyCode.Return;

    private bool bInput = false;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        bInput = false;
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
#endif
	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (Input.GetKeyDown(key))
        {
            bInput = true;
        }
	}


    public override void InitTransition()
    {
        bInput = false;
    }

    public override bool IsTransition()
    {
        if (bInput)
        {
            bInput = false;
            return true;
        }
        return false;
    }

    public override bool ErrorCheck()
    {
        return true;
    }
}
