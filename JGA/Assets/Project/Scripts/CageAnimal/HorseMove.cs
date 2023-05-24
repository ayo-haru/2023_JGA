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
    [SerializeField] private Animator animator;
    //private AudioSource audioSource;
    private Rigidbody rb;
    [SerializeField] private List<Transform> rootPos;
    public float horseSpeed;    // スピード
    public float stopTime;      // その場にとどまる時間
    private float nowTime = 0.0f;   // 経過時間
    private Vector3 target;     // 目的地
    private float speed;        // 速さ
    private bool bMove = true;
    private bool bEat = false;
    private GameObject basKet;
    [SerializeField] private Transform basketPos;
    private int nCarrot = -1;

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
        if (animator == null) animator = GetComponent<Animator>();
        //if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        if (basKet == null) basKet = GameObject.Find("BasKet 1_001");
        if (rootPos.Count <= 0)
        {
            Debug.LogWarning("ルートがありません。(Hourse.cpp)");
        }
        else
        {
            RandPos();
        }
        if (basketPos == null)
            Debug.LogWarning("バスケットの座標がありません。 (Hourse.cpp)");
        if (!basKet.GetComponent<HourseBasKet>())
            Debug.LogWarning("バスケットに HourseBasKet.cpp をつけてください。");
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

        if (rootPos.Count <= 0) return;


        Dir();
        if (bMove)
        {
            Move();
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
        if (!basKet.GetComponent<HourseBasKet>())
        {
            int targetPos = Random.Range(0, rootPos.Count);
            target.x = rootPos[targetPos].transform.localPosition.x;
            target.y = transform.localPosition.y;
            target.z = rootPos[targetPos].transform.localPosition.z;
        }
        else
        {
            for (int i = 0; i < basKet.GetComponent<HourseBasKet>().carrot.Count; i++)
            {
                if (!basKet.GetComponent<HourseBasKet>().bBasket[i])
                {
                    // バスケットの中ににんじんがなかったらランダムに移動する
                    int targetPos = Random.Range(0, rootPos.Count);
                    target.x = rootPos[targetPos].transform.localPosition.x;
                    target.y = transform.localPosition.y;
                    target.z = rootPos[targetPos].transform.localPosition.z;
                }
                else
                {
                    // バスケットに移動する
                    if (basketPos == null) return;
                    //Debug.Log("バスケットに移動");
                    target.x = basketPos.transform.localPosition.x;
                    target.y = transform.localPosition.y;
                    target.z = basketPos.transform.localPosition.z;
                }
            }
        }
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
            AnimPlay();
        }
        // 到着したら
        if (nowPos >= 1)
        {
            // 動きストップ
            bMove = false;
            if (basKet.GetComponent<HourseBasKet>())
            {
                for (int i = 0; i < basKet.GetComponent<HourseBasKet>().carrot.Count; i++)
                {
                    if (basKet.GetComponent<HourseBasKet>().bBasket[i])
                    {
                        if (!bEat) bEat = true;
                        if (target.x == basketPos.transform.localPosition.x &&
                            target.z == basketPos.transform.localPosition.z)
                        {
                            target.x = basKet.transform.localPosition.x - (-41.0f);
                            target.z = basKet.transform.localPosition.z - 40.0f;
                            nCarrot = i;
                        }
                    }
                }
            }
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            AnimPlay();
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                targetRotation, 120.0f * Time.deltaTime);
        }
    }

    /// <summary>
    /// アニメーション処理
    /// </summary>
    private void AnimPlay()
    {
        if (bMove)
        {
            if (!animator.GetBool("isWalk"))
                animator.SetBool("isWalk", true);
        }
        else
        {
            if (animator.GetBool("isWalk"))
                animator.SetBool("isWalk", false);
        }
        if(bEat)
        {
            if (!animator.GetBool("isEat"))
                animator.SetBool("isEat", true);
        }
        else
        {
            if (animator.GetBool("isEat"))
                animator.SetBool("isEat", false);
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
            if (bEat) bEat = false;
            if (basKet.GetComponent<HourseBasKet>())
            {
                if (basKet.GetComponent<HourseBasKet>().bBasket[nCarrot])
                    basKet.GetComponent<HourseBasKet>().bBasket[nCarrot] = false;
            }
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
        animator.speed = 0.0f;
        // 足音ストップ
        //audioSource.Stop();
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    private void Resumed()
    {
        // アニメーション
        animator.speed = 1.0f;
    }
}
