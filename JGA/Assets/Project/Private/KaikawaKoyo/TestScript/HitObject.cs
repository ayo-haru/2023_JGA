//=============================================================================
// @File	: [HitObject.cs]
// @Brief	: はたかれるオブジェクト全般の処理
// @Author	: KAIKAWA KOYO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HitObject : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField, Tooltip("吹っ飛ぶ強さ")]
    public float blowpower = 13.0f;    // 吹っ飛ぶ強さ
    [SerializeField, Tooltip("上方向のベクトル調整値")]
    public float topvector = 1.0f;    // 上方向のベクトル調整値

    // ポーズ時の値保存用
    private Vector3 pauseVelocity;
    private Vector3 pauseAngularVelocity;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

        rb = GetComponent<Rigidbody>();
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

    // ポーズ処理
    private void Pause()
    {
        pauseVelocity = rb.velocity;
        pauseAngularVelocity = rb.angularVelocity;
        rb.isKinematic = true;
    }

    private void Resumed()
    {
        rb.velocity = pauseVelocity;
        rb.angularVelocity = pauseAngularVelocity;
        rb.isKinematic = false;
    }

 
}
