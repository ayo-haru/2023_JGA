//=============================================================================
// @File	: [TransitionFarTarget.cs]
// @Brief	: 遷移条件　ターゲットが遠ざかったら
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
// 2023/03/02	(小楠)データの取得方法変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFarTarget : AITransition
{
    //ターゲットのTransforom
    [SerializeField] private Transform target;

    private GuestData data;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        data = GetComponent<AIManager>().GetGuestData();
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
        //特になし
    }

    public override bool IsTransition()
    {
        return Vector3.Distance(gameObject.transform.position, target.position) > data.reactionArea ? true : false;
    }
}