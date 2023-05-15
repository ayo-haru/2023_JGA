//=============================================================================
// @File	: [FishObject.cs]
// @Brief	: 
// @Author	: YOUR_NAME
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishObject : BaseObj
{

    

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        objType = ObjType.HIT_HOLD;
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
}
