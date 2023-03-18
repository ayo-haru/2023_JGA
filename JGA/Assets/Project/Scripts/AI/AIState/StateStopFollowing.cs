//=============================================================================
// @File	: [StateStopFollowing.cs]
// @Brief	: 追従やめる時の処理　立ち止まって？出す
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/18	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStopFollowing : AIState
{
    //アニメーター
    private Animator animator;
    //感情UI
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
        if (!animator) animator = GetComponent<Animator>();

        if (!ErrorCheck()) return;

        //アニメーション初期化
        animator.SetBool("isWalk", false);

        //ui設定
        ui.SetEmotion(EEmotion.QUESTION);
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
        bool bError = true;

        if (!animator)
        {
            Debug.LogError("アニメーターが取得されていません");
            bError = false;
        }
        if (!ui)
        {
            Debug.LogError("感情UIが設定されていません");
            bError = false;
        }
        return bError;
    }
}
