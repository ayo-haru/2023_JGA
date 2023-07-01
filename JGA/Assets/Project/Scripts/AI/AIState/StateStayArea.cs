//=============================================================================
// @File	: [StateStayArea.cs]
// @Brief	: 指定エリアで待機　　ペンギンエリア以外は使わないでね
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
// 2023/03/03	(小楠)終了処理追加
// 2023/03/08	(小楠)アニメーションの制御追加
// 2023/03/11	(小楠)目的地の方を向くようにした、目的地との距離を調整
// 2023/03/11	(小楠）navmeshagentの目的地をちょっとずらして、お客さんをばらけるようにした
// 2023/03/18	(小楠）動物の方向くようにした
// 2023/03/19	(小楠）ペンギンエリアに着いたときに客の人数のカウントを追加
// 2023/03/25	(伊地田）自動生成に対応
// 2023/03/30	(小楠）複数個所のペンギンエリアに対応
// 2023/04/10	(小楠）ナビメッシュが動かなくなってしまうバグを直した
// 2023/04/17	(小楠）ペンギンのTransform取得した
// 2023/05/23	(小楠）到着した時のアニメーションを増やした
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateStayArea : AIState
{
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //動物のTransform
    private Transform animal;
    //お客さん用データ
    private GuestData.Data data;
    //目的地に着いたか
    private bool isStay = false;
    //アニメーター
    private GuestAnimation guestAnimation;
    //アニメーションの秒数計測用
    private float fAnimTimer;
    //感情ui
    [SerializeField] private EmosionUI ui;

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
        //コンポーネント、データ取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();
        GetAnimalTransrom();
        //エラーチェック
        if (!ErrorCheck()) return;

        //ナビメッシュエージェントの設定
        //一番近い位置のペンギンエリアを目的地に設定する
        Vector3 nearPos = data.penguinTF[0].position;
        for(int i = 1; i < data.penguinTF.Count; ++i)
        {
            if (Vector3.Distance(gameObject.transform.position, nearPos) < Vector3.Distance(gameObject.transform.position,data.penguinTF[i].position)) continue;
            nearPos = data.penguinTF[i].position;
        }

        agent.SetDestination(nearPos + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f)));
        agent.speed = data.speed;
        agent.stoppingDistance = Random.Range(1.0f,data.cageDistance);

        //フラグ初期化
        isStay = false;

        //アニメーション初期化
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WALK);
        guestAnimation.SetLookAt(null);
        fAnimTimer = 0.0f;

        //ui設定
        ui.SetEmotion(EEmotion.NONE);

        //客人数のカウントから外すため、タグをデフォルトに戻す
        gameObject.tag = "Untagged";

        //ペンギンブースに着いた客の人数を追加
        GuestNumUI guestNumUI = GameObject.Find("GuestNumUI").GetComponent<GuestNumUI>();
        if (guestNumUI){
            guestNumUI.Add();
        }else{
            Debug.LogError("客人数表示用UIが取得できませんでした");
        }
    }

    public override void UpdateState()
    {
        //エラーチェック
        if (!ErrorCheck()) return;

        //驚きモーション中は移動させない
        agent.isStopped = (guestAnimation.GetAnimationState() == GuestAnimation.EGuestAnimState.SURPRISED);
        if (agent.isStopped) return;

        //ペンギンエリアについている場合の処理
        if (isStay)
        {
            //動物の方を向く
            Quaternion rot = Quaternion.LookRotation(((!animal) ? agent.destination : animal.position) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            //IDLEモーションの場合はタイマーを更新
            if (guestAnimation.GetAnimationState() != GuestAnimation.EGuestAnimState.IDLE) return;
            fAnimTimer -= Time.deltaTime;
            if (fAnimTimer > 0.0f) return;
            fAnimTimer = Random.Range(5.0f, 10.0f);
            //待機アニメーションの再生
            guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WATCH1 + Random.Range(0, 2));
            return;
        }

        //経路計算中
        if (agent.pathPending) return;

        //指定位置に移動中
        if (agent.remainingDistance > agent.stoppingDistance) return;

        //待機アニメーションの再生
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.IDLE);
        switch (Random.Range(0, 3))
        {
            case 1: guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WATCH1); break;
            case 2: guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.WATCH2); break;
        }

        //ui設定
        ui.SetEmotion(EEmotion.HIGH_TENSION);

        isStay = true;
    }

    public override void FinState()
    {
        if(agent)agent.stoppingDistance = 0.0f;
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!agent)Debug.LogError("ナビメッシュエージェントが取得されていません");
        if ((data.penguinTF == null) ? true : data.penguinTF.Count <= 0) Debug.LogError("待機位置が設定されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if (!guestAnimation) Debug.LogError("アニメーション制御用スクリプトが取得されていません");
        if (!ui) Debug.LogError("感情UIが設定されていません");

        return agent && ((data.penguinTF == null) ? false : data.penguinTF.Count > 0) && (data!=null) && guestAnimation && ui;
        
    }
    public void GetAnimalTransrom()
    {
        if (animal != null) return;

        GameObject Object = GameObject.Find("GuestSharedObject");
        if (!Object) return;
        GuestSharedObject sharedObject = Object.GetComponent<GuestSharedObject>();
        if (!sharedObject) return;
        animal = sharedObject.GetAnimalTransform(GameData.eRoot.PENGUIN_E);
    }
}
