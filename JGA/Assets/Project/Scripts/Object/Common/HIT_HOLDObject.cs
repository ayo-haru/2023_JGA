//=============================================================================
// @File	: [HIT_HOLD.cs]
// @Brief	: オブジェクトのタイプを別スクリプトで設定
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/18	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIT_HOLDObject : BaseObj
{
	private void Start()
	{
		objType = ObjType.HIT_HOLD;
	}
}
