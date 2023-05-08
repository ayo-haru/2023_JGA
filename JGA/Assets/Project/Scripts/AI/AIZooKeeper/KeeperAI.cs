//=============================================================================
// @File	: [KeeperAI.cs]
// @Brief	: 飼育員のNavMeshを使った処理
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/24	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UnityEditor;

public class KeeperAI : MonoBehaviour
{
    private enum Effect
    {
        exclamation,    // ！エフェクト
        question,       // ？エフェクト   
    }

    [SerializeField] private Animator animator;
    private bool surpriseFlg = true;
    private bool questionFlg = true;
    private bool moveFlg = false;
    private AudioSource audioSource;
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private NavMeshAgent navMesh;
    private RaycastHit rayhit;
    private int rootNum = 0;
    private GameObject exclamationEffect;   // ！エフェクト
    private GameObject questionEffect;      // ？エフェクト
    private Vector3 startPos;       // 初期位置
    private Quaternion startDir;    // 初期回転

    [SerializeField] private bool chaseNow = false;    // ペンギンを追いかけているフラグ
    private Player player;

    private GameObject parentObj;       // 親オブジェクト取得
    private bool gimmickFlg = false;    // ギミックオブジェクトを見つけたか
    private bool catchFlg = false;      // ギミックオブジェクトを掴んだか
    private bool soundObjFlg = false;   // 音がなったオブジェクトがあるか
    private bool bResetPos = false;

    private BaseObj soundObj;
    private RadioObject radioObj;
    private Vector3 soundStartPos;
    private Quaternion soundStartDir;
    private bool radioResetFlg = true;
    private bool flg = false;
    private bool dirFlg = false;

    [SerializeField]
    private ZooKeeperData.Data data;    // 飼育員用の外部で設定できるパラメーターたち

    /// <summary>
    /// Inspectorから自身の値が変更されたときに自動で呼ばれる
    /// </summary>
    private void OnValidate()
    {
        sphereCollider = this.GetComponent<SphereCollider>();
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

        if (sphereCollider == null)
        {
            sphereCollider = this.GetComponent<SphereCollider>();
        }
        navMesh = GetComponent<NavMeshAgent>();
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        if (player == null) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (soundObj == null) soundObj = GameObject.Find("Radio_002").GetComponent<BaseObj>();
        if (radioObj == null) radioObj = GameObject.Find("Radio_002").GetComponent<RadioObject>();
        if (parentObj == null) parentObj = soundObj.gameObject.transform.root.gameObject; // 親オブジェクト取得

        // 初期位置と初期回転を取得
        startPos = this.transform.position;
        startDir = this.transform.rotation;
        soundStartPos = soundObj.gameObject.transform.position;
        soundStartDir = soundObj.gameObject.transform.rotation;

        // 位置更新を手動で行う
        navMesh.updatePosition = false;
        // NavMeshストップ
        NavMeshStop();
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

        //Debug.Log("sound : " + soundObj.GetisPlaySound());
        //Debug.Log("navMesh : " + navMesh.isStopped);

        Move();
        if (!navMesh.isStopped) CharControl();
        if (moveFlg)
        {
            AnimPlay();
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
            NavMeshStop();
            return;
        }

        if (MySceneManager.GameData.isCatchPenguin)
        {
            NavMeshStop();
            // ペンギンが捕まっているときはポーズ中なのでポーズ中でも処理を行う
            ReStart();
        }
    }

