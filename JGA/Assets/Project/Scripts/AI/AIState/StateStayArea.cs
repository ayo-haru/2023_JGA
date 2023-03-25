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
    private Animator animator;
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
        if (!animator) animator = GetComponent<Animator>();
        GetAnimalTransrom();
        //エラーチェック
        if (!ErrorCheck()) return;

        //ナビメッシュエージェントの設定
        agent.SetDestination(data.penguinTF.position + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f)));
        agent.speed = data.speed;
        agent.stoppingDistance = Random.Range(1.0f,data.cageDistance);

        isStay = false;

        //アニメーション初期化
        animator.SetBool("isWalk", true);

        //ui設定
        ui.SetEmotion(EEmotion.NONE);
    }

    public override void UpdateState()
    {
        //エラーチェック
        if (!ErrorCheck()) return;

        if (isStay)
        {
            //動物の方を向く
            Quaternion rot = Quaternion.LookRotation(((!animal) ? data.penguinTF.position : animal.position) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            return;
        }

        if (agent.pathPending) return;
        //指定位置に着いたら
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //待機アニメーションの再生
            animator.SetBool("isWalk", false);
            //ui設定
            ui.SetEmotion(EEmotion.HIGH_TENSION);
            isStay = true;

            //ペンギンブースに着いた客の人数を追加
            GuestNumUI guestNumUI = GameObject.Find("GuestNumUI").GetComponent<GuestNumUI>();
            if (guestNumUI){
                guestNumUI.Add();
            }else{
                Debug.LogError("客人数表示用UIが取得できませんでした");
            }
        }
    }

    public override void FinState()
    {
        if(agent)agent.stoppingDistance = 0.0f;
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!agent)Debug.LogError("ナビメッシュエージェントが取得されていません");
        if (!data.penguinTF) Debug.LogError("待機位置が設定されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if (!animator)Debug.LogError("アニメーターが取得されていません");
        if (!ui) Debug.LogError("感情UIが設定されていません");

        return agent && data.penguinTF && (data!=null) && animator && ui;
    }
    public void GetAnimalTransrom()
    {
        if (animal != null) return;

        //動物の名前から動物の親オブジェクトを取得
        int index = data.penguinTF.name.IndexOf("CagePos");
        if (index < 0) return;
        GameObject obj = GameObject.Find(data.penguinTF.name.Substring(0, index));
        if ((!obj) ? true : obj.transform.childCount <= 0) return;
        //子オブジェクトの中からランダムで1つ動物をanimalsに格納
        animal = obj.transform.GetChild(Random.Range(0, obj.transform.childCount));   
    }
}
