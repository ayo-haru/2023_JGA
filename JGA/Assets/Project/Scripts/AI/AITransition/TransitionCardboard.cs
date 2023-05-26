//=============================================================================
// @File	: [TransitionCardboard.cs]
// @Brief	: 遷移条件　段ボールの音が鳴ったら
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCardboard : AITransition
{
    private Transform playerTransform;
    private GuestData.Data data;
    private List<BaseObj> interactObjecs;

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
        if (!playerTransform) playerTransform = GameObject.FindWithTag("Player").transform;
        if (data == null) data = GetComponent<AIManager>().GetGuestData();
        if (interactObjecs != null) return;

        interactObjecs = new List<BaseObj>();
        GameObject Object = GameObject.Find("GuestSharedObject");
        if (!Object) return;
        GuestSharedObject sharedObject = Object.GetComponent<GuestSharedObject>();
        if (!sharedObject) return;
        interactObjecs = sharedObject.GetCarboardObjects();
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        //プレイヤーが範囲内に居るか
        if (Vector3.Distance(transform.position, playerTransform.position) > data.reactionArea) return false;

        //範囲内にあるインタラクトオブジェクトのフラグが立っているか
        for (int i = 0; i < interactObjecs.Count; ++i)
        {
            if (Vector3.Distance(transform.position + transform.forward * data.soundAreaOffset, interactObjecs[i].transform.position) > data.reactionArea) continue;
            if (interactObjecs[i].GetisPlaySound()) return true;
        }
        return false;
    }

    public override bool ErrorCheck()
    {
        if (!playerTransform) Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (data == null) Debug.LogError("ゲスト用データが取得されていません");
        if ((interactObjecs == null) ? true : interactObjecs.Count <= 0) Debug.LogWarning("インタラクトオブジェクトがありません");

        return playerTransform && (data != null) && ((interactObjecs == null) ? false : interactObjecs.Count > 0);
    }
}
