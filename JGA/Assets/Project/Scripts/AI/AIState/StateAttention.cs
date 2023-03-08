//=============================================================================
// @File	: [StateAttention.cs]
// @Brief	: 注目中の処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
// 2023/03/03	(小楠)終了処理追加　UI追加
// 2023/03/05	(小楠)UIの表示を変更
// 2023/03/08	(小楠)アニメーションの処理を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateAttention : AIState
{
    //プレイヤー位置
    private Transform target;
    //回転速度
    [SerializeField,Min(1)] private float rotSpeed = 2.0f;
    //感情UI
    [SerializeField] private EmosionUI ui;

    private NavMeshAgent agent;

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
        if(!agent)agent = GetComponent<NavMeshAgent>();
        if(agent)agent.speed = 0.0f;
        if (!target) target = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //注目中のUIを表示
        ui.SetEmotion(EEmotion.QUESTION);

        if(!animator) animator = GetComponent<Animator>();
        if (animator) animator.SetBool("isWalk", false);
    }

    public override void UpdateState()
    {
        if (target)
        {
            //プレイヤーの方向を向く
            Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
            rot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
            transform.rotation = rot;
        }else
        {
            Debug.LogError("プレイヤーが取得できていません");
        }

    }

    public override void FinState()
    {
        ui.SetEmotion(EEmotion.NONE);
    }
}
