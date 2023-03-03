//=============================================================================
// @File	: [StateFollowPenguin.cs]
// @Brief	: ペンギン追いかける処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	(小楠)ペンギンと一定距離保てるようにした
// 2023/03/02	(小楠）データの取得方法変更
// 2023/03/03	(小楠）UI追加
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
    [SerializeField] private EmosionUI ui;
    private GuestData data;

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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();
        agent.SetDestination(target.position);
        agent.speed = data.speed;

        ui.SetEmotion(EEmotion.ENJOY);
    }

    public override void UpdateState()
    {
        //ペンギンとの距離が近い場合は移動しない
        agent.speed = (agent.remainingDistance < data.distance) ? 0.0f : data.speed;

        agent.SetDestination(target.position);
    }

    public override void FinState()
    {
        ui.SetEmotion(EEmotion.NONE);
    }
}
