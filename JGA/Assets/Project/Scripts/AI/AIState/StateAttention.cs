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
// 2023/03/23	(小楠)驚いたアニメーション追加
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
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //アニメーター
    private Animator animator;
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
        //コンポーネント取得
        if(!agent)agent = GetComponent<NavMeshAgent>();
        if (!target) target = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if(!animator) animator = GetComponent<Animator>();

        //エラーチェック
        if (!ErrorCheck()) return;

        agent.speed = 0.0f;
        ui.SetEmotion(EEmotion.QUESTION);
        animator.SetBool("isWalk", false);
        animator.SetTrigger("surprised");
    }

    public override void UpdateState()
    {
        //エラーチェック
        if (!ErrorCheck()) return;

        //プレイヤーの方向を向く
        Quaternion rot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);

    }

    public override void FinState()
    {
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!target)Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (!ui)Debug.LogError("感情UIが設定されていません");
        if (!agent)Debug.LogError("ナビメッシュエージェントが取得されていません");
        if (!animator)Debug.LogError("アニメータが取得されていません");

        return target && ui && agent && animator;
    }
}
