//=============================================================================
// @File	: [StateStayArea.cs]
// @Brief	: 指定エリアで待機
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateStayArea : AIState
{
    private NavMeshAgent agent;
    [SerializeField] Transform target;
    private GuestData data;
    //目的地に着いたか
    private bool isStay = false;
	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        agent = GetComponent<NavMeshAgent>();
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

    public override void InitState()
    {
        if (!agent) GetComponent<NavMeshAgent>();
        agent.SetDestination(target.position);
        agent.speed = data.speed;
        isStay = false;
    }

    public override void UpdateState()
    {
        if (isStay) return;

        if (agent.pathPending) return;
        //指定位置に着いたら
        if (agent.remainingDistance < 1.0f)
        {
            //待機アニメーションの再生

            //移動停止
            agent.speed = 0.0f;

            isStay = true;

        }
    }
}