//=============================================================================
// @File	: [ZooKeeperAI.cs]
// @Brief	: 飼育員のNavMeshを使った処理
// @Author	: MAKIYA MIO
// @Editer	: Ichida Mai
// @Detail	: https://yttm-work.jp/unity/unity_0036.html
//            https://www.sejuku.net/blog/83620
//            https://www.matatabi-ux.com/entry/2021/03/18/100000 
//            https://light11.hatenadiary.com/entry/2019/12/10/223519
//
// [Date]
// 2023/02/28	スクリプト作成
// 2023/02/28	スピードをスライダーで変更可能にした
// 2023/02/28	Raycastの追加
// 2023/03/02   ギミックオブジェクトとの当たり判定処理の作成
// 2023/03/08   アニメーション追加、Move()に記述(吉原)
// 2023/03/10   Rayで追従する処理追加
// 2023/03/19   飼育員をプログラムで配置するためにインスペクターで値決めてたのをScriptableObjectで決めるように変えた(伊地田)
// 2023/03/21   一時停止処理の作成
// 2023/03/29   足滑りの無い移動実装
// 2023/03/30   オブジェクトの音に反応する処理作成
// 2023/03/31   プレイヤーを追いかける時の角度を調整する処理追加
// 2023/04/08   RayHitのレイヤーにだけ当たり判定をとるようにした
// 2023/04/21   見失う処理追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UnityEditor;

public class ZooKeeperAI : MonoBehaviour
{
    public enum Status
    {
        returnObj,      // オブジェクトを元に戻す
        notReturnObj,   // オブジェクトを元に戻さない
    }
    private enum Effect
    {
        exclamation,    // ！エフェクト
        question,       // ？エフェクト   
    }

    [SerializeField] private Animator animator;
    private bool surpriseFlg = true;    // 驚くアニメーション用フラグ
    private bool ResetFlg = true;       // 戻すアニメーション用フラグ
    private AudioSource audioSource;
    private NavMeshAgent navMesh;
    private int rootNum = 0;
    private GameObject exclamationEffect;   // ！エフェクト
    private GameObject questionEffect;      // ？エフェクト

    [SerializeField] private bool chaseNow = false;    // ペンギンを追いかけているフラグ
    private SphereCollider sphereCollider;
    private Player player;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private GameObject parentObj;       // 親オブジェクト取得
    private bool gimmickFlg = false;    // ギミックオブジェクトを見つけたか
    private bool catchFlg = false;      // ギミックオブジェクトを掴んだか
    private bool soundObjFlg = false;   // 音がなったオブジェクトがあるか
    private int gimmickNum = -1;


    [Space(100)]
    [Header("デバッグ用直置きしたプレハブか？\nチェックいれて下のdataを設定すると\n直置きでも使えるよ")]
    [SerializeField] private bool isDebug = false;
    /*
     * インスペクターで設定してた値はScriptableObjectで設定できるようにしました
     * ZooKeeperData.csに設定の項目はあるよ。
     * Assets/Project/Script/AI/ZooKeeperData　にデータのScriptableObjectはあるよ。
     * 生成は3/19現在StageSceneManager.csのStartでやってます！
    */
    [SerializeField]
    private ZooKeeperData.Data data;    // 飼育員用の外部で設定できるパラメーターたち

