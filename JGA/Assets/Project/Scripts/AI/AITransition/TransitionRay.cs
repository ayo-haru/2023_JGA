//=============================================================================
// @File	: [TransitionRay.cs]
// @Brief	: 遷移条件　プレイヤーが視界に入ったか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: https://nekojara.city/unity-object-sight
// 
// [Date]
// 2023/03/05	スクリプト作成
// 2023/03/07	視界をRayから円錐に変更
// 2023/03/10	視界を位置を調整
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionRay : AITransition
{
    private Transform playerTransform;
    private GuestData data;
    [SerializeField,Range(0,360),Tooltip("視線の向き")] private float angle = 45.0f;
    [SerializeField, Range(0, 180), Tooltip("視野角")] private float viewAngle = 45.0f;
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
        if(!data)data = GetComponent<AIManager>().GetGuestData();
        if (!playerTransform) playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    public override bool IsTransition()
    {
        if (!ErrorCheck()) return false;

        //プレイヤーが視界内に入っているか
        // 視線の向き
        Vector3 dir = eyesPos.forward;
        dir.y -= angle / 360.0f;

        // ターゲットまでの向きと距離計算
        Vector3 targetDir = playerTransform.position - eyesPos.position;
        float targetDistance = targetDir.magnitude;

        // cos(θ/2)を計算
        float cosHalf = Mathf.Cos(viewAngle / 2 * Mathf.Deg2Rad);

        // 自身とターゲットへの向きの内積計算
        // ターゲットへの向きベクトルを正規化する必要があることに注意
        float innerProduct = Vector3.Dot(dir, targetDir.normalized);


        //Rayの可視化
        Vector3 pos = eyesPos.position;
        Debug.DrawRay(pos, dir * 10, Color.red, 1.0f / 60.0f);

        // 視界判定 
        if ((innerProduct > cosHalf && targetDistance < data.rayLength) == inv) return false;

        //視界に入っていた場合プレイヤーに向かってRayを飛ばして、当たったら障害物に隠れていないので、trueを返す
        RaycastHit hit;

        //客からプレイヤーに向けて例を飛ばす
        Physics.Raycast(pos, targetDir, out hit, data.rayLength);

        //プレイヤーと当たったか判定
        return (hit.collider.gameObject.tag == "Player") != inv;
    }

    public override bool ErrorCheck()
    {
        bool bError = true;
        if (!eyesPos)
        {
            Debug.LogError("目の位置が設定されていません");
            bError = false;
        }
        if (!data)
        {
            Debug.LogError("ゲスト用データが取得されていません");
            bError = false;
        }
        if (!playerTransform)
        {
            Debug.LogError("プレイヤーのトランスフォームが取得されていません");
            bError = false;
        }
        return bError;
    }
}
