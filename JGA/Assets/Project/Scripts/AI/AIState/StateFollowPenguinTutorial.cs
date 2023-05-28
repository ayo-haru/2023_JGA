//=============================================================================
// @File	: [StateFollowPenguinTutorial.cs]
// @Brief	: チュートリアル用ペンギン追いかける処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/23	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateFollowPenguinTutorial : AIState
{
    //ペンギン
    private GameObject penguin;
    //ペンギンのTransform
    private Transform target;
    //ペンギンのスクリプト
    private Player player;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //感情UI
    [SerializeField] private EmosionUI ui;
    //お客さん用データ
    private GuestData.Data data;
    //アニメーター
    private GuestAnimation guestAnimation;
    //目的地の位置調節用
    private Vector3 posOffset;
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
        //オブジェクト、コンポーネントの取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        if (!penguin) penguin = GameObject.FindWithTag("Player");
        if (!target && penguin) target = penguin.transform;
        if (!player && penguin) player = penguin.GetComponent<Player>();
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();

        //エラーチェック
        if (!ErrorCheck()) return;

        //ナビメッシュエージェントの設定
        agent.SetDestination(target.position);
        agent.speed = player.MaxAppealSpeed * data.followSpeed;
        agent.stoppingDistance = data.distance;

        //プレイヤーの位置からどれだけずらすかを乱数で設定
        posOffset = new Vector3(Random.Range(-5.0f,5.0f), 0.0f,Random.Range(-5.0f,5.0f));

        //UIの表示
        ui.SetEmotion(EEmotion.ATTENSION_HIGH);

        //ペンギンの方を向かせる
        guestAnimation.SetLookAt(target);
    }

    public override void UpdateState()
    {
        if (!ErrorCheck()) return;

        //驚きモーション中は移動させない
        GuestAnimation.EGuestAnimState animState = guestAnimation.GetAnimationState();
        agent.isStopped = (animState == GuestAnimation.EGuestAnimState.SURPRISED || animState == GuestAnimation.EGuestAnimState.STAND_UP || animState == GuestAnimation.EGuestAnimState.SIT_IDLE);
        //ペンギンが客に押されてしまうのを防ぐため、ペンギンとの距離が近かったら移動させない
        if(!agent.isStopped)agent.isStopped = Vector3.Distance(transform.position, target.position) < data.distance;
        //プレイヤーが客に向かって歩いてるときは追従しない
        if(!agent.isStopped)agent.isStopped = Vector3.Dot(agent.velocity.normalized, player.vForce.normalized) < 0;
        //目的地設定
        if (NavMesh.SamplePosition(target.position + posOffset, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) agent.SetDestination(hit.position);
        //アニメーション更新
        guestAnimation.SetAnimation((agent.velocity.magnitude > 0.5f) ? GuestAnimation.EGuestAnimState.WALK : GuestAnimation.EGuestAnimState.IDLE);
    }

    public override void FinState()
    {
        if(ui)ui.SetEmotion(EEmotion.NONE);
        if(agent)agent.stoppingDistance = 0.0f;
    }

    public override bool ErrorCheck()
    {
        if (!penguin)Debug.LogError("プレイヤーが取得できてません");
        if (!target)Debug.LogError("プレイヤーのトランスフォームが取得できてません");
        if (!player)Debug.LogError("プレイヤー用スクリプトが取得できてません");
        if (!agent)Debug.LogError("ナビメッシュエージェントが取得できてません");
        if (!ui)Debug.LogError("感情UIが設定されていません");
        if (data==null)Debug.LogError("ゲスト用データがが取得できてません");
        if (!guestAnimation) Debug.LogError("アニメーション制御用スクリプトが取得できていません");

        return penguin && target && player && agent && ui && (data!=null) && guestAnimation;
    }
}