    /// <summary>
    /// Inspectorから自身の値が変更されたときに自動で呼ばれる
    /// </summary>
    private void OnValidate()
    {
        sphereCollider = this.GetComponent<SphereCollider>();
        /*
         * 最初からシーン上に配置しないので一回コメントアウトさせていただいた（伊地田）
         */
        //sphereCollider.radius = data.search; // colliderのradiusを変更する
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
        //デバッグ用直置きしたとき用のデータセット。
        //自動生成はStageSceneManagerで行っている
        if (isDebug)
        {
            StageSceneManager _StageSceneManager = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
            data.rootTransforms = new List<Transform>();
            for (int i = 0; i < data.roots.Length; i++)
            {
                MySceneManager.eRoot index = data.roots[i];
                data.rootTransforms.Add(_StageSceneManager.GetRootTransform(index));
            }
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (player == null) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (gimmickObj == null) gimmickObj = transform.root.gameObject.GetComponent<GimmickObj>();  // 親オブジェクトのスクリプト取得
        //sphereCollider.radius = search; // colliderのradiusを変更する

        // 位置更新を手動で行う
        navMesh.updatePosition = false;
        // 巡回ルートに要素があるか
        if (data.rootTransforms.Count >= 1)
        {
            rootNum = 0;
            navMesh.SetDestination(data.rootTransforms[rootNum].position);  // 目的地の設定
            navMesh.speed = data.speed * player.MaxMoveSpeed;               // 速度設定
        }
        else
        {
            NavMeshStop();
        }
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

        Move();
        CharControl();
        if (ResetFlg)
        {
            AnimPlay();
            Dir();
        }
    }

    private void Update()
    {
        if (PauseManager.isPaused && !MySceneManager.GameData.isCatchPenguin) {
            NavMeshStop();
            return;
        }

        if (MySceneManager.GameData.isCatchPenguin)
        {
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
            surpriseFlg = true;
            navMesh.speed = data.speed * player.MaxMoveSpeed;
            MySceneManager.GameData.isCatchPenguin = true;
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の索敵範囲にペンギンがいるか
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        #region ペンギン
        if (other.gameObject.tag == "Player")
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
                    soundObjFlg = false;
                    // コルーチン開始
                    StartCoroutine("PenguinChase");
                }
            }
            else
            {
                // 範囲外か隠れたか
                if (!chaseNow || RayHit(pos) == 3)
                {
                    // コルーチン開始
                    StartCoroutine("HidePenguin");
                }
            }
        }
        #endregion

