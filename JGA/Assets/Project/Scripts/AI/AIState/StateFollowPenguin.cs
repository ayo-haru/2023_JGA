//=============================================================================
// @File	: [StateFollowPenguin.cs]
// @Brief	: ペンギン追いかける処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	(小楠)ペンギンと一定距離保てるようにした
// 2023/03/02	(小楠）データの取得方法変更
// 2023/03/03	(小楠）UI追加
// 2023/03/05	(小楠）UIの表示変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class StateFollowPenguin : AIState
{
    //ペンギン
    private GameObject player;
    //ペンギンのTransform
    private Transform target;
    private NavMeshAgent agent;
    [SerializeField] private EmosionUI ui;
    private GuestData data;

    private float fTimer = 0.0f;

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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();
        if (!player) player = GameObject.FindWithTag("Player");
        if (!target) target = player.GetComponent<Transform>();
        agent.SetDestination(target.position);
        agent.speed = data.speed;

        //UIの表示
        ui.SetEmotion(EEmotion.ATTENSION_HIGH);

        fTimer = 0.0f;
    }

    public override void UpdateState()
    {
        //ペンギンがアピールしているかどうかで、反応する距離やUIの変更を行う

        //客が反応したかどうか 専用アクションをしてる　かつ　反応できる範囲に居る
        if ((Gamepad.current.buttonEast.IsPressed() || Input.GetKey(KeyCode.Space)) && 
            Vector3.Distance(transform.position,target.position) <= data.reactionArea * ((int)ui.GetEmotion() / (float)EEmotion.ATTENSION_HIGH))
        {
            ui.SetEmotion(EEmotion.ATTENSION_HIGH);
            fTimer = 0.0f;
            agent.speed = data.speed;
        }else
        {
            //反応してないので停止する
            agent.speed = 0.0f;
            //感情の更新
            switch (ui.GetEmotion()) {
                case EEmotion.ATTENSION_HIGH:
                case EEmotion.ATTENSION_MIDDLE:
                    fTimer += Time.deltaTime;
                    float coolDownTime = ui.GetEmotion() == EEmotion.ATTENSION_HIGH ? data.firstCoolDownTime : data.secondCoolDownTime;
                    if(fTimer >= coolDownTime)
                    {
                        fTimer = 0.0f;
                        ui.SetEmotion(ui.GetEmotion() - 1);
                    }
                    break;
            }
        }




        //ペンギンとの距離が近い場合は移動しない
        if (agent.remainingDistance < data.distance) agent.speed = 0.0f;

        agent.SetDestination(target.position);
    }

    public override void FinState()
    {
        ui.SetEmotion(EEmotion.NONE);
    }
}
