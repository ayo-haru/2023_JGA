//=============================================================================
// @File	: [ZooKeeper.cs]
// @Brief	: 飼育員のNavMeshを使ってない処理
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;

public class ZooKeeper : MonoBehaviour
{
    private enum Effect
    {
        exclamation,    // ！エフェクト
        question,       // ？エフェクト   
    }
    private GameObject exclamationEffect;   // ！エフェクト
    private GameObject questionEffect;      // ？エフェクト

    [SerializeField]
    private ZooKeeperData.Data data;    // 飼育員用の外部で設定できるパラメーターたち
    [SerializeField] private Animator animator;
    private AudioSource audioSource;
    private RaycastHit rayhit;
    private SphereCollider sphereCollider;
    private Vector3 startPos;       // 初期位置
    private Quaternion startDir;    // 初期回転
    [SerializeField] private bool chaseNow = false;
    private bool moveFlg = false;
    private bool soundObjFlg = false;
    private bool surpriseFlg = true;
    private GameObject targetObj;
    private Vector3 target;     // 目的地
    private float speed;        // 速さ
    private Player player;


    /// <summary>
    /// Inspectorから自身の値が変更されたときに自動で呼ばれる
    /// </summary>
    private void OnValidate()
    {
        sphereCollider = this.GetComponent<SphereCollider>();
        /*
         * 最初からシーン上に配置しないので一回コメントアウトさせていただいた（伊地田）
         */
        sphereCollider.radius = data.search; // colliderのradiusを変更する
    }

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
        if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (player == null) player = GameObject.FindWithTag("Player").GetComponent<Player>();

        startPos = this.transform.position;     // 初期位置を取得
        startDir = this.transform.rotation;     // 初期回転を取得
        AnimPlay(1);
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

