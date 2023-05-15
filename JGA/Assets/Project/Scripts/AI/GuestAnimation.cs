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
// 2023/05/15	アニメーションの処理をステートからこっちに移動、首の向き変えれるようにした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class GuestAnimation : MonoBehaviour
{
    [SerializeField,Header("アニメーター")] private Animator animator;
    [SerializeField,Range(0.0f,1.0f),Header("最小アニメーション速度")] private float minAnimSpeed = 0.5f;
    [SerializeField,Range(0.0f,1.0f),Header("最大アニメーション速度")] private float maxAnimSpeed = 1.0f;
    [SerializeField,Header("ナビメッシュエージェントの最大速度")] private float maxAgentSpeed = 5.0f;
    [SerializeField, Header("首のTransform")] private Transform neckTransform;
    //歩行フラグ
    private bool bWalk = false;
    //アニメーションスピード
    private float animSpeed = 1.0f;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //アニメーションステート
    public enum EGuestAnimState { IDLE,WALK,SURPRISED,MAX_GUEST_ANIM_STATE,};
    private Transform lookAtTarget = null;
    private float fAnimTimer;

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
        bool nowWalk = GetAnimationState() == EGuestAnimState.WALK;
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

    private void LateUpdate()
    {
        if (PauseManager.isPaused) return;
        if (!neckTransform || !lookAtTarget) return;

        //首をターゲットの方向に向ける
        if(fAnimTimer < 1.0f)fAnimTimer += Time.deltaTime;
        Quaternion rot = Quaternion.LookRotation(lookAtTarget.position - neckTransform.position);
        neckTransform.rotation = Quaternion.Slerp(neckTransform.rotation, rot, fAnimTimer);
        //体をターゲットの方向に向ける
        rot = Quaternion.LookRotation(lookAtTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * (Mathf.Abs(neckTransform.rotation.y) * 10.0f + 1.0f));
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

    public void SetAnimation(EGuestAnimState _state)
    {
        switch (_state)
        {
            case EGuestAnimState.IDLE:
                animator.SetBool("isWalk", false);
                break;
            case EGuestAnimState.WALK:
                animator.SetBool("isWalk", true);
                break;
            case EGuestAnimState.SURPRISED:
                animator.SetTrigger("surprised");
                break;
        }
    }
    public void SetLookAt(Transform _target)
    {
        if (lookAtTarget == _target) return;

        fAnimTimer = 0.0f;
        lookAtTarget = _target;
    }

    public EGuestAnimState GetAnimationState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return EGuestAnimState.WALK;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return EGuestAnimState.IDLE;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Surprised")) return EGuestAnimState.SURPRISED;

        return EGuestAnimState.MAX_GUEST_ANIM_STATE;
    }
}
