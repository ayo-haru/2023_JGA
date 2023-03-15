//=============================================================================
// @File	: [TransitionInRangeDestination.cs]
// @Brief	: 遷移条件　目的地が範囲内にあるか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TransitionInRangeDestination : AITransition
{
    //ゲスト用データ
    private GuestData data;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //遷移条件反転用フラグ
    [SerializeField,Tooltip("目的地から離れた場合に遷移したいときはチェック入れてください")] private bool inv;

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
        if (!data) data = GetComponent<AIManager>().GetGuestData();
        if (!agent) agent = GetComponent<NavMeshAgent>();
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        return (Vector3.Distance(gameObject.transform.position, agent.destination) <= data.reactionArea) != inv;
    }

    public override bool ErrorCheck()
    {
        bool bError = true;
        if (!data)
        {
            Debug.LogError("ゲスト用データが取得されていません");
            bError = false;
        }
        if (!agent)
        {
            Debug.LogError("ナビメッシュエージェントが取得されていません");
            bError = false;
        }
        return bError;
    }
}
