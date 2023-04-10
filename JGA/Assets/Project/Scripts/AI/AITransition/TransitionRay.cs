//=============================================================================
// @File	: [TransitionRay.cs]
// @Brief	: 遷移条件　プレイヤーが視界に入ったか
// @Author	: Ogusu Yuuko
// @Editer	: Ichida Mai
// @Detail	: https://nekojara.city/unity-object-sight
// 
// [Date]
// 2023/03/05	スクリプト作成
// 2023/03/07	視界をRayから円錐に変更
// 2023/03/10	視界を位置を調整
// 2023/03/25	自動生成に対応
// 2023/04/10	視界の判定方法変更しました
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRay : AITransition
{
    private Transform playerTransform;
    private GuestData.Data data;
    [SerializeField,Tooltip("プレイヤーが視界から外れた時に遷移したい場合はチェックを入れてください")] private bool inv = false;
    [SerializeField] private Transform eyesPos; //目の位置
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
        if(data==null)data = GetComponent<AIManager>().GetGuestData();
        if (!playerTransform) playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    public override bool IsTransition()
    {
        if (!ErrorCheck()) return false;

        // ターゲットまでの向きと距離計算
        Vector3 targetDir = playerTransform.position - eyesPos.position;
        float targetDistance = targetDir.magnitude;
        float targetAngle = Vector3.Angle(transform.forward, targetDir);

        if ((targetAngle < data.viewAngle && targetDistance < data.rayLength) == inv) return false;

        //視界に入っていた場合プレイヤーに向かってRayを飛ばして、当たったら障害物に隠れていないので、trueを返す
        RaycastHit hit;
        //客からプレイヤーに向けて例を飛ばす
        Physics.Raycast(eyesPos.position, targetDir, out hit, data.rayLength);
        //プレイヤーと当たったか判定
        return (hit.collider.gameObject.tag == "Player") != inv;
    }

    public override bool ErrorCheck()
    {
        if (!eyesPos)Debug.LogError("目の位置が設定されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if (!playerTransform)Debug.LogError("プレイヤーのトランスフォームが取得されていません");

        return eyesPos && (data!=null) && playerTransform;
    }
}