    /// <summary>
    /// 飼育員とペンギン
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        #region ペンギン
        if (collision.gameObject.tag == "Player")
        {
            NavMeshStop();
            chaseNow = false;
            MySceneManager.GameData.isCatchPenguin = true;
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の索敵範囲
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (chaseNow) return;

        #region ラジオ
        // 音がなってるか
        if (other.gameObject == soundObj.gameObject && soundObj.GetisPlaySound())
        {
            RadioPos();
            if (radioResetFlg) return;
            if (flg) return;
            soundObjFlg = true;
            navMesh.SetDestination(other.gameObject.transform.position);
            Dir();
            // オブジェクトの方を向いたら一時停止して動く
            if (dirFlg)
            {
                // コルーチン開始
                StartCoroutine("MoveStop");
                dirFlg = false;
                flg = true;
            }
        }
        #endregion

        #region ペンギン
        if (other.gameObject.tag == "Player")
        {
            if (!surpriseFlg) return;
            if (soundObjFlg) return;
            if (gimmickFlg) return;
            var pos = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(this.transform.forward, pos);
            Debug.DrawRay(new Vector3(transform.position.x, 2.0f, transform.position.z),
                pos * data.searchDistance, Color.red);

            // 視界の角度内に収まってRayが当たっているか
            if (targetAngle < data.searchAngle && RayHit(pos) == 1)
            {
                // コルーチン開始
                StartCoroutine("PenguinChase");
            }
        }
        #endregion
    }

#region ラジオが元の位置にあるか
    /// <summary>
    /// ラジオが元の位置にあるか
    /// </summary>
    private void RadioPos()
    {
        int x1 = Mathf.FloorToInt(soundObj.gameObject.transform.position.x);
        int z1 = Mathf.FloorToInt(soundObj.gameObject.transform.position.z);
        int x2 = Mathf.FloorToInt(soundStartPos.x);
        int z2 = Mathf.FloorToInt(soundStartPos.z);

        if (x1 != x2 || z1 != z2)
        {
            radioResetFlg = false;
        }
        else
        {
            radioResetFlg = true;
        }
    }
#endregion

#region 飼育員の移動
    /// <summary>
    /// 飼育員の移動
    /// </summary>
    private void Move()
    {
        // ペンギンを追いかける
        if (chaseNow)
        {
            navMesh.destination = player.transform.position;
            return;
        }
        // 音がなったオブジェクトの位置に行く
        if (soundObjFlg)
        {
            // オブジェクトの位置に到着したか
            if (navMesh.remainingDistance <= 2.5f    // 目標地点までの距離が2.5ｍ以下になったら到着
                    && !navMesh.pathPending)         // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // コルーチン開始
                StartCoroutine("SoundObj");
                return;
            }
        }
        // オブジェクトを元の位置に戻す
        if (gimmickFlg)
        {
            if (radioResetFlg) return;
            if (!catchFlg)
            {
                StartCoroutine("CatchObj");
                return;
            }
            else
            {
                // オブジェクトの元の位置に到着したか
                if (navMesh.remainingDistance <= 1.5f    // 目標地点までの距離が1.5ｍ以下になったら到着
                     && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    // コルーチン開始
                    StartCoroutine("ResetObj");
                    return;
                }
            }
        }
            
        // 元の位置に戻る
        if (bResetPos)
        {
            if (navMesh.remainingDistance <= 1.5f   // 目標地点までの距離が1.5ｍ以下になったら到着
                && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                bResetPos = false;
                moveFlg = false;
                NavMeshStop();
                this.transform.position = startPos;
                this.transform.rotation = startDir;
            }
        }
    }
    #endregion

#region navMeshを使わず移動する
    /// <summary>
    /// navMeshを使わず移動する
    /// </summary>
    private void CharControl()
    {
        //transform.position += transform.forward * navMesh.speed * Time.deltaTime;
        //navMesh.nextPosition = transform.position;
        if (navMesh.pathPending) return;
        Vector3 targetDeltaPosition;    // 差分取得用
        
        // nextPositionからdeltaPositionを算出
        targetDeltaPosition = navMesh.nextPosition - transform.position;
    
        // エージェントの移動を正とする
        transform.position = navMesh.nextPosition;
    
        // エージェントに追従する
        if (targetDeltaPosition.magnitude > navMesh.radius)
            transform.position = navMesh.nextPosition - 0.9f * targetDeltaPosition;
    }
#endregion