        if (moveFlg)
        {
            Move();
            Dir();
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
            ReStart();
        }
    }

    #region ペンギンを捕まえた時の処理
    /// <summary>
    /// 飼育員とペンギンの当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            MySceneManager.GameData.isCatchPenguin = true;
        }
    }
    #endregion

    #region 索敵範囲内の処理
    /// <summary>
    /// 飼育員の索敵範囲
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        // 音がなってるか
        if (other.GetComponent<BaseObj>())
        {
            if (other.GetComponent<BaseObj>().GetisPlaySound())
            {
                soundObjFlg = true;
                Walk();
                targetObj = other.gameObject;
                target = other.transform.position;
            }
        }

        if (!soundObjFlg && other.gameObject.tag == "Player")
        {
            var pos = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(this.transform.forward, pos);
            Debug.DrawRay(new Vector3(transform.position.x, 2.0f, transform.position.z), 
                pos * data.searchDistance, Color.red);

            // 視界の角度内に収まってRayが当たっているか
            if (targetAngle < data.searchAngle && RayHit(pos) == 1)
            {
                if (!chaseNow)
                {
                    // コルーチン開始
                    StartCoroutine("PenguinChase");
                }
            }
        }
    }
    #endregion

    #region 移動処理
    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        // 音がなってるか
        if (soundObjFlg)
        {
            target = targetObj.transform.position;
            speed = (data.speed * player.MaxMoveSpeed) * Time.deltaTime;
            SetPos(target, speed);
        }

        // 元の位置に戻る
        if(!soundObjFlg && !chaseNow)
        {
            speed = (data.speed * player.MaxMoveSpeed) * Time.deltaTime;
            SetPos(target, speed);
        }

        // ペンギンを追従
        if (chaseNow && !soundObjFlg)
        {
            target = player.transform.position;
            speed = (data.speed * player.MaxRunSpeed) * Time.deltaTime;
            SetPos(target, speed);
        }
    }
    #endregion

    #region 目的地に移動処理
    /// <summary>
    /// 目的地に移動処理
    /// </summary>
    private void SetPos(Vector3 _target, float _speed)
    {
        if (moveFlg)
        {
            var distance = Vector3.Distance(transform.position, _target);   // 距離
            var nowPos = _speed / distance;     // 現在の位置
            // 到着していなかったら
            if (nowPos >= 0)
            {
                // 移動する
                AnimPlay(0);
                transform.position = Vector3.Lerp(transform.position, _target, nowPos);
            }
            // 到着したら
            if (nowPos >= 1)
            {
                Stop();
                AnimPlay(1);
                // 音が鳴ったオブジェクトに到着
                if (soundObjFlg)
                {
                    // コルーチン開始
                    StartCoroutine("SoundObj");
                }
                else  // 初期位置に到着
                {
                    this.transform.rotation = startDir;
                }
            }
        }
    }
    #endregion

    #region 飼育員の向きを変える処理
    /// <summary>
    /// 向きの調整
    /// </summary>
    private void Dir()
    {
        // 次に目指すべき位置を取得
        var nextPoint = target;
        Vector3 targetDir = nextPoint - transform.position;

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
                transform.position += transform.forward * 5.0f * Time.deltaTime;
            }
        }
    }
    #endregion

    #region アニメーション
    /// <summary>
    /// アニメーション処理
    /// </summary>
    private void AnimPlay(int num)
    {
        switch(num)
        {
            case 0:
                // 歩行アニメーション再生
                animator.SetBool("isWalk", true);
                break;
            case 1:
                // 待機モーションにする
                animator.SetBool("isWalk", false);
                break;
        }
    }
    #endregion

    #region Rayの当たり判定
    /// <summary>
    /// Rayが当たっているか
    /// </summary>
    private int RayHit(Vector3 pos)
    {
        // RayHitレイヤーとだけ衝突する
        int layerMask = 1 << 10;
        // Rayが当たっているか
        if (Physics.Raycast(new Vector3(transform.position.x, 2.0f, transform.position.z),
            pos, out rayhit, data.searchDistance, layerMask))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離、レイヤーマスク
        {
            // 当たったオブジェクトがペンギンかどうか
            if (rayhit.collider.tag == "Player")
            {
                return 1;
            }
        }
        return -1;
    }
    #endregion

    #region エフェクト作成
    /// <summary>
    /// エフェクト作成
    /// </summary>
    private void CreateEffect(Effect effect)
    {
        switch (effect)
        {
            case Effect.exclamation:    // ！エフェクト
                exclamationEffect =
                    EffectManager.Create(
                        new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z),
                        3);
                break;
            case Effect.question:       // ？エフェクト
                // エフェクト表示
                questionEffect =
                    EffectManager.Create(
                        new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z),
                        4);
                break;
        }
    }
    #endregion

    #region コルーチン
    /// <summary>
    /// ペンギンを追従するコルーチン
    /// </summary>
    private IEnumerator PenguinChase()
    {
        if (surpriseFlg)
        {
            surpriseFlg = false;
            // ストップ
            Stop();
            // エフェクト表示
            CreateEffect(Effect.exclamation);
            // 驚くアニメーション開始
            animator.Play("Surprised", 0, 0.0f);
        }

        yield return new WaitForSeconds(1.5f);

        // エフェクト削除
        if (exclamationEffect) Destroy(exclamationEffect);
        // ペンギンを追従開始
        Walk();
        chaseNow = true;
    }

    /// <summary>
    /// 音がなった地点で一時停止するコルーチン
    /// </summary>
    private IEnumerator SoundObj()
    {
        // エフェクト表示
        CreateEffect(Effect.question);
        // アニメーション再生

        yield return new WaitForSeconds(10.0f);

        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
        yield return new WaitForSeconds(0.5f);

        // 元の位置に戻る
        soundObjFlg = false;
        Walk();
        target = startPos;
    }
    #endregion

    #region 索敵範囲外の処理
    /// <summary>
    /// 索敵範囲外
    /// </summary>
    private void PlayerOutOfRange()
    {
        surpriseFlg = true;
        if (exclamationEffect) Destroy(exclamationEffect);
    }
    #endregion

    #region ストップ、動かす
    /// <summary>
    /// ストップ
    /// </summary>
    private void Stop()
    {
        moveFlg = false;
    }

    /// <summary>
    /// 動かす
    /// </summary>
    private void Walk()
    {
        moveFlg = true;
    }
    #endregion

    private void StatusReset()
    {
        chaseNow = false;
        surpriseFlg = true;
        Stop();
    }

    /// <summary>
    /// 視界範囲内（扇状視界）を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0, 0, 1, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up,
            Quaternion.Euler(0f, -data.searchAngle, 0f) * transform.forward, data.searchAngle * 2.0f, data.searchDistance);
    }

    /// <summary>
    /// 初期配置に戻す
    /// </summary>
    private void ReStart()
    {
        StatusReset();
        AnimPlay(1);
        this.transform.position = startPos;
        this.transform.rotation = startDir;
    }

    #region ポーズ処理
    /// <summary>
    /// ポーズ開始
    /// </summary>
    private void Pause()
    {
        Stop();
        // アニメーション
        animator.speed = 0.0f;
        // 足音ストップ
        audioSource.Stop();
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    private void Resumed()
    {
        if(chaseNow)
        {
            Walk();
        }
        // アニメーション
        animator.speed = 1.0f;
    }
    #endregion
}