//=============================================================================
// @File	: [AIState.cs]
// @Brief	: ステートのベースクラス
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: ステートの初期化、更新処理を記述。このクラスを継承して各ステートの処理を作る
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/03	(小楠)終了処理を追加
// 2023/03/15	(小楠)エラーチェック用の関数を追加
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
    /// <summary>
    /// ステートの終了処理
    /// </summary>
    public abstract void FinState();
    /// <summary>
    /// エラーチェック
    /// </summary>
    /// <returns></returns>
    public abstract bool ErrorCheck();
}
