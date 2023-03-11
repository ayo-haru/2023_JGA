//=============================================================================
// @File	: [StateDefaultRootWalk.cs]
// @Brief	: 指定されたルートの移動
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
// 2023/03/02	(小楠）データの取得方法変更
// 2023/03/03	(小楠）終了処理追加
// 2023/03/08	(小楠）アニメーション追加。ターゲットリストが0の時のエラー直した
// 2023/03/10	(小楠）追跡範囲の変更。目的地の方向くようにした
// 2023/03/11	(小楠）目的地との距離を調整
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateDefaultRootWalk : AIState
{
    //目的地のリスト
    [SerializeField] private List<Transform> targetList;
    //現在向かっている目的地
    private int targetNum = 0;
    //待機時間カウント用
    private float fTimer = 0.0f;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //お客さん用のデータ
    private GuestData data;
    //アニメーター
    private Animator animator;

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
        //ナビメッシュ取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        //データ取得
        if (!data) data = GetComponent<AIManager>().GetGuestData();

        //ナビメッシュエージェントの設定
        if(targetList.Count > 0) agent.SetDestination(targetList[targetNum].position);
        agent.speed = data.speed;
        agent.stoppingDistance = Random.Range(1,data.cageDistance);

        fTimer = 0.0f;

        //アニメーション初期化
        if (!animator) animator = GetComponent<Animator>();
        if (animator) animator.SetBool("isWalk", true);
    }

    public override void UpdateState()
    {
        if (targetList.Count <= 0) return;
        if (agent.pathPending) return;

        //目的地までの経路がない場合は目的地の変更
        if(agent.path.status == NavMeshPathStatus.PathPartial)
        {
            ChangeTarget();
        }
        //待機時間
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (animator) animator.SetBool("isWalk", false);

            fTimer += Time.deltaTime;
            if (data.waitTime <= fTimer)
            {
                ChangeTarget();
            }

            //目的地の方を向く
            //できれば、動物の方を向くようにしたい
            Quaternion rot = Quaternion.LookRotation(targetList[targetNum].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

        }
    }

    public override void FinState()
    {
        agent.stoppingDistance = 0.0f;
    }
    /// <summary>
    /// 目的地の変更
    /// </summary>
    public void ChangeTarget()
    {
        //ターゲットが1つ以下の場合は処理しない
        if(targetList.Count <= 1) return;

        targetNum = (targetNum + 1) % targetList.Count;
        agent.SetDestination(targetList[targetNum].position);
        fTimer = 0.0f;
        if (animator) animator.SetBool("isWalk", true);
    }
}
