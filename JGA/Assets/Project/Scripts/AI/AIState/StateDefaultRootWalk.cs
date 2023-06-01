//=============================================================================
// @File	: [StateDefaultRootWalk.cs]
// @Brief	: 指定されたルートの移動
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
// 2023/03/02	(小楠）データの取得方法変更
// 2023/03/03	(小楠）終了処理追加
// 2023/03/08	(小楠）アニメーション追加。ターゲットリストが0の時のエラー直した
// 2023/03/10	(小楠）追跡範囲の変更。目的地の方向くようにした
// 2023/03/11	(小楠）目的地との距離を調整
// 2023/03/18	(小楠）動物の方向くようにした
// 2023/03/25	(伊地田）自動生成に対応
// 2023/05/20	(小楠)要らないコメント消したりした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateDefaultRootWalk : AIState
{
    //動物のTransform
    private List<Transform> animals;
    //現在向かっている目的地
    private int targetNum = 0;
    //待機時間カウント用
    private float fTimer = 0.0f;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //お客さん用のデータ
    private GuestData.Data data;
    //アニメーター
    private GuestAnimation guestAnimation;
    //ペンギン用スクリプト
    private Player player;
    
    //一回だけ処理するようのフラグ
    private bool bOnce = true;
    private bool bChange = false;

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
        //データ、コンポーネント取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();
        if (!player) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        GetAnimalsTransrom();

        //エラーチェック
        if (!ErrorCheck()) return;

        //ナビメッシュエージェントの設定
        agent.SetDestination(data.rootTransforms[targetNum].position);
        agent.speed = (player) ? player.MaxAppealSpeed * data.followSpeed * data.inBoothSpeed : data.speed;
        agent.stoppingDistance = Random.Range(1,data.cageDistance);

        //この時点で目的地の近くにいる場合はばらけさせる
        if(bOnce && Vector3.Distance(transform.position,agent.destination) <= agent.stoppingDistance && data.rootTransforms.Count == 1)
        {
            agent.SetDestination(data.rootTransforms[targetNum].position + new Vector3(Random.Range(-10.0f,10.0f), 0.0f, Random.Range(-10.0f,10.0f)) * agent.stoppingDistance);
            bChange = true;
        }
        bOnce = false;

        //待機時間初期化
        fTimer = 0.0f;

        //アニメーション設定
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WALK);
        guestAnimation.SetLookAt(null);
    }

    public override void UpdateState()
    {
        //エラーチェック
        if (!ErrorCheck()) return;

        //驚きモーション中は移動させない
        agent.isStopped = (guestAnimation.GetAnimationState() == GuestAnimation.EGuestAnimState.SURPRISED);

        //経路計算中
        if (agent.pathPending) return;

        //目的地までの経路がない場合は目的地の変更
        if(agent.path.status == NavMeshPathStatus.PathPartial)
        {
            ChangeTarget();
        }

        //目的地へ移動中
        if (agent.remainingDistance > agent.stoppingDistance) return;

        //待機時間中の処理
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.IDLE);

        //ランダム生成用エントランスに到着したら
        if ((agent.destination.x - agent.stoppingDistance < data.entranceTF.position.x && agent.destination.x + agent.stoppingDistance > data.entranceTF.position.x) &&
            (agent.destination.z - agent.stoppingDistance < data.entranceTF.position.z && agent.destination.z + agent.stoppingDistance > data.entranceTF.position.z))
        {
            //カウントの減算とオブジェクト消す
            GameData.randomGuestCnt--;
            Destroy(this.gameObject);

            return;
        }

        //待機時間更新
        fTimer += Time.deltaTime;
        if (data.waitTime <= fTimer)
        {
            ChangeTarget();
        }

        //動物の方を向く
        Quaternion rot = Quaternion.LookRotation(((!animals[targetNum]) ? data.rootTransforms[targetNum].position : animals[targetNum].position) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
    }

    public override void FinState()
    {
        if(agent)agent.stoppingDistance = 0.0f;
    }

    public override bool ErrorCheck()
    {
        if((data.rootTransforms == null) ? true : data.rootTransforms.Count <= 0)Debug.LogError("目的地のリストがありません");
        if (!agent)Debug.LogError("ナビメッシュエージェントが取得されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if (!guestAnimation) Debug.LogError("アニメーション制御用スクリプトが取得されていません");
        if (!player) Debug.LogWarning("プレイヤー用スクリプトが取得されていません");

        return ((data.rootTransforms == null) ? false : data.rootTransforms.Count > 0) && agent && (data!=null) && guestAnimation;
    }

    /// <summary>
    /// 目的地の変更
    /// </summary>
    public void ChangeTarget()
    {
        //ターゲットが1つ以下の場合は処理しない
        if(data.rootTransforms.Count <= 1 && !bChange) return;
        bChange = false;

        //ランダム生成されたものなら、ルートを最後まで回ったらエントランスに戻る
        if (data.isRandom && targetNum == data.rootTransforms.Count - 1) {
            agent.SetDestination(data.entranceTF.position);
        } else {
            targetNum = (targetNum + 1) % data.rootTransforms.Count;
            agent.SetDestination(data.rootTransforms[targetNum].position);
        }
        fTimer = 0.0f;
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WALK);
    }
    /// <summary>
    /// 動物の位置を取得
    /// </summary>
    public void GetAnimalsTransrom()
    {
        if (animals != null) return;

        animals = new List<Transform>();
        GameObject Object = GameObject.Find("GuestSharedObject");
        if (!Object) return;
        GuestSharedObject sharedObject = Object.GetComponent<GuestSharedObject>();
        if (!sharedObject) return;
        for(int i = 0; i < data.rootTransforms.Count; ++i){
            animals.Add(sharedObject.GetAnimalTransform(data.rootTransforms[i].name));
        }
    }
}
