//=============================================================================
// @File	: [StateAroundWalk.cs]
// @Brief	: 歩き回る処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateAroundWalk : AIState
{
    //目的地のリスト
    [SerializeField] private List<Transform> targetList;
    //現在向かっている目的地
    private int targetNum = 0;
    //目的地に着いたときの待機時間(動物眺めてる時間)
    [SerializeField, Min(0)] private float waitTime = 10.0f;
    private float fTimer = 0.0f;
    //歩く速度
    [SerializeField, Min(0)] private float walkSpeed = 1.0f;

    private NavMeshAgent agent;

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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(targetList[targetNum].position);
        agent.speed = walkSpeed;
        fTimer = 0.0f;
    }

    public override void UpdateState()
    {
        //待機時間
        if (!agent.pathPending && agent.remainingDistance < 1.0f)
        {
            fTimer += Time.deltaTime;

            if (waitTime <= fTimer)
            {
                //目的地変更
                targetNum = Random.Range(0, targetList.Count);
                agent.SetDestination(targetList[targetNum].position);
                fTimer = 0.0f;
            }
        }
    }
}
