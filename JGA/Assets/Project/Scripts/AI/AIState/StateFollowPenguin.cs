//=============================================================================
// @File	: [StateFollowPenguin.cs]
// @Brief	: ペンギン追いかける処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	(小楠)ペンギンと一定距離保てるようにした
// 2023/03/02	(小楠）データの取得方法変更
// 2023/03/03	(小楠）UI追加
// 2023/03/05	(小楠）UIの表示変更
// 2023/03/06	(小楠）コントローラのエラー直した　追従の仕様変更
// 2023/03/07	(小楠）プレイヤーの方向向くようにした
// 2023/03/08	(小楠）アニメーションの処理追加
// 2023/03/11	(小楠）navmeshagentの目的地をちょっとずらして、お客さんをばらけるようにした
// 2023/03/12	(小楠）プレイヤーからアピールフラグ取得した
// 2023/03/24	(小楠）ペンギンの追従速度をプレイヤー基準に変更
// 2023/03/25	(小楠）自動生成に対応
// 2023/04/10	(小楠）バグ直した
// 2023/05/20	(小楠）要らないコメント消したりした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateFollowPenguin : AIState
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
    //感情の変化時間計算用
    private float fTimer = 0.0f;
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
        agent.speed = data.speed;
        agent.stoppingDistance = data.distance;

        //プレイヤーの位置からどれだけずらすかを乱数で設定
        posOffset = new Vector3(Random.Range(-5.0f,5.0f), 0.0f,Random.Range(-5.0f,5.0f));

        //UIの表示
        ui.SetEmotion(EEmotion.ATTENSION_HIGH);

        //ペンギンの方を向かせる
        guestAnimation.SetLookAt(target);

        //タイマー初期化
        fTimer = 0.0f;
    }

    public override void UpdateState()
    {
        if (!ErrorCheck()) return;

        //客が反応したかどうか 専用アクションをしてる　かつ　反応できる範囲に居る
        if (Vector3.Distance(transform.position, target.position + posOffset) <= data.reactionArea * ((int)ui.GetEmotion() / (float)EEmotion.ATTENSION_HIGH) &&
            player.IsAppeal)
        {
             ui.SetEmotion(EEmotion.ATTENSION_HIGH);
             fTimer = 0.0f;
        }else
        {
            //感情の更新
            switch (ui.GetEmotion()) {
                case EEmotion.ATTENSION_HIGH:
                case EEmotion.ATTENSION_MIDDLE:
                    fTimer += Time.deltaTime;
                    float coolDownTime = ui.GetEmotion() == EEmotion.ATTENSION_HIGH ? data.firstCoolDownTime : data.secondCoolDownTime;
                    if(fTimer >= coolDownTime)
                    {
                        ui.SetEmotion(ui.GetEmotion() - 1);
                        fTimer = 0.0f;
                    }
                    break;
            }
        }

        //驚きモーション中は移動させない
        agent.isStopped = (guestAnimation.GetAnimationState() == GuestAnimation.EGuestAnimState.SURPRISED);
        //ペンギンが客に押されてしまうのを防ぐため、ペンギンとの距離が近かったら移動させない
        if(!agent.isStopped)agent.isStopped = Vector3.Distance(transform.position, target.position) < data.distance;

        //!!!,!!の時は追従する プレイヤーが客に向かって歩いてるときは追従しない
        float dot = Vector3.Dot(agent.velocity.normalized,player.vForce.normalized);
        agent.speed = (ui.GetEmotion() >= EEmotion.ATTENSION_MIDDLE && dot >= 0) ? player.MaxAppealSpeed * data.followSpeed : 0.0f; 
        agent.SetDestination(target.position + posOffset);

        //アニメーション更新
        guestAnimation.SetAnimation((agent.velocity.magnitude > 0.2f) ? GuestAnimation.EGuestAnimState.WALK : GuestAnimation.EGuestAnimState.IDLE);
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
