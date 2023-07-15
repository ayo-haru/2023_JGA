//=============================================================================
// @File	: [IManagerCommon.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/07/16	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManagerCommon
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_asyncOperation"></param>
    /// <returns></returns>
    public IEnumerator WaitAddScene(AsyncOperation _asyncOperation);
}
