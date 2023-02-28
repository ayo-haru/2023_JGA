//=============================================================================
// @File	: [StateFollowPenguin.cs]
// @Brief	: ペンギン追いかける処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	ペンギンと一定距離保てるようにした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateFollowPenguin : AIState
{
    //ペンギンのTransform
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    [SerializeField, Min(0)] private float followSpeed = 1.0f;

    //ペンギンとの距離
    [SerializeField, Range(2, 10)] private float distance = 1.0f;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        agent = GetComponent<NavMeshAgent>();
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
        agent.speed = followSpeed;
    }

    public override void UpdateState()
    {
        //ペンギンとの距離が近い場合は移動しない
        agent.speed = (agent.remainingDistance < distance) ? 0.0f : followSpeed;

        agent.SetDestination(target.position);
    }
}
