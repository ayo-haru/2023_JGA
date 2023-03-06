//=============================================================================
// @File	: [TransitionInRangeOther.cs]
// @Brief	: 遷移条件　ターゲット(プレイヤー以外)が範囲内に居るか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: プレイヤーが範囲内に居るか判定したい場合はTransitionInRangePlayer.csを使ってください
// 
// [Date]
// 2023/03/06	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInRangeOther : AITransition
{
    private GuestData data;
    [SerializeField,Tooltip("ターゲットが範囲外に出たら遷移したい場合はチェックを入れてください")] private bool inv = false;
    [SerializeField]private Transform target;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		
	}

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

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

    public override void InitTransition()
    {
        if (!data) data = GetComponent<AIManager>().GetGuestData();
    }

    public override bool IsTransition()
    {
        if (!target)
        {
            Debug.LogError("ターゲットが設定されていません");
            return false;
        }
        return (Vector3.Distance(gameObject.transform.position, target.position) <= data.reactionArea) != inv;
    }
}
