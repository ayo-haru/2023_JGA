//=============================================================================
// @File	: [StateAttention.cs]
// @Brief	: 注目中の処理
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

public class StateAttention : AIState
{
    //プレイヤー位置
    [SerializeField] private Transform target;
    //回転速度
    [SerializeField,Min(1)] private float rotSpeed = 2.0f;

    private NavMeshAgent agent;

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

    public override void InitState()
    {
        if(!agent)agent = GetComponent<NavMeshAgent>();
        agent.speed = 0.0f;

        //注目中のUIを表示
    }

    public override void UpdateState()
    {
        //プレイヤーの方向を向く
        Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
        rot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
        transform.rotation = rot;
    }
}
