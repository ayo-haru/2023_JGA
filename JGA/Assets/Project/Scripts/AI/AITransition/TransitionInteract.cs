//=============================================================================
// @File	: [TransitionInteract.cs]
// @Brief	: 遷移条件　ペンギンが近くにいてかつインタラクトしたら
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransitionInteract : AITransition
{
    private GameObject player;
    private Transform playerTransform;
    private GuestData data;

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
        if (!playerTransform && player) playerTransform = player.GetComponent<Transform>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();
    }

    public override bool IsTransition()
    {
        if(!player || !playerTransform || !data)
        {
            Debug.LogError("データが取得されていません");
            return false;
        }
        //プレイヤーがインタラクトをしたかフラグを取得して返す
        //仮で□ボタンかFキー押しっぱなしだったらtrueを返す

        //プレイヤーが範囲内に居るか
        if (Vector3.Distance(transform.position, playerTransform.position) > data.reactionArea) return false;

        //インタラクトされたか
        Gamepad input = Gamepad.current;
        return (input != null) ? input.buttonWest.IsPressed() : false || Input.GetKey(KeyCode.F);
    }
}
