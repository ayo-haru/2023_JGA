//=============================================================================
// @File	: [KeepOutSign.cs]
// @Brief	: 立ち入り禁止看板
// @Author	: KAIKAWA KOYO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/13	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepOutSign : MonoBehaviour
{
    private Rigidbody rb;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        rb = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
	    if(rb.IsSleeping())
        {
            rb.isKinematic = true;
        }
	}
}
