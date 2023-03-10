//=============================================================================
// @File	: [CardboardBox.cs]
// @Brief	: インタラクトオブジェクトである段ボールの処理
// @Author	: KAIKAWA KOYO
// @Editer	: KAIKAWA KOYO
// @Detail	: 
// 
// [Date]
// 2023/03/07	スクリプト作成
// 2023/03/10   はたかれる以外で動かないように。音鳴ったフラグの作成。
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardBox : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField, Tooltip("吹っ飛ぶ強さ")]
    private float blowpower = 10.0f;    // 吹っ飛ぶ強さ
    [SerializeField, Tooltip("上方向のベクトル調整値")]
    private float topvector = 0.1f;    // 吹っ飛ぶ強さ

    public bool IsSound { get; set; }   // 音が鳴ったフラグ
    private bool delay;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 音の素材が用意出来たらAoudioSourceの終了フラグを使ってIsSoundを切り替えたい
        if (IsSound)
        {
            if (!delay)
                delay = true;
            else
            {
                delay = false;
                IsSound = false;
            }
        }

        if (!rb.isKinematic && rb.IsSleeping())
        {
            rb.isKinematic = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // プレイヤーが範囲内にいる時にインタラクトフラグがTrueになったらふき飛ぶよ
        if (other.gameObject.tag == "Player")
        {
            if(other.GetComponent<Player>().IsInteract)
            {
                rb.isKinematic = false;
                Vector3 vec = (transform.position + new Vector3(0.0f, topvector, 0.0f) - other.transform.position).normalized;
                rb.velocity = vec * blowpower;
                rb.AddTorque(vec * blowpower / 2);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // はたかれてから止まるまでの間にオブジェクトにぶつかったら音を鳴らす
        if(rb.IsSleeping())
        {
            Debug.Log("音鳴ったよ");
            IsSound = true;
        }
    }
}
