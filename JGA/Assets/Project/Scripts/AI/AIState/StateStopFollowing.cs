//=============================================================================
// @File	: [StateStopFollowing.cs]
// @Brief	: 追従やめる時の処理　立ち止まって？出す
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/18	スクリプト作成
// 2023/03/24	お客さんの移動速度０にした
// 2023/05/15	アニメーション制御方法を変更
// 2023/05/20	要らないコメント消した
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateStopFollowing : AIState
{
    //アニメーター
    private GuestAnimation guestAnimation;
    //感情UI
    [SerializeField] private EmosionUI ui;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
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
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();
        if (!agent) agent = GetComponent<NavMeshAgent>();

        if (!ErrorCheck()) return;

        //アニメーション初期化
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.IDLE);
        guestAnimation.SetLookAt(null);

        //ui設定
        ui.SetEmotion(EEmotion.QUESTION);

        //追従停止
        agent.isStopped = true;
    }

    public override void UpdateState()
    {
        //特になし
    }

    public override void FinState()
    {
        if (ui) ui.SetEmotion(EEmotion.NONE);
        if (agent) agent.isStopped = false;
    }

    public override bool ErrorCheck()
    {
        if (!guestAnimation) Debug.LogError("アニメーション制御用スクリプトが取得されていません");
        if (!ui)Debug.LogError("感情UIが設定されていません");
        if (!agent) Debug.LogError("ナビメッシュエージェントが取得されていません");

        return guestAnimation && ui && agent;
    }
}
