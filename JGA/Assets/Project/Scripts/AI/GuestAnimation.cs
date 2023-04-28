//=============================================================================
// @File	: [GuestAnimation.cs]
// @Brief	: ゲスト用アニメーション制御スクリプト
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/21	スクリプト作成
// 2023/03/22	ポーズの処理追加
// 2023/03/30	足音削除
// 2023/04/28	歩くときのアニメーションスピードをNavMeshAgentの速度基準に変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class GuestAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField,Range(0.0f,1.0f)] private float minAnimSpeed = 0.5f;
    [SerializeField,Range(0.0f,1.0f)] private float maxAnimSpeed = 1.0f;
    [SerializeField] private float maxAgentSpeed = 5.0f;
    private bool bWalk = false;
    private float animSpeed = 1.0f;
    private NavMeshAgent agent;


	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        //ポーズ時の動作を登録
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(gameObject);

        agent = GetComponent<NavMeshAgent>();
        bWalk = animator.GetCurrentAnimatorStateInfo(0).IsName("Walk");
    }
#if false
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{


    }
#endif
	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (PauseManager.isPaused) return;
        if (!animator || !agent) return;

        //Walkから他のアニメーション又は他のアニメーションからWalkに切り替わったか？
        bool nowWalk = animator.GetCurrentAnimatorStateInfo(0).IsName("Walk");
        if (bWalk == nowWalk) return;

        //切り替わっていた場合
        bWalk = nowWalk;
        if (!bWalk){
            animator.speed = animSpeed = 1.0f;
        }else{
            float fWork = agent.speed / maxAgentSpeed;
            animator.speed = animSpeed = (fWork < minAnimSpeed) ? minAnimSpeed : (fWork >= maxAnimSpeed) ? maxAnimSpeed : fWork;
        }
    }
#if false
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
		
	}
#endif
    private void Pause()
    {
        if(animator)animator.speed = 0.0f;
    }

    private void Resumed()
    {
        if(animator)animator.speed = animSpeed;
    }
}
