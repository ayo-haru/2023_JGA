//=============================================================================
// @File	: [TransitionMegaphone.cs]
// @Brief	: 遷移条件　ペンギンが近くでメガホンを使用したら
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/21	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMegaphone : AITransition
{
    private Transform playerTransform;
    private Player player;
    private GuestData.Data data;

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
        //コンポーネント、オブジェクトの取得
        if (!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (!playerTransform && player) playerTransform = player.gameObject.transform;
        if (data == null) data = GetComponent<AIManager>().GetGuestData();
    }

    public override bool IsTransition()
    {
        if (!ErrorCheck()) return false;
        //メガホンフラグが立っているか
        if (!player.IsMegaphone) return false;
        //音が聞こえる範囲に居るか
        return Vector3.Distance(transform.position + transform.forward * data.soundAreaOffset, playerTransform.position) <= data.reactionArea;
    }

    public override bool ErrorCheck()
    {
        if (!player) Debug.LogError("プレイヤーのスクリプトが取得されていません");
        if (!playerTransform) Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (data == null) Debug.LogError("ゲスト用データが取得されていません");

        return player && playerTransform && (data != null);
    }
}
