//=============================================================================
// @File	: [TransitionInteract.cs]
// @Brief	: 遷移条件　ペンギンが近くにいてかつインタラクトしたら
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/07	スクリプト作成
// 2023/03/13	(小楠)インタラクトフラグ取得
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteract : AITransition
{
    private GameObject player;
    private Transform playerTransform;
    private GuestData data;
    private GameObject[] interactObjecs;

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
        //コンポーネント、オブジェクトの取得
        if (!player) player = GameObject.FindWithTag("Player");
        if (!playerTransform && player) playerTransform = player.GetComponent<Transform>();
        if (!data) data = GetComponent<AIManager>().GetGuestData();
        if (interactObjecs == null) interactObjecs = GameObject.FindGameObjectsWithTag("Interact");
    }

    public override bool IsTransition()
    {
        #region エラーチェック
        if (!player || !playerTransform || !data)
        {
            Debug.LogError("データが取得されていません");
            return false;
        }
        if((interactObjecs == null) ? true : interactObjecs.Length <= 0)
        {
            Debug.LogError("インタラクトオブジェクトがありません");
            return false;
        }
        #endregion

        //プレイヤーが範囲内に居るか
        if (Vector3.Distance(transform.position, playerTransform.position) > data.reactionArea) return false;

        //範囲内にあるインタラクトオブジェクトのフラグが立っているか
        for(int i = 0; i < interactObjecs.Length; ++i)
        {
            if (Vector3.Distance(transform.position, interactObjecs[i].transform.position) > data.reactionArea) continue;
            CardboardBox cardboardBox = interactObjecs[i].GetComponent<CardboardBox>();
            if (!cardboardBox) continue;
            if (cardboardBox.IsSound) return true;
        }
        return false;
    }
}
