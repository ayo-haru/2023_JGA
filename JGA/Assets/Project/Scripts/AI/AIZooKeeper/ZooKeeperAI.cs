//=============================================================================
// @File	: [ZooKeeperAI.cs]
// @Brief	: 飼育員の仮の処理
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
// 2023/03/02	オブジェクトのListを追加
// 2023/03/02   ギミックオブジェクトとの当たり判定処理の作成
// 2023/03/08   ギアニメーション追加、Move()に記述(吉原)
// 2023/03/10   Rayで追従する処理追加
// 2023/03/19   飼育員をプログラムで配置するためにインスペクターで値決めてたのをScriptableObjectで決めるように変えた(伊地田)
// 2023/03/21   一時停止処理の作成開始
// 2023/03/25   飼育員の速度変更
// 2023/03/29   足滑りの無い移動実装
// 2023/03/30   オブジェクトの音に反応する処理作成
// 2023/03/31   プレイヤーを追いかける時の角度を調整する処理追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

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
    private AudioSource audioSource;
    private AnimationCurve animationCurve;
    private NavMeshAgent navMesh;
    private int rootNum = 0;
    private GameObject exclamationEffect;   // ！エフェクト
    private GameObject questionEffect;      // ？エフェクト


    [SerializeField] private bool chaseNow = false;    // ペンギンを追いかけているフラグ
    private SphereCollider sphereCollider;
    private Player player;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private GameObject soundObj;
    private GameObject parentObj;       // 親オブジェクト取得
    private bool gimmickFlg = false;    // ギミックオブジェクトに当たったか
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
            navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の設定

            // 速度設定(始めは歩いてる)
            navMesh.speed = data.speed * player.MaxMoveSpeed;
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
        if (PauseManager.isPaused)
            return;

        Move();
        CharControl();
        Dir();
    }

    private void Update()
    {
        if (PauseManager.isPaused)
            return;

        if (MySceneManager.GameData.isCatchPenguin)
        {
            ReStart();
        }
    }

    /// <summary>
    /// 飼育員とペンギン、ギミックの当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        #region ペンギン
        if (collision.gameObject.tag == "Player")
        {
            MySceneManager.GameData.isCatchPenguin = true;
            chaseNow = false;
            navMesh.speed = data.speed * player.MaxMoveSpeed;
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
            var direction = transform.forward;  // 方向
            float targetAngle = Vector3.Angle(this.transform.forward, pos);
            Debug.DrawRay(transform.position, pos * data.searchDistance, Color.red);

            // 視界の角度内に収まっているかどうか
            if (targetAngle < data.searchAngle)
            {
                // Rayが当たっているか
                if (Physics.Raycast(transform.position, pos, out rayhit, data.searchDistance))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離
                {
                    // 当たったオブジェクトがペンギンかどうか
                    if (rayhit.collider == other)
                    {
                        if (!chaseNow)
                        {
                            soundObjFlg = false;
                            // コルーチン開始
                            StartCoroutine("PenguinChase");
                        }
                    }
                }
                else
                {
                    // Ray当たってない
                    if (!chaseNow)
                    {
                        PlayerOutOfRange();
                    }
                }
            }
            else
            {
                // 範囲外
                if (!chaseNow)
                {
                    PlayerOutOfRange();
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
                        var directionObj = transform.forward;  // 方向
                        float objAngle = Vector3.Angle(this.transform.forward, posObj);
                        Debug.DrawRay(transform.position, posObj * data.searchDistance, Color.black);

                        // 視界の角度内に収まっているかどうか
                        if (objAngle < data.searchAngle)
                        {
                            // Rayが当たっているか
                            if (Physics.Raycast(transform.position, posObj, out rayhit, data.searchDistance))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離
                            {
                                if (rayhit.collider.tag == "Interact" && !gimmickFlg)
                                {
                                    soundObjFlg = false;
                                    gimmickFlg = true;
                                    parentObj = gimmickObj.gimmickList[i].transform.root.gameObject;        // 親オブジェクト取得
                                    navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);
                                    gimmickNum = i;
                                }
                            }
                            else if (chaseNow)
                            {
                                ObjOutOfRange();
                            }
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
                soundObj = other.gameObject;
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
        if (other.gameObject.tag == "Player")
        {
            chaseNow = false;
            navMesh.speed = data.speed * player.MaxMoveSpeed;
            if (data.rootTransforms.Count >= 1)
            {
                navMesh.SetDestination(data.rootTransforms[rootNum].position);     // 目的地の再設定
            }
            else
            {
                NavMeshStop();
            }
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の移動
    /// </summary>
    private void Move()
    {
        #region 移動
        // ペンギンを追いかける
        if (chaseNow)
        {
            navMesh.destination = player.transform.position;
        }
        // オブジェクトを元の位置に戻す
        if (gimmickFlg)
        {
            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", true);
            //--------------------------------

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
            // なったオブジェクトまで移動
            navMesh.SetDestination(soundObj.transform.position);
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
            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", true);
            //--------------------------------
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
            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", false);
            //--------------------------------
        }
        #endregion
    }

    /// <summary>
    /// navMeshを使わず移動する
    /// </summary>
    private void CharControl()
    {
        Vector3 targetDeltaPosition;    // 差分取得用

        if (!navMesh.pathPending)
        {
            // nextPositionからdeltaPositionを算出
            targetDeltaPosition = navMesh.nextPosition - transform.position;

            // エージェントの移動を正とする
            transform.position = navMesh.nextPosition;

            // エージェントに追従する
            if (targetDeltaPosition.magnitude > navMesh.radius)
                transform.position = navMesh.nextPosition - 0.9f * targetDeltaPosition;
        }
    }

    #region コルーチン
    /// <summary>
    /// ペンギンを追従するコルーチン
    /// </summary>
    private IEnumerator PenguinChase()
    {
        if (surpriseFlg)
        {
            // 驚きモーション中は移動させない
            NavMeshStop();
            // エフェクト表示
            CreateEffect(Effect.exclamation);
            //-----------------------
            // 驚くアニメーション開始
            animator.SetTrigger("isSurprise");
            //-----------------------
            surpriseFlg = false;
        }

        yield return new WaitForSeconds(1.5f);

        // 驚くアニメーション終了したらペンギンを追従開始
        chaseNow = true;
        NavMeshMove();

        // オブジェクトを置く
        gimmickFlg = false;
        if (catchFlg)   // オブジェクトを持ってる時
        {
            catchFlg = false;
            Bring();
        }

        // エフェクト削除
        yield return new WaitForSeconds(0.5f);
        if (exclamationEffect) Destroy(exclamationEffect);
    }

    /// <summary>
    /// オブジェクトを運ぶコルーチン
    /// </summary>
    private IEnumerator CatchObj()
    {
        // エフェクト表示
        CreateEffect(Effect.question);
        // 止まる
        NavMeshStop();

        catchFlg = true;
        Bring();

        yield return new WaitForSeconds(1.5f);

        // 動く
        NavMeshMove();

        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
    }

    /// <summary>
    /// オブジェクトを元に戻すコルーチン
    /// </summary>
    private IEnumerator ResetObj()
    {
        // 止まる
        NavMeshStop();

        //------------------------
        // 置くアニメーション開始（仮で驚くモーション）
        animator.SetTrigger("isSurprise");
        //------------------------

        yield return new WaitForSeconds(1.5f);

        gimmickFlg = false;
        catchFlg = false;
        gimmickObj.bReset[gimmickNum] = true;
        Bring();

        // 動く
        NavMeshMove();
        navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
    }

    /// <summary>
    /// 音がなった地点で一時停止するコルーチン
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

    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private void Bring()
    {
        #region 運ぶ
        if (catchFlg && gimmickFlg)
        {
            // 掴む
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = true;   // 物理演算の影響を受けないようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = false;
            gimmickObj.gimmickList[gimmickNum].transform.parent = this.transform;
            navMesh.SetDestination(gimmickObj.resetPos[gimmickNum].transform.position);
        }
        else if (gimmickObj.bReset[gimmickNum] || chaseNow)
        {
            if (questionEffect) Destroy(questionEffect);
            // はなす
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = parentObj.transform;
            if (!chaseNow)
            {
                gimmickObj.gimmickList[gimmickNum].transform.position = gimmickObj.resetPos[gimmickNum].transform.position;
            }
        }
        #endregion
    }

    /// <summary>
    /// 向きの調整
    /// </summary>
    private void Dir()
    {
        // 次に目指すべき位置を取得
        var nextPoint = navMesh.steeringTarget;
        Vector3 targetDir = nextPoint - transform.position;

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
                        new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z),
                        3);
                break;
            case Effect.question:       // ？エフェクト
                // エフェクト表示
                questionEffect =
                    EffectManager.Create(
                        new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z),
                        4);
                break;
        }
    }

    /// <summary>
    /// NavMeshストップ
    /// </summary>
    private void NavMeshStop()
    {
        navMesh.isStopped = true;
        navMesh.speed = 0.0f;
    }

    /// <summary>
    /// NavMeshを動かす
    /// </summary>
    private void NavMeshMove()
    {
        navMesh.isStopped = false;
        if (chaseNow)
        {
            navMesh.speed = data.speed * player.MaxRunSpeed;
        }
        else
        {
            navMesh.speed = data.speed * player.MaxMoveSpeed;
        }
    }

    /// <summary>
    /// 視界範囲内（扇状視界）を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        //Handles.color = new Color(0, 0, 1, 0.3f);
        //Handles.DrawSolidArc(transform.position, Vector3.up,
        //    Quaternion.Euler(0f, -data.searchAngle, 0f) * transform.forward, data.searchAngle * 2.0f, data.searchDistance);
    }

    /// <summary>
    /// 初期配置に戻す
    /// </summary>
    private void ReStart()
    {
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
}
