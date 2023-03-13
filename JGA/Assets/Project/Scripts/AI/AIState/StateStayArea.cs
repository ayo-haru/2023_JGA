//=============================================================================
// @File	: [StateStayArea.cs]
// @Brief	: 指定エリアで待機
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
// 2023/03/03	(小楠)終了処理追加
// 2023/03/08	(小楠)アニメーションの制御追加
// 2023/03/11	(小楠)目的地の方を向くようにした、目的地との距離を調整
// 2023/03/11	(小楠）navmeshagentの目的地をちょっとずらして、お客さんをばらけるようにした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateStayArea : AIState
{
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //待機位置のTransform
    [SerializeField] Transform target;
    //お客さん用データ
    private GuestData data;
    //目的地に着いたか
    private bool isStay = false;
    //アニメーター
    private Animator animator;
    //感情ui
    [SerializeField] EmosionUI ui;

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
        //ナビメッシュエージェント、データ取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();

        //ナビメッシュエージェントの設定
        agent.SetDestination(target.position + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f)));
        agent.speed = data.speed;
        agent.stoppingDistance = Random.Range(1.0f,data.cageDistance);

        isStay = false;

        //アニメーション初期化
        if (!animator) animator = GetComponent<Animator>();
        if (animator) animator.SetBool("isWalk", true);

        //ui設定
        ui.SetEmotion(EEmotion.NONE);
    }

    public override void UpdateState()
    {
        if (isStay)
        {
            //目的地の方を向く
            //できれば、動物の方を向くようにしたい
            Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            return;
        }

        if (agent.pathPending) return;
        //指定位置に着いたら
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //待機アニメーションの再生
            if (animator) animator.SetBool("isWalk", false);
            //ui設定
            ui.SetEmotion(EEmotion.HIGH_TENSION);
            isStay = true;

        }
    }

    public override void FinState()
    {
        agent.stoppingDistance = 0.0f;
        ui.SetEmotion(EEmotion.NONE);
    }
}
