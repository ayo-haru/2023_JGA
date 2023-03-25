//=============================================================================
// @File	: [TransitionInRangePlayer.cs]
// @Brief	: 遷移条件　プレイヤーが範囲内にいるか
// @Author	: Ogusu Yuuko
// @Editer	: Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/03/05	スクリプト作成
// 2023/03/06	スクリプト名変えた エラーチェック追加
// 2023/03/25	自動生成に対応
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInRangePlayer : AITransition
{
    //お客さん用データ
    private GuestData.Data data;
    //遷移条件反転用フラグ
    [SerializeField,Tooltip("プレイヤーが範囲外に出たら遷移したい場合はチェックを入れてください")] private bool inv = false;
    //プレイヤーのトランスフォーム
    private Transform target;

#if false
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
#endif
    public override void InitTransition()
    {
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        if (!target) target = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        return (Vector3.Distance(gameObject.transform.position, target.position) <= data.reactionArea) != inv;
    }

    public override bool ErrorCheck()
    {
        if (!target)Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");

        return target && (data!=null);
    }
}
