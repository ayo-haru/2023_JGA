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
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    private bool bSurprise = true;    // 驚くアニメーション用フラグ
    private bool bMove = false;       // 戻すアニメーション用フラグ
    private bool bResetPos = false;
    private bool bHidePlayer = false;
    private AudioSource audioSource;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private NavMeshAgent navMesh;
    private bool bPathPending = true;
    private int nRoot = 0;
    private Quaternion startDir;    // 初期回転
    private GameObject exclamationEffect;   // ！エフェクト
    private GameObject questionEffect;      // ？エフェクト

    [SerializeField] private bool bChaseNow = false;    // ペンギンを追いかけているフラグ
    private bool bDir = false;
    private SphereCollider sphereCollider;
    private Player player;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private GameObject parentObj;       // 親オブジェクト取得
    [SerializeField] private GameObject hand;   // オブジェクトを持つ場所
    private bool bGimmick = false;    // ギミックオブジェクトを見つけたか
    private bool bCatch = false;      // ギミックオブジェクトを掴んだか
    private bool bSoundObj = false;   // 音がなったオブジェクトがあるか
    private int nGimmick = -1;

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
                GameData.eRoot index = data.roots[i];
                data.rootTransforms.Add(_StageSceneManager.GetRootTransform(index));
            }
        }

        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = this.GetComponent<AudioSource>();
        if (rb == null) rb = this.GetComponent<Rigidbody>();
        if (capsuleCollider == null) capsuleCollider = this.GetComponent<CapsuleCollider>();
        if (player == null) player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (gimmickObj == null) gimmickObj = transform.root.gameObject.GetComponent<GimmickObj>();  // 親オブジェクトのスクリプト取得
        startDir = this.transform.rotation;
        //sphereCollider.radius = search; // colliderのradiusを変更する
        
        // 位置更新を手動で行う
        navMesh.updatePosition = false;
        NavMeshStop();

        // 巡回ルートに要素があるか
        if (data.rootTransforms.Count >= 1)
        {
            nRoot = 0;
            navMesh.SetDestination(data.rootTransforms[nRoot].position);  // 目的地の設定
            if (navMesh.pathPending)
            {
                bMove = false;
                bPathPending = true;
                animator.SetBool("isWalk", false);
            }
        }
        else
        {
            bMove = false;
            NavMeshStop();
        }
    }

    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
    {
        if (PauseManager.isPaused && !GameData.isCatchPenguin)
        {
            return;
        }

        if (bMove)
        {
            if (bChaseNow) AnimPlay();
            Dir();
        }
        if (!bChaseNow) Distance();
        // 巡回ルートに要素があるか
        if (data.rootTransforms.Count >= 1)
            if (bPathPending) PathPending();
    }

    private void Update()
    {
        if (PauseManager.isPaused && !GameData.isCatchPenguin)
        {
            NavMeshStop();
            return;
        }

        if (GameData.isCatchPenguin)
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
            bChaseNow = false;
            bSurprise = true;
            bResetPos = false;
            navMesh.speed = data.speed * player.MaxMoveSpeed;
            GameData.isCatchPenguin = true;
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
            if (bHidePlayer) return;
            var pos = other.transform.position - transform.position;
            float targetAngle = Vector3.Angle(this.transform.forward, pos);

            // 視界の角度内に収まってRayが当たっているか
            if (targetAngle < data.searchAngle && RayHit(pos) == 1)
            {
                if (bChaseNow) return;
                bSoundObj = false;
                if (data.rootTransforms.Count <= 0)
                    if (bResetPos) bResetPos = false;
                // コルーチン開始
                StartCoroutine("PenguinChase");
            }
            else
            {
                // 隠れたか
                if (bChaseNow && RayHit(pos) == 3)
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
            if (data.status == Status.notReturnObj) return;
            if (bChaseNow) return;
            if (other.gameObject.tag != "Interact") return;
            
            // 索敵範囲のオブジェクトが元の位置になかったら目的地をオブジェクトに設定
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (other.gameObject != gimmickObj.gimmickList[i]) continue;
                if (gimmickObj.bReset[i]) return;
                if (gimmickObj.bBring[i]) return;

                var posObj = other.transform.position - transform.position;
                float objAngle = Vector3.Angle(this.transform.forward, posObj);

                // 視界の角度内に収まってRayが当たっているか
                if (objAngle < data.searchAngle && RayHit(posObj) == 2)
                {
                    bSoundObj = false;
                    parentObj = gimmickObj.gimmickList[i].transform.root.gameObject;    // 親オブジェクト取得
                    navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);
                    bGimmick = true;
                    nGimmick = i;
                }
                else if (bChaseNow)
                {
                    ObjOutOfRange();
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
                bSoundObj = true;
                // なったオブジェクトまで移動
                navMesh.SetDestination(other.gameObject.transform.position);
            }
        }
        #endregion
    }

    #region 飼育員が到着したか
    /// <summary>
    /// 飼育員の移動
    /// </summary>
    private void Distance()
    {
        // オブジェクトを元の位置に戻す
        if (bGimmick)
        {
            if (!bCatch)
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
                if (navMesh.remainingDistance <= 2.5f    // 目標地点までの距離が1.5ｍ以下になったら到着
                     && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    // 置くアニメーション終了したらオブジェクトを置く
                    // コルーチン開始
                    StartCoroutine("ResetObj");
                }
            }
        }
        // 音がなったオブジェクトの位置に行く
        if (bSoundObj)
        {
            // オブジェクトの位置に到着したか
            if (navMesh.remainingDistance <= 2.5f    // 目標地点までの距離が1.5ｍ以下になったら到着
                    && !navMesh.pathPending)         // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // プレイヤーに遭遇してない、オブジェクトもないなら
                if (!bChaseNow && !bGimmick)
                {
                    // コルーチン開始
                    StartCoroutine("SoundObj");
                }
                bSoundObj = false;
            }
        }
        if (data.rootTransforms.Count >= 1 && !bGimmick && !bChaseNow && !bSoundObj)
        {
            if (navMesh.remainingDistance <= 2.0f    // 目標地点までの距離が2.0ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // リストの順番に巡回する
                if (data.rootTransforms.Count - 1 > nRoot)
                {
                    nRoot += 1;
                }
                else
                {
                    nRoot = 0;
                }
                navMesh.SetDestination(data.rootTransforms[nRoot].position); // 目的地の再設定
            }
        }
        if (data.rootTransforms.Count <= 0 && bResetPos)
        {
            if (navMesh.remainingDistance <= 2.0f    // 目標地点までの距離が2.0ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                bResetPos = false;
                NavMeshStop();
            }
        }
    }
    #endregion

    #region navMeshを使わず移動する
    /// <summary>
    /// navMeshを使わず移動する
    /// </summary>
    private void Move()
    {
        transform.Translate(Vector3.forward * navMesh.speed * Time.deltaTime);
    }
    #endregion

    #region アニメーション
    /// <summary>
    /// ペンギン追従中アニメーション
    /// </summary>
    private void AnimPlay()
    {
        if (animator.GetBool("isChase")) return;

        var distance = Vector3.Distance(transform.position, player.transform.position);   // 距離
        var speed = (data.speed * player.MaxRunSpeed) * Time.deltaTime;
        var nowPos = speed / distance;     // 現在の位置

        if (nowPos <= 0.02f) return;
        // 距離が近くなったら
        animator.SetBool("isChase", true);
    }
    #endregion

    #region 飼育員の向きを変えて移動する処理
    /// <summary>
    /// 向きを変えて移動する処理
    /// </summary>
    private void Dir()
    {
        // 次に目指すべき位置を取得
        var nextPoint = navMesh.steeringTarget;
        Vector3 targetDir = nextPoint - transform.position;

        // ゼロベクトルじゃなかったら
        if (targetDir != Vector3.zero)
        {
            // その方向に向けて旋回する
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 120.0f * Time.deltaTime);
            rotation.x = 0.0f;
            rotation.z = 0.0f;
            transform.rotation = rotation;

            // 自分の向きと次の位置の角度差
            float angle = Vector3.Angle(targetDir, transform.forward);
            if (angle < 30.0f)
            {
                if (!bDir) bDir = true;
            }
            else
            {
                if (bDir) bDir = false;
            }

            // 見失った時向きを変えてから移動する
            if (bDir && bHidePlayer)
            {
                // 動く
                NavMeshMove();
                bHidePlayer = false;
            }

            if (navMesh.isStopped) return;
            Move();
            if (bChaseNow)
                navMesh.SetDestination(player.transform.position);
            else
                if (!animator.GetBool("isWalk")) animator.SetBool("isWalk", true);
            navMesh.nextPosition = transform.position;
        }
    }
    #endregion

    /// <summary>
    /// 経路計算できたか
    /// </summary>
    private void PathPending()
    {
        if (navMesh.pathPending) return;

        if (!bMove) bMove = true;
        if (bPathPending) bPathPending = false;
        navMesh.speed = data.speed * player.MaxMoveSpeed;   // 速度設定
        if (navMesh.isStopped) navMesh.isStopped = false;
        animator.SetBool("isWalk", true);
    }

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
            if (rayhit.collider.tag == "Interact" && !bGimmick)
            {
                return 2;
            }
            // 当たったオブジェクトが隠れるオブジェクトかどうか
            if (rayhit.collider.tag == "HideObj")
            {
                bHidePlayer = true;
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
        if (bSurprise)
        {
            bSurprise = false;
            bMove = false;
            // 驚きモーション中は移動させない
            NavMeshStop();
            // エフェクト表示
            if (!exclamationEffect)
                CreateEffect(Effect.exclamation);
            // 驚くアニメーション開始
            animator.SetTrigger("isSurprise");
            bChaseNow = true;
        }

        yield return new WaitForSeconds(1.5f);

        // オブジェクトを持っていたら置く
        if (bGimmick) bGimmick = false;
        if (bCatch)
        {
            bCatch = false;
            Bring();
        }
        // エフェクト削除
        if (exclamationEffect) Destroy(exclamationEffect);
        // ペンギンを追従開始
        navMesh.SetDestination(player.transform.position);
        NavMeshMove();
    }

    /// <summary>
    /// ペンギンを見失う
    /// </summary>
    private IEnumerator HidePenguin()
    {
        if (bMove)
        {
            bMove = false;
            // エフェクト表示
            if (!questionEffect)
                CreateEffect(Effect.question);
            // 止まる
            NavMeshStop();
            PlayerOutOfRange();
            bChaseNow = false;
        }

        yield return new WaitForSeconds(1.0f);

        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
        bMove = true;
        // 巡回ルートに要素があるか
        if (data.rootTransforms.Count >= 1)
        {
            navMesh.SetDestination(data.rootTransforms[nRoot].position); // 目的地の再設定
        }
        else
        {
            bResetPos = true;
            navMesh.SetDestination(data.respawnTF.position);
        }

        if (bDir) bDir = false;
        Dir();
    }

    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private IEnumerator CatchObj()
    {
        // 止まる
        NavMeshStop();
        if (bMove)
        {
            bMove = false;
            // エフェクト表示
            if (!questionEffect)
                CreateEffect(Effect.question);
            // 拾うアニメーション開始
            animator.SetTrigger("isPickUp001");
        }

        Bring();
        yield return new WaitForSeconds(1.0f);

        navMesh.SetDestination(gimmickObj.resetPos[nGimmick]);
        // 動く
        NavMeshMove();
        bCatch = true;
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
        if (bMove)
        {
            bMove = false;
            // 置くアニメーション開始
            animator.SetFloat("speed", -1f);
            animator.Play("Pick001", 0, 1f);
        }

        yield return new WaitForSeconds(1.0f);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
        {
            bGimmick = false;
            bCatch = false;
            gimmickObj.bReset[nGimmick] = true;
            Bring();

            yield return new WaitForSeconds(1.0f);
            animator.SetFloat("speed", 1f);
            animator.Play("Walk");
            // 動く
            if (data.rootTransforms.Count >= 1)
            {
                navMesh.SetDestination(data.rootTransforms[nRoot].position); // 目的地の再設定
            }
            else
            {
                bResetPos = true;
                navMesh.SetDestination(data.respawnTF.position);
            }
            NavMeshMove();
        }
    }

    /// <summary>
    /// 音がなった地点で一時停止する
    /// </summary>
    private IEnumerator SoundObj()
    {
        // エフェクト表示
        if (!questionEffect)
            CreateEffect(Effect.question);
        // 止まる
        NavMeshStop();

        yield return new WaitForSeconds(2.0f);

        // 動く
        if (data.rootTransforms.Count >= 1)
        {
            navMesh.SetDestination(data.rootTransforms[nRoot].position); // 目的地の再設定
        }
        else
        {
            bResetPos = true;
            navMesh.SetDestination(data.respawnTF.position);
        }
        NavMeshMove();
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
        GameObject obj = gimmickObj.gimmickList[nGimmick].gameObject;
        if (bGimmick)
        {
            // オブジェクトを持っている間、オブジェクトと飼育員の当たり判定を無くす
            Physics.IgnoreCollision(capsuleCollider, obj.GetComponent<BoxCollider>(), true);
            // 掴む
            gimmickObj.bBring[nGimmick] = true;
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().isKinematic = true;
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().useGravity = false;
            gimmickObj.gimmickList[nGimmick].transform.parent = hand.transform;
            gimmickObj.gimmickList[nGimmick].transform.localPosition = Vector3.zero;
        }
        else if (gimmickObj.bReset[nGimmick] || bChaseNow)
        {
            if (questionEffect) Destroy(questionEffect);
            // オブジェクトと飼育員の当たり判定をつける
            Physics.IgnoreCollision(capsuleCollider, obj.GetComponent<BoxCollider>(), false);
            // はなす
            gimmickObj.bBring[nGimmick] = false;
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().isKinematic = false;
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[nGimmick].transform.parent = parentObj.transform;
            if (!bChaseNow)
            {
                // ギミックを初期位置、初期回転にする
                gimmickObj.gimmickList[nGimmick].transform.position = gimmickObj.resetPos[nGimmick];
                gimmickObj.gimmickList[nGimmick].transform.rotation = gimmickObj.resetRot[nGimmick];
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
        bSurprise = true;
        animator.Play("Idle");
        if (animator.GetBool("isChase")) animator.SetBool("isChase", false);
        if (exclamationEffect) Destroy(exclamationEffect);
        if (!bGimmick)
        {
            // 巡回ルートに要素があるか
            if (data.rootTransforms.Count >= 1)
            {
                navMesh.SetDestination(data.rootTransforms[nRoot].position);     // 目的地の再設定
            }
            else
            {
                // リスポーン地点に戻る
                navMesh.SetDestination(data.respawnTF.position);
            }
        }
    }

    /// <summary>
    /// 索敵範囲外
    /// </summary>
    private void ObjOutOfRange()
    {
        bGimmick = false;
        if (bCatch)
        {
            bCatch = false;
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[nGimmick].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[nGimmick].transform.parent = parentObj.transform;
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
        if (!navMesh.isStopped) navMesh.isStopped = true;
        navMesh.velocity = Vector3.zero;
        navMesh.speed = 0.0f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // アニメーション更新
        if (animator.GetBool("isWalk")) animator.SetBool("isWalk", false);
        if (!bChaseNow)
        {
            if (animator.GetBool("isChase")) animator.SetBool("isChase", false);
            animator.Play("Idle");
        }
        if (data.rootTransforms.Count <= 0)
            if (bMove) bMove = false;
    }

    /// <summary>
    /// NavMeshを動かす
    /// </summary>
    private void NavMeshMove()
    {
        if (data.rootTransforms.Count <= 0)
        {
            if (!bChaseNow && !bResetPos && !bGimmick && !bSoundObj)
            {
                if (bMove) bMove = false;
                return;
            }
        }
        if (!bMove) bMove = true;
        if (navMesh.isStopped) navMesh.isStopped = false;
        if (bChaseNow)
        {
            navMesh.speed = data.speed * player.MaxRunSpeed;
        }
        else
        {
            navMesh.speed = data.speed * player.MaxMoveSpeed;
            if (!animator.GetBool("isWalk")) animator.SetBool("isWalk", true);
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
        if (bChaseNow) bChaseNow = false;
        if (!bSurprise) bSurprise = true;
        if (bResetPos) bResetPos = false;
        animator.SetBool("isChase", false);
        // インスペクターで設定したリスポーン位置に再配置する
        navMesh.Warp(data.respawnTF.position);
        if (data.rootTransforms.Count <= 0)
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
        NavMeshMove();
        // アニメーション
        animator.speed = 1.0f;
       
    }
    #endregion
}