#region アニメーション
    /// <summary>
    /// アニメーションさせるかどうか
    /// </summary>
    private void AnimPlay()
    {
        if (navMesh.isStopped) return;

        // ペンギン追従中
        if(chaseNow)
        {
            if (animator.GetBool("isChase")) return;

            var distance = Vector3.Distance(transform.position, player.transform.position);   // 距離
            var speed = (data.speed * player.MaxRunSpeed) * Time.deltaTime;
            var nowPos = speed / distance;     // 現在の位置
            
            if (nowPos <= 0.02f) return;

            // 距離が近くなったら
            animator.SetBool("isChase", true);
        }
        // NavMeshの計算が終わっている、NavMeshが停止していない、ペンギン追従していない
        if (!navMesh.pathPending && !chaseNow)
        {
            if (animator.GetBool("isWalk")) return;

            // 歩行アニメーション再生
            animator.SetBool("isWalk", true);
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
        var nextPoint = navMesh.steeringTarget;
        Vector3 targetDir = nextPoint - transform.position;

        // ゼロベクトルじゃなかったら
        if (targetDir != Vector3.zero)
        {
            // その方向に向けて旋回する(120度/秒)
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 120.0f * Time.deltaTime);
            rotation.x = 0.0f;
            rotation.z = 0.0f;
            transform.rotation = rotation;

            // 自分の向きと次の位置の角度差が30度以下の場合
            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < 30.0f)
            {
                if(!dirFlg) dirFlg = true;
            }
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
            // 当たったオブジェクトがラジオかどうか
            if (rayhit.collider.gameObject == soundObj && !gimmickFlg)
            {
                return 2;
            }
            // 当たったオブジェクトが隠れるオブジェクトかどうか
            if (rayhit.collider.tag == "HideObj")
            {
                return 3;
            }
        }
        return -1;
    }
#endregion

#region コルーチン
    /// <summary>
    /// ペンギンを追従する
    /// </summary>
    private IEnumerator PenguinChase()
    {
        if (surpriseFlg)
        {
            surpriseFlg = false;
            // 驚きモーション中は移動させない
            NavMeshStop();
            // エフェクト表示
            CreateEffect(Effect.exclamation);
            // 驚くアニメーション開始
            animator.SetTrigger("isSurprise");
            chaseNow = true;
        }

        yield return new WaitForSeconds(0.5f);

        // エフェクト削除
        if (exclamationEffect) Destroy(exclamationEffect);
        // ペンギンを追従開始
        NavMeshMove();
    }

    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private IEnumerator CatchObj()
    {
        // 止まる
        NavMeshStop();
        if (moveFlg)
        {
            moveFlg = false;
            // 拾うアニメーション開始
            animator.SetTrigger("isPickUp");
        }

        Bring();
        yield return new WaitForSeconds(3.0f);

        // 動く
        NavMeshMove();
        navMesh.SetDestination(soundStartPos);
        catchFlg = true;
        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
    }

    /// <summary>
    /// オブジェクトを元に戻す
    /// </summary>
    private IEnumerator ResetObj()
    {
        // 止まる
        NavMeshStop();
        if (moveFlg)
        {
            moveFlg = false;
            // 置くアニメーション開始
            animator.SetBool("isWalk", false);
            animator.SetFloat("speed", -1f);
            //animator.SetTrigger("isPickUp");
            animator.Play("Pick", 0, 1f);
        }

        yield return new WaitForSeconds(1.0f);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
        {
            gimmickFlg = false;
            catchFlg = false;
            radioResetFlg = true;
            Bring();

            yield return new WaitForSeconds(1.0f);
            animator.SetFloat("speed", 1f);
            animator.SetBool("isWalk", true);
            animator.Play("Walk");
            // 動く
            NavMeshMove();
            moveFlg = true;
            bResetPos = true;
            navMesh.SetDestination(startPos); // 目的地の再設定
        }
    }

    /// <summary>
    /// 一時停止するコルーチン
    /// </summary>
    private IEnumerator MoveStop()
    {
        if (questionFlg)
        {
            questionFlg = false;
            // エフェクト表示
            CreateEffect(Effect.question);
        }

        yield return new WaitForSeconds(1.0f);

        NavMeshMove();
    }

    /// <summary>
    /// 音がなった地点で一時停止する
    /// </summary>
    private IEnumerator SoundObj()
    {
        // 止まる
        NavMeshStop();

        yield return new WaitForSeconds(1.0f);

        // ラジオを戻す
        radioObj.StopRadio();
        soundObjFlg = false;
        gimmickFlg = true;
    }
    #endregion

