//=============================================================================
// @File	: [StateAroundWalk.cs]
// @Brief	: 歩き回る処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	(小楠)目的地にたどり着けないときの処理を追加
// 2023/03/02	(小楠)データの取得方法変更
// 2023/03/03	(小楠)終了処理追加
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
    //待機時間カウント用
    private float fTimer = 0.0f;

    private NavMeshAgent agent;

    private GuestData data;
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
    public override void InitState()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();
        agent.SetDestination(targetList[targetNum].position);
        agent.speed = data.speed;
        fTimer = 0.0f;
    }

    public override void UpdateState()
    {
        if (agent.pathPending) return;

        //目的地付けないときは目的地変更
        if (agent.path.status == NavMeshPathStatus.PathPartial)
        {
            ChangeTarget();
        }

        //待機時間
        if (agent.remainingDistance < 1.0f)
        {
            fTimer += Time.deltaTime;

            if (data.waitTime <= fTimer)
            {
                ChangeTarget();
            }
        }
    }

    public override void FinState()
    {
        //特になし
    }

    public override bool ErrorCheck()
    {
        return true;
    }
    /// <summary>
    /// 目的地変更
    /// </summary>
    public void ChangeTarget()
    {
        targetNum = Random.Range(0, targetList.Count);
        agent.SetDestination(targetList[targetNum].position);
        fTimer = 0.0f;
    }
}
