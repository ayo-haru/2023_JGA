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
// 2023/05/28	後ろ歩きつくった
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
    //アニメーションスピード
    private float animSpeed = 1.0f;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //アニメーションステート
    public enum EGuestAnimState { IDLE,WALK,SURPRISED,SIT_IDLE,STAND_UP,WAIT,WATCH1,WATCH2,MAX_GUEST_ANIM_STATE,};
    private EGuestAnimState state;
    //首を向ける方向
    private Transform lookAtTarget = null;
    private Transform beforeLookAtTarget = null;
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
        state = EGuestAnimState.IDLE;
    }
#if false
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{


    }
#endif
#if false
    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
        MyFixedUpdate();
    }
#endif
    public void MyFixedUpdate()
    {
        if (PauseManager.isPaused) return;
        if (!animator || !agent) return;

        //立ち上がった時に位置がNavmeshとモデルの位置がずれるため、補正する
        if (!animator.applyRootMotion && animator.transform.localPosition != Vector3.zero)
        {
            animator.transform.localPosition = Vector3.MoveTowards(animator.transform.localPosition, Vector3.zero, 0.01f);
        }

        EGuestAnimState nowState = GetAnimationState();
        // アニメーションが切り替わっているか
        if (nowState != state)
        {
            //Walkから他のアニメーション又は他のアニメーションからWalkに切り替わったか？
            if (nowState == EGuestAnimState.WALK)
            {
                float fWork = agent.speed / maxAgentSpeed;
                animSpeed = (fWork < minAnimSpeed) ? minAnimSpeed : (fWork >= maxAnimSpeed) ? maxAnimSpeed : fWork;
                animator.SetFloat("speed", animSpeed);
            }
            else if (state == EGuestAnimState.WALK)
            {
                animSpeed = 1.0f;
                animator.SetFloat("speed", animSpeed);
            }

            //座った状態になったか？
            if (nowState == EGuestAnimState.SIT_IDLE)
            {
                animator.applyRootMotion = true;
            }
            //立ち上がった状態になったか
            if (nowState == EGuestAnimState.SURPRISED && state == EGuestAnimState.STAND_UP)
            {
                animator.applyRootMotion = false;
            }
        }

        //歩いているか？
        if (nowState == EGuestAnimState.WALK)
        {
            //客の向きとagentの進行方向が逆の場合は後ろ歩き
            //客の向きとagentの進行方向が同じ場合は前歩き
            if ((Vector3.Dot(transform.forward, agent.velocity) < 0 && animSpeed > 0) || (Vector3.Dot(transform.forward, agent.velocity) > 0 && animSpeed < 0))
            {
                animSpeed *= -1;
                animator.SetFloat("speed", animSpeed);
            }
        }

        state = nowState;
    }
#if false
    private void LateUpdate()
    {
        MyLateUpdate();
    }
#endif
    public void MyLateUpdate()
    {
        if (PauseManager.isPaused) return;
        if (!neckTransform) return;
        //座っている又は立っている最中は回転しない
        EGuestAnimState state = GetAnimationState();
        if (state == EGuestAnimState.SIT_IDLE || state == EGuestAnimState.STAND_UP) return;

        if (lookAtTarget)
        {
            //首をターゲットの方向に向ける
            if (fAnimTimer < 1.0f) fAnimTimer += Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(lookAtTarget.position - neckTransform.position);
            neckTransform.rotation = Quaternion.Slerp(neckTransform.rotation, rot, fAnimTimer);
            //体をターゲットの方向に向ける
            rot = Quaternion.LookRotation(lookAtTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * (Mathf.Abs(neckTransform.rotation.y) * 10.0f + 1.0f));
        }
        else if (beforeLookAtTarget)
        {
            //首を元の位置に戻す
            if (fAnimTimer > 0.0f) fAnimTimer -= Time.deltaTime;
            Quaternion rot = Quaternion.LookRotation(beforeLookAtTarget.position - neckTransform.position);
            neckTransform.rotation = Quaternion.Slerp(neckTransform.rotation, rot, fAnimTimer);
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
        if (animator) animator.SetFloat("speed", 0.0f);
    }

    private void Resumed()
    {
        if (animator) animator.SetFloat("speed", animSpeed);
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
            case EGuestAnimState.STAND_UP:          //立ち上がった後にびっくりするアニメーションを再生するため
                animator.SetTrigger("surprised");
                break;
            case EGuestAnimState.SIT_IDLE:
                animator.SetTrigger("sit");
                break;
            case EGuestAnimState.WAIT:
                animator.SetTrigger("wait");
                break;
            case EGuestAnimState.WATCH1:
                animator.SetTrigger("watch1");
                break;
            case EGuestAnimState.WATCH2:
                animator.SetTrigger("watch2");
                break;
        }
    }
    public void SetLookAt(Transform _target)
    {
        if (lookAtTarget == _target) return;

        fAnimTimer = (_target) ? 0.0f : 1.0f;
        beforeLookAtTarget = lookAtTarget;
        lookAtTarget = _target;
    }

    public EGuestAnimState GetAnimationState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return EGuestAnimState.IDLE;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return EGuestAnimState.WALK;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Surprised")) return EGuestAnimState.SURPRISED;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sit")) return EGuestAnimState.SIT_IDLE;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("StandUp")) return EGuestAnimState.STAND_UP;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait")) return EGuestAnimState.WAIT;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Watch1")) return EGuestAnimState.WATCH1;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Watch2")) return EGuestAnimState.WATCH2;

        return EGuestAnimState.MAX_GUEST_ANIM_STATE;
    }
}
