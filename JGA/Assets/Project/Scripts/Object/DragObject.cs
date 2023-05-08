//=============================================================================
// @File	: [DragObject.cs]
// @Brief	: 引きずるオブジェクト
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : BaseObj
{
	
	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	protected override void Awake()
	{
		Init();
		objType = ObjType.DRAG;
	}
}
