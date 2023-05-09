//=============================================================================
// @File	: [HorseMove.cs]
// @Brief	: 馬動かす
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/17	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class HorseMove : MonoBehaviour
{
    //[SerializeField] private Animator animator;
    //private AudioSource audioSource;
    private Rigidbody rb;
    public float horseSpeed;    // スピード
    public float stopTime;      // その場にとどまる時間
    private float nowTime = 0.0f;   // 経過時間
    private Vector3 target;     // 目的地
    private float speed;        // 速さ
    private bool bMove = true;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        // ポーズ時の動作を登録
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        //if (animator == null) animator = GetComponent<Animator>();
        //if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        RandPos();
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (PauseManager.isPaused && !MySceneManager.GameData.isCatchPenguin)
        {
            return;
        }

        if (bMove)
        {
            Move();
            Dir();
        }
        else
        {
            Timer();
        }
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (PauseManager.isPaused && !MySceneManager.GameData.isCatchPenguin)
        {
            return;
        }

        if (MySceneManager.GameData.isCatchPenguin)
        {
            // ペンギンが捕まっているときはポーズ中なのでポーズ中でも処理を行う
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        speed = horseSpeed * Time.deltaTime;
        SetPos(speed);
    }

    /// <summary>
    /// ランダムで目的地変更
    /// </summary>
    private void RandPos()
    {
        target.x = Random.Range(183, 200);
        target.y = transform.localPosition.y;
        target.z = Random.Range(-175, -158);
    }

    /// <summary>
    /// 目的地に移動
    /// </summary>
    private void SetPos(float _speed)
    {
        var distance = Vector3.Distance(transform.localPosition, target);
        var nowPos = _speed / distance;
        // 到着していなかったら
        if(nowPos >= 0)
        {
            // 目的地に移動
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, nowPos);
        }
        // 到着したら
        if(nowPos >= 1)
        {
            // 動きストップ
            bMove = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // 目的地変更
            Timer();
        }
    }

    /// <summary>
    /// 馬の向き変更
    /// </summary>
    private void Dir()
    {
        // 次に目指すべき位置を取得
        var nextPoint = target;
        Vector3 targetDir = nextPoint - transform.localPosition;

        // ゼロベクトルじゃなかったら
        if (targetDir != Vector3.zero)
        {
            // その方向に向けて旋回する(120度/秒)
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 120.0f * Time.deltaTime);

            // 自分の向きと次の位置の角度差が30度以上の場合、その場で旋回
            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < 30.0f)
            {
            }
        }
    }

    /// <summary>
    /// 時間処理
    /// </summary>
    private void Timer()
    {
        nowTime += Time.deltaTime;
        if (nowTime > stopTime)
        {
            RandPos();
            nowTime = 0.0f;
            bMove = true;
        }
    }

    /// <summary>
    /// ポーズ開始
    /// </summary>
    private void Pause()
    {
        // 動きストップ
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // アニメーション
        //animator.speed = 0.0f;
        // 足音ストップ
        //audioSource.Stop();
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    private void Resumed()
    {
        // アニメーション
        //animator.speed = 1.0f;
    }
}