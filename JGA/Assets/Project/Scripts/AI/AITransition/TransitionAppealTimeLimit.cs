//=============================================================================
// @File	: [TransitionAppealTimeLimit.cs]
// @Brief	: 遷移条件　感情が最低になってから一定時間アピールがなかったら
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

public class TransitionAppealTimeLimit : AITransition
{
    //制限時間
    [SerializeField] private float timeLimit = 1.0f;
    //時間計測用
    private float fTimer = 0.0f;
    //反転用フラグ
    [SerializeField, Tooltip("一定時間アピールしていたら遷移したいときはチェック入れてください")] bool inv = false;
    //プレイヤー用スクリプト
    private Player player;
    //感情ui
    [SerializeField]private EmosionUI ui;
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

    public override void InitTransition()
    {
        if (!player) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        fTimer = 0.0f;
    }

    public override bool IsTransition()
    {
        if (!ErrorCheck()) return false;

        //タイマーの更新
        fTimer = (player.IsAppeal == inv && (ui.GetEmotion() == (inv ? EEmotion.ATTENSION_HIGH : EEmotion.ATTENSION_LOW))) ? fTimer + Time.deltaTime : 0.0f;

        return fTimer >= timeLimit;

    }

    public override bool ErrorCheck()
    {
        if (!player)Debug.LogError("プレイヤーのスクリプトが取得されていません");
        if (!ui) Debug.LogError("感情UIが設定されていません");

        return player && ui;
    }
}
