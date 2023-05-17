//=============================================================================
// @File	: [StateSit.cs]
// @Brief	: 座ってるときの処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/17	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateSit : AIState
{
    //アニメーション
    private GuestAnimation guestAnimation;
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
        //コンポーネント取得
        if (!guestAnimation) guestAnimation = GetComponent<GuestAnimation>();

        if (!ErrorCheck()) return;

        //アニメーション設定
        guestAnimation.SetAnimation(GuestAnimation.EGuestAnimState.SIT_IDLE);
        guestAnimation.SetLookAt(null);

        //感情UI設定
        ui.SetEmotion(EEmotion.HIGH_TENSION);
    }

    public override void UpdateState()
    {
        //特になし
    }

    public override void FinState()
    {
        if (ui) ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!guestAnimation) Debug.LogError("アニメーション制御用のスクリプトが取得されていません");
        if (!ui) Debug.LogError("感情UIが設定されていません");

        return guestAnimation && ui;
    }
}
