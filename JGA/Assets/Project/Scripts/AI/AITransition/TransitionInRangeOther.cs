//=============================================================================
// @File	: [TransitionInRangeOther.cs]
// @Brief	: 遷移条件　ターゲット(プレイヤー以外)が範囲内に居るか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: プレイヤーが範囲内に居るか判定したい場合はTransitionInRangePlayer.csを使ってください
// 
// [Date]
// 2023/03/06	スクリプト作成
// 2023/03/30	複数のぺんぎんエリアに対応
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInRangeOther : AITransition
{
    //ゲスト用データ
    private GuestData.Data data;
    //遷移条件反転用フラグ
    [SerializeField,Tooltip("ターゲットが範囲外に出たら遷移したい場合はチェックを入れてください")] private bool inv = false;
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
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        for(int i = 0; i < data.penguinTF.Count; ++i)
        {
            if ((Vector3.Distance(gameObject.transform.position, data.penguinTF[i].position) <= data.reactionArea) != inv) return true;
        }

        return false;
    }

    public override bool ErrorCheck()
    {
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if ((data.penguinTF == null) ? true : data.penguinTF.Count <= 0) Debug.LogError("目的地のトランストームが設定されていません");

        return (data!=null) && (data.penguinTF == null) ? false : data.penguinTF.Count > 0;
    }
}
