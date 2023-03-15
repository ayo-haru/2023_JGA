//=============================================================================
// @File	: [AITransition.cs]
// @Brief	: 遷移条件のベースクラス
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: ステートの遷移条件を記述。このクラスを継承して各遷移条件を作る。
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/15	(小楠)エラーチェック用の関数追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AITransition : MonoBehaviour
{
    /// <summary>
    /// 初期化処理
    /// </summary>
    public abstract void InitTransition();
    /// <summary>
    /// 遷移できるかどうか
    /// </summary>
    /// <returns></returns>
    public abstract bool IsTransition();
    /// <summary>
    /// エラーチェック
    /// </summary>
    /// <returns></returns>
    public abstract bool ErrorCheck();
}
