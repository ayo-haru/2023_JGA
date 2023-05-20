//=============================================================================
// @File	: [StateGimmickAnimal.cs]
// @Brief	: ギミック動物に夢中になってるときの処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/05	スクリプト作成
// 2023/05/10	動物の方向を取得
// 2023/05/20	要らないコメント消したりした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateGimmickAnimal : AIState
{
    //お客さん用のデータ
    private GuestData.Data data;
    //ペンギン用スクリプト
    private Player player;
    //animator
    private GuestAnimation guestAnimation;
    //感情ui
    [SerializeField] private EmosionUI ui;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //注目する動物
    [SerializeField] private MySceneManager.eRoot animal = MySceneManager.eRoot.POLARBEAR;
    //動物のTransform
    private Transform animalTransform;
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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();
        if (!player) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (data == null) data = GetComponent<AIManager>().GetGuestData();
        //ケージポス（agentの目的地）取得
        GetCagePos();
        //動物のトランスフォーム取得
        GetAnimal();

        if (!ErrorCheck()) return;

        //感情UIの設定
        ui.SetEmotion(EEmotion.HIGH_TENSION);
        //アニメーションの設定
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WALK);
        guestAnimation.SetLookAt(null);
        //ナビメッシュエージェントの設定
        agent.speed = (player) ? player.MaxAppealSpeed * data.followSpeed * data.inBoothSpeed : data.speed;
        agent.stoppingDistance = Random.Range(1, data.cageDistance);
    }

    public override void UpdateState()
    {
        if (!ErrorCheck()) return;
        if (agent.pathPending) return;//経路計算中

        //速度に応じてアニメーションを切り替え
        guestAnimation.SetAnimation((agent.velocity.magnitude > 0.0f) ? GuestAnimation.EGuestAnimState.WALK : GuestAnimation.EGuestAnimState.IDLE);

        //目的地に移動中
        if (agent.remainingDistance > agent.stoppingDistance) return;

        //動物の方を見る
        Quaternion rot = Quaternion.LookRotation(((!animalTransform) ? agent.destination : animalTransform.position) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
    }

    public override void FinState()
    {
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!ui) Debug.LogError("感情UIが設定されていません");
        if (!agent) Debug.LogError("ナビメッシュエージェントが取得されていません");
        if (!guestAnimation) Debug.LogError("アニメーション制御用のスクリプトが取得されていません");
        if (data == null) Debug.LogError("お客さん用のデータが取得されていません");
        if (!player) Debug.LogError("プレイヤー用スクリプトが取得されていません");
        if (!animalTransform) Debug.LogWarning("動物が取得されていません");

        return ui && agent && guestAnimation && (data != null) && player;
    }

    private bool GetAnimal()
    {
        if (animalTransform != null) return false;

        GameObject Object = GameObject.Find("GuestSharedObject");
        if (!Object) return false;
        GuestSharedObject sharedObject = Object.GetComponent<GuestSharedObject>();
        if (!sharedObject) return false;
        animalTransform = sharedObject.GetAnimalTransform(animal);
        return animalTransform;
    }
    private bool GetCagePos()
    {
        if (!agent) return false;
        StageSceneManager manager = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
        if (!manager) return false;
        //ケージポス取得
        Vector3 pos = manager.GetRootTransform(animal).position;
        //ばらけさせる
        pos.x += Random.Range(-10.0f, 10.0f);
        pos.z += Random.Range(-10.0f, 10.0f);

        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 10.0f, NavMesh.AllAreas)) return false;

        //ケージポスを目的地に設定
        agent.SetDestination(hit.position);

        return true;
    }

    public Transform GetTargetAnimal()
    {
        return animalTransform;
    }
}
