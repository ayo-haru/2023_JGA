//=============================================================================
// @File	: [TransitionRay.cs]
// @Brief	: 遷移条件　プレイヤーが視界に入ったか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/05	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRay : AITransition
{
    private GuestData data;
    [SerializeField,Range(0,360)] private float angle = 45.0f;
    [SerializeField,Tooltip("プレイヤーが視界から外れた時に遷移したい場合はチェックを入れてください")] private bool inv = false;

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
        if(!data)data = GetComponent<AIManager>().GetGuestData();
    }

    public override bool IsTransition()
    {
        RaycastHit hit;
        Vector3 pos = transform.position;
        Vector3 direction = transform.forward;

        //お客さんの位置からRayを出すと、お客さんのColliderに当たってしまうので、少しずらしてる
        pos += direction * 1;

        //斜め下に傾ける
        direction.y -= angle / 360.0f;

        //Rayの可視化
        Debug.DrawRay(pos, direction * 10, Color.red, 1.0f / 60.0f);

        //何も当たらなかった場合
        if (!Physics.Raycast(pos, direction, out hit, data.rayLength)) return inv;

        //プレイヤーと当たったか判定
        return (hit.collider.gameObject.tag == "Player") != inv;
    }
}