#region オブジェクトを掴む、はなす処理
    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private void Bring()
    {
        if (!catchFlg && gimmickFlg)
        {
            // 掴む
            soundObj.GetComponent<Rigidbody>().isKinematic = true;   // 物理演算の影響を受けないようにする
            soundObj.GetComponent<Rigidbody>().useGravity = false;
            var pos = transform.position;
            soundObj.gameObject.transform.position =
                new Vector3(pos.x - 2f,
                            pos.y + 5f,
                            pos.z);
            soundObj.gameObject.transform.parent = this.transform;
            soundObj.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (radioResetFlg)
        {
            if (questionEffect) Destroy(questionEffect);
            // はなす
            soundObj.GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            soundObj.GetComponent<Rigidbody>().useGravity = true;
            soundObj.gameObject.transform.parent = parentObj.transform;
            soundObj.gameObject.transform.position = soundStartPos;
            soundObj.gameObject.transform.rotation = soundStartDir;
        }
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
                        new Vector3(transform.position.x, transform.position.y + 7.5f, transform.position.z),
                        3);
                exclamationEffect.transform.parent = this.transform;
                break;
            case Effect.question:       // ？エフェクト
                // エフェクト表示
                questionEffect =
                    EffectManager.Create(
                        new Vector3(transform.position.x, transform.position.y + 7.5f, transform.position.z),
                        4);
                questionEffect.transform.parent = this.transform;
                break;
        }
    }
#endregion

#region NavMeshをストップ、動かす
    /// <summary>
    /// NavMeshストップ
    /// </summary>
    private void NavMeshStop()
    {
        if (navMesh.updateRotation) navMesh.updateRotation = false; // 回転を止める
        if (!navMesh.isStopped) navMesh.isStopped = true;
        navMesh.velocity = Vector3.zero;
        navMesh.speed = 0.0f;
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        // 待機モーションにする
        animator.SetBool("isWalk", false);
    }

    /// <summary>
    /// NavMeshを動かす
    /// </summary>
    private void NavMeshMove()
    {
        if (!moveFlg) moveFlg = true;
        if (navMesh.isStopped) navMesh.isStopped = false;
        if (!navMesh.updateRotation) navMesh.updateRotation = true; // 回転する
        if (chaseNow)
        {
            navMesh.speed = data.speed * player.MaxRunSpeed;
        }
        else
        {
            navMesh.speed = data.speed * player.MaxMoveSpeed;
        }
    }
#endregion

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
        if (chaseNow) chaseNow = false;
        if (!surpriseFlg) surpriseFlg = true;
        if (moveFlg) moveFlg = false;
        animator.SetBool("isChase", false);
        AnimPlay();
        // 初期位置に戻る
        navMesh.Warp(startPos);
        navMesh.transform.rotation = startDir;
    }

    /// <summary>
    /// 外部で設定したデータを流し込む
    /// </summary>
    /// <param name="_data"></param>
    public void SetData(ZooKeeperData.Data _data)
    {
        data = _data;
    }

#region ポーズ処理
    /// <summary>
    /// ポーズ開始
    /// </summary>
    private void Pause()
    {
        NavMeshStop();
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
        // 追いかけている時、navMeshが動いている時
        if(chaseNow || !navMesh.isStopped)
            NavMeshMove();
        // アニメーション
        animator.speed = 1.0f;
    }
#endregion

}