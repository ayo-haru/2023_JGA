//=============================================================================
// @File	: [AIState.cs]
// @Brief	: ステートのベースクラス
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: ステートの初期化、更新処理を記述。このクラスを継承して各ステートの処理を作る
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    /// <summary>
    /// ステートの初期化処理
    /// </summary>
    public abstract void InitState();
    /// <summary>
    /// ステートの更新処理
    /// </summary>
    public abstract void UpdateState();
}
