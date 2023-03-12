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
// 2023/03/11   音の実装。
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardBox : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource sound;

    [SerializeField, Tooltip("吹っ飛ぶ強さ")]
    private float blowpower = 10.0f;    // 吹っ飛ぶ強さ
    [SerializeField, Tooltip("上方向のベクトル調整値")]
    private float topvector = 0.1f;    // 上方向のベクトル調整値
    private float objectsize;

    public bool IsSound { get; set; }   // 音が鳴ったフラグ

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        rb = GetComponent<Rigidbody>();
        sound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // オブジェクトのサイズを計算
        objectsize = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3;
    }


    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    private void FixedUpdate()
    {

    }

    private void Update()
    {
        // IsSoundフラグ切り替え
        IsSound = sound.isPlaying;

        // 動きが止まったら動かないようにする
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
                rb.velocity = vec * blowpower * objectsize; // オブジェクトのサイズによって飛ばす力を変える
                vec = (transform.position - other.transform.position).normalized;
                rb.AddTorque(vec * blowpower * objectsize);
                // ここで音を鳴らすよ
                sound.Play();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // はたかれてから止まるまでの間にオブジェクトにぶつかったら音を鳴らす
        if(!rb.IsSleeping())
        {
            // ここで音を鳴らすよ
            sound.Play();
        }
    }
}
