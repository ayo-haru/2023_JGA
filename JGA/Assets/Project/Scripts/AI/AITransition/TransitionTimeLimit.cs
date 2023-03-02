//=============================================================================
// @File	: [TransitionTimeLimit.cs]
// @Brief	: 遷移条件　一定時間たったら
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/03	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTimeLimit : AITransition
{
    [SerializeField,Min(0)] private float timeLimit = 1.0f;
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

    public override void InitTransition()
    {
        fTimer = 0.0f;
    }

    public override bool IsTransition()
    {
        fTimer += Time.deltaTime;

        return fTimer >= timeLimit;
    }
}
