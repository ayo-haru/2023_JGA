//=============================================================================
// @File	: [AITransitionAttensionAction.cs]
// @Brief	: 遷移条件 専用アクションをしたか
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/05	スクリプト作成
// 2023/03/06	(小楠)コントローラのエラー直した
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransitionAttensionAction : AITransition
{
    private GameObject player;

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
        if (!player) player = GameObject.FindWithTag("Player");
    }

    public override bool IsTransition()
    {
        //プレイヤーが専用アクションをしたかフラグを取得して返す

        Gamepad input = Gamepad.current;
        //仮で○ボタン押しっぱなしだったらtrueを返す
        return (input != null) ? input.buttonEast.IsPressed() : false || Input.GetKey(KeyCode.Space);
    }
}