        #region ギミックオブジェクト
        if (gimmickObj.gimmickList.Count >= 1)
        {
            // 索敵範囲にギミックオブジェクトが入ったら目的地をオブジェクトに設定
            if (data.status == Status.returnObj && other.gameObject.tag == "Interact" && !chaseNow)
            {
                for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
                {
                    if (other.gameObject == gimmickObj.gimmickList[i] && !gimmickObj.bReset[i])
                    {
                        var posObj = other.transform.position - transform.position;
                        float objAngle = Vector3.Angle(this.transform.forward, posObj);
                        Debug.DrawRay(transform.position, posObj * data.searchDistance, Color.black);

                        // 視界の角度内に収まってRayが当たっているか
                        if (objAngle < data.searchAngle && RayHit(posObj) == 2)
                        {
                            soundObjFlg = false;
                            gimmickFlg = true;
                            parentObj = gimmickObj.gimmickList[i].transform.root.gameObject;        // 親オブジェクト取得
                            navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);
                            gimmickNum = i;
                        }
                        else if (chaseNow)
                        {
                            ObjOutOfRange();
                        }
                    }
                }
            }
        }
        #endregion

        #region 音がなるオブジェクト
        // 音がなってるか
        if (other.GetComponent<BaseObj>())
        {
            if (other.GetComponent<BaseObj>().GetisPlaySound())
            {
                soundObjFlg = true;
                // なったオブジェクトまで移動
                navMesh.SetDestination(other.gameObject.transform.position);
            }
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の索敵範囲外のとき
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        #region ペンギン
        //if (other.gameObject.tag == "Player")
        //{
        //    chaseNow = false;
        //    navMesh.speed = data.speed * player.MaxMoveSpeed;
        //    if (data.rootTransforms.Count >= 1)
        //    {
        //        navMesh.SetDestination(data.rootTransforms[rootNum].position);     // 目的地の再設定
        //    }
        //    else
        //    {
        //        NavMeshStop();
        //    }
        //}
        #endregion
    }

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
        }
        // オブジェクトを元の位置に戻す
        if (gimmickFlg)
        {
            if (!catchFlg)
            {
                // オブジェクトの位置に到着したか
                if (navMesh.remainingDistance <= 2.5f   // 目標地点までの距離が2.0ｍ以下になったら到着
                         && !navMesh.pathPending)       // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    // コルーチン開始
                    StartCoroutine("CatchObj");
                }
            }
            else
            {
                // オブジェクトの元の位置に到着したか
                if (navMesh.remainingDistance <= 1.5f    // 目標地点までの距離が1.5ｍ以下になったら到着
                     && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    // 置くアニメーション終了したらオブジェクトを置く
                    // コルーチン開始
                    StartCoroutine("ResetObj");
                }
            }
        }
        // 音がなったオブジェクトの位置に行く
        if (soundObjFlg)
        {
            // オブジェクトの位置に到着したか
            if (navMesh.remainingDistance <= 1.5f    // 目標地点までの距離が1.5ｍ以下になったら到着
                    && !navMesh.pathPending)         // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // プレイヤーに遭遇してない、オブジェクトもないなら
                if (!chaseNow && !gimmickFlg)
                {
                    // コルーチン開始
                    StartCoroutine("SoundObj");
                }
                soundObjFlg = false;
            }
        }
        if (data.rootTransforms.Count >= 1 && !gimmickFlg && !chaseNow && !soundObjFlg)
        {
            if (navMesh.remainingDistance <= 2.0f    // 目標地点までの距離が2.0ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // リストの順番に巡回する
                if (data.rootTransforms.Count - 1 > rootNum)
                {
                    rootNum += 1;
                }
                else
                {
                    rootNum = 0;
                }
                navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
            }
        }
        else if (data.rootTransforms.Count <= 0)
        {
            NavMeshStop();
        }
    }
    #endregion

    #region navMeshを使わず移動する
    /// <summary>
    /// navMeshを使わず移動する
    /// </summary>
    private void CharControl()
    {
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
        if(chaseNow)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);   // 距離
            var speed = (data.speed * player.MaxRunSpeed) * Time.deltaTime;
            var nowPos = speed / distance;     // 現在の位置

            // 距離が近くなったら
            if (nowPos >= 0.02f)
            {
                animator.SetBool("isChase", true);
            }
        }
        // 巡回リストが0、NavMeshの計算が終わってない、ペンギンを追従してない時
        if ((data.rootTransforms.Count <= 0 || navMesh.pathPending) && !chaseNow)
        {
            // 待機モーションにする
            animator.SetBool("isWalk", false);
        }
        // NavMeshの計算が終わっているかつNavMeshが停止していない時
        if(!navMesh.pathPending && !navMesh.isStopped)
        {
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 120.0f * Time.deltaTime);

            // 自分の向きと次の位置の角度差が30度以上の場合、その場で旋回
            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < 30.0f)
            {
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
            // 当たったオブジェクトがギミックかどうか
            if (rayhit.collider.tag == "Interact" && !gimmickFlg)
            {
                return 2;
            }
            // 当たったオブジェクトが隠れるオブジェクトかどうか
            if(rayhit.collider.tag == "HideObj")
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

        yield return new WaitForSeconds(2.5f);
            
        // ペンギンを追従開始
        NavMeshMove();

        // オブジェクトを持っていたら置く
        if (gimmickFlg) gimmickFlg = false;
        if (catchFlg)
        {
            catchFlg = false;
            Bring();
        }

        // エフェクト削除
        yield return new WaitForSeconds(0.5f);
        if (exclamationEffect) Destroy(exclamationEffect);
    }

    /// <summary>
    /// ペンギンを見失う
    /// </summary>
    private IEnumerator HidePenguin()
    {
        if (ResetFlg)
        {
            ResetFlg = false;
            // エフェクト表示
            CreateEffect(Effect.question);
            // 止まる
            NavMeshStop();
            animator.SetBool("isWalk", false);
            PlayerOutOfRange();
            chaseNow = false;
        }

        yield return new WaitForSeconds(1.0f);

        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
        // 動く
        NavMeshMove();
        ResetFlg = true;
        navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
    }

    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private IEnumerator CatchObj()
    {
        // 止まる
        NavMeshStop();
        if (ResetFlg)
        {
            ResetFlg = false;
            // エフェクト表示
            CreateEffect(Effect.question);
            // 拾うアニメーション開始
            animator.SetTrigger("isPickUp001");
        }

        yield return new WaitForSeconds(5.0f);
        catchFlg = true;
        Bring();

        yield return new WaitForSeconds(1.0f);
        // 動く
        NavMeshMove();

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
        if (ResetFlg)
        {
            ResetFlg = false;
            // 置くアニメーション開始
            animator.SetBool("isWalk", false);
            animator.SetFloat("speed", -1f);
            animator.Play("Pick001", 0, 1f);
        }

        yield return new WaitForSeconds(1.0f);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
        {
            gimmickFlg = false;
            catchFlg = false;
            gimmickObj.bReset[gimmickNum] = true;
            Bring();

            yield return new WaitForSeconds(1.0f);
            animator.SetFloat("speed", 1f);
            animator.SetBool("isWalk", true);
            animator.Play("Walk");
            // 動く
            NavMeshMove();
            navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
        }
    }

    /// <summary>
    /// 音がなった地点で一時停止する
    /// </summary>
    private IEnumerator SoundObj()
    {
        // エフェクト表示
        CreateEffect(Effect.question);
        // 止まる
        NavMeshStop();

        yield return new WaitForSeconds(2.0f);

        // 動く
        NavMeshMove();
        navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
    }
    #endregion

    #region オブジェクトを掴む、はなす処理
    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private void Bring()
    {
        if (catchFlg && gimmickFlg)
        {
            ResetFlg = true;
            // 掴む
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = true;   // 物理演算の影響を受けないようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = false;
            gimmickObj.gimmickList[gimmickNum].transform.parent = this.transform;
            gimmickObj.gimmickList[gimmickNum].transform.localPosition = new Vector3(0.0f, 1.0f, 1.0f);
            navMesh.SetDestination(gimmickObj.resetPos[gimmickNum].transform.position);
        }
        else if (gimmickObj.bReset[gimmickNum] || chaseNow)
        {
            if (questionEffect) Destroy(questionEffect);
            ResetFlg = true;
            // はなす
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = parentObj.transform;
            if (!chaseNow)
            {
                gimmickObj.gimmickList[gimmickNum].transform.position = gimmickObj.resetPos[gimmickNum].transform.position;
            }
        }
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
        if (!gimmickFlg)
        {
            if (data.rootTransforms.Count >= 1)
            {
                navMesh.SetDestination(data.rootTransforms[rootNum].position);     // 目的地の再設定
            }
            else
            {
                navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
            }
        }
    }

    /// <summary>
    /// 索敵範囲外
    /// </summary>
    private void ObjOutOfRange()
    {
        gimmickFlg = false;
        if (catchFlg)
        {
            catchFlg = false;
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = parentObj.transform;
        }
    }
    #endregion

    #region エフェクト作成
    /// <summary>
    /// エフェクト作成
    /// </summary>
    private void CreateEffect(Effect effect)
    {
        switch(effect)
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
        navMesh.velocity = Vector3.zero;
        navMesh.speed = 0.0f;
        if (!navMesh.isStopped) navMesh.isStopped = true;
    }

    /// <summary>
    /// NavMeshを動かす
    /// </summary>
    private void NavMeshMove()
    {
        // 巡回スートがなかったら
        if (data.rootTransforms.Count <= 0) return;

        if (navMesh.isStopped) navMesh.isStopped = false;
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

#if UNITY_EDITOR
    /// <summary>
    /// 視界範囲内（扇状視界）を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0, 0, 1, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up,
            Quaternion.Euler(0f, -data.searchAngle, 0f) * transform.forward, data.searchAngle * 2.0f, data.searchDistance);
    }
#endif

    /// <summary>
    /// 初期配置に戻す
    /// </summary>
    private void ReStart()
    {
        if (chaseNow) chaseNow = false;
        if (!surpriseFlg) surpriseFlg = true;
        animator.SetBool("isChase", false);
        // インスペクターで設定したリスポーン位置に再配置する
        navMesh.Warp(data.respawnTF.position);
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
        NavMeshMove();
        // アニメーション
        animator.speed = 1.0f;
    }
    #endregion
}
