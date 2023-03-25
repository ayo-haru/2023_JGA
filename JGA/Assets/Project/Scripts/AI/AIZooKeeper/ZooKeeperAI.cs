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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UniRx;

public class ZooKeeperAI : MonoBehaviour
{
    public enum Status
    {
        returnObj,      // オブジェクトを元に戻す
        notReturnObj,   // オブジェクトを元に戻さない
    }

    [SerializeField] private Animator animator;
    private AnimatorStateInfo animStateInfo;
    private bool surpriseFlg = true;    // 驚くアニメーション用フラグ
    private int rootNum = 0;
    private GameObject exclamationEffect;
    private GameObject questionEffect;

    /*
     * インスペクターで設定してた値はScriptableObjectで設定できるようにしました
     * ZooKeeperData.csに設定の項目はあるよ。
     * Assets/Project/Script/AI/ZooKeeperData　にデータのScriptableObjectはあるよ。
     * 生成は3/19現在StageSceneManager.csのStartでやってます！
     */
    private ZooKeeperData.Data data;    // 飼育員用の外部で設定できるパラメーターたち

    [SerializeField] private bool chaseNow = false;    // ペンギンを追いかけているフラグ
    private SphereCollider sphereCollider;
    //private float angle = 45.0f;

    private Transform playerPos;
    private Player player;
    private NavMeshAgent navMesh;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private GameObject parentObj;
    private bool gimmickFlg = false;    // ギミックオブジェクトに当たったか
    private bool catchFlg = false;      // ギミックオブジェクトを掴んだか
    private int resetNum = -1;
    private int gimmickNum = -1;

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
        animator = GetComponent<Animator>();
        playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        gimmickObj = transform.root.gameObject.GetComponent<GimmickObj>();  // 親オブジェクトのスクリプト取得
        navMesh.speed = data.speed;
        //sphereCollider.radius = search; // colliderのradiusを変更する

        // 巡回ルートに要素があるか
        if (data.rootTransforms.Count >= 1)
        {
            rootNum = 0;
            navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の設定
            // 歩く
            navMesh.speed = data.speed;
        }
        else
        {
            navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
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
    }

    private void Update()
    {
        if (PauseManager.isPaused)
            return;

        if (MySceneManager.GameData.isCatchPenguin) {
            ReStart();
        }
        if (animator)
        {
            animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
    }

    /// <summary>
    /// 飼育員とペンギン、ギミックの当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        #region ペンギン
        if(collision.gameObject.tag == "Player")
        {
            MySceneManager.GameData.isCatchPenguin = true;
            //Debug.Log("捕まえた");
        }
        #endregion

        #region ペンギンブース
        if (collision.gameObject.name == "PenguinBooth")
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
        #endregion
    }

    /// <summary>
    /// 飼育員の索敵範囲にペンギンがいるか
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        #region ペンギン
        if(other.gameObject.tag == "Player")
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
                    if(rayhit.collider == other)
                    {
                        // コルーチン開始
                        StartCoroutine("PenguinChase");
                        navMesh.destination = other.transform.position;    // ペンギンを追従
                    }
                }
                else
                {
                    // Ray当たってない
                    chaseNow = false;
                    surpriseFlg = true;
                    if (gimmickFlg && !catchFlg)
                    {
                        navMesh.SetDestination(gimmickObj.gimmickList[gimmickNum].transform.position);   // 目的地をオブジェクトの位置に設定
                    }
                    if (catchFlg) // オブジェクトを運んでいるか
                    {
                        navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
                    }
                    else if (!gimmickFlg)
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
            }
            else
            {
                // 範囲外
                chaseNow = false;
                surpriseFlg = true;
                if (gimmickFlg && !catchFlg)
                {
                    navMesh.SetDestination(gimmickObj.gimmickList[gimmickNum].transform.position);   // 目的地をオブジェクトの位置に設定
                }
                if (catchFlg) // オブジェクトを運んでいるか
                {
                    navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
                }
                else if(!gimmickFlg)
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
        }
        #endregion

        #region ギミックオブジェクト
        // 索敵範囲にギミックオブジェクトが入ったら目的地をオブジェクトに設定
        if (data.status == Status.returnObj && other.gameObject.tag == "Interact" && !chaseNow)
        {
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (other.gameObject == gimmickObj.gimmickList[i])
                {
                    if (!gimmickObj.bReset[i]) // 元の位置になかったら
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
                                    parentObj = gimmickObj.gimmickList[i].transform.root.gameObject;        // 親オブジェクト取得
                                    navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);   // 目的地をオブジェクトの位置に設定
                                    gimmickFlg = true;
                                    resetNum = i;
                                    gimmickNum = i;
                                }
                            }
                            else
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
                        }
                        else
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
                    }
                }
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
            if(gimmickFlg && !catchFlg)
            {
                navMesh.SetDestination(gimmickObj.gimmickList[gimmickNum].transform.position);   // 目的地をオブジェクトの位置に設定
            }
            if (catchFlg) // オブジェクトを運んでいるか
            {
                navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
            }
            else
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
        #endregion

        #region ギミックオブジェクト
        if (other.gameObject.tag == "Interact" && catchFlg)
        {
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = parentObj.transform;
            catchFlg = false;
            gimmickFlg = false;
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の移動
    /// </summary>
    private void Move()
    {
        #region 移動
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
                    catchFlg = true;
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
                    gimmickFlg = false;
                    catchFlg = false;
                    // コルーチン開始
                    StartCoroutine("FrameStop");
                    navMesh.SetDestination(data.rootTransforms[rootNum].position); // 目的地の再設定
                }
            }
        }
        else if (data.rootTransforms.Count >= 1 && !catchFlg)
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
        else
        {
            navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）

            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", false);
            //--------------------------------
        }
        #endregion
    }

    /// <summary>
    /// ペンギンを追従するコルーチン
    /// </summary>
    private IEnumerator PenguinChase()
    {
        if (surpriseFlg)
        {
            // 驚きモーション中は移動させない
            navMesh.isStopped = true;
            navMesh.speed = 0.0f;
            //-----------------------
            // 驚くアニメーション開始
            animator.SetTrigger("isSurprise");
            //-----------------------
            // エフェクト表示
            exclamationEffect =
                EffectManager.Create(
                    new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z),
                    3);
            surpriseFlg = false;

            yield return new WaitForSeconds(2.0f);
        }

        // 驚くアニメーション終了したらペンギンを追従開始
        chaseNow = true;
        navMesh.isStopped = false;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
        navMesh.speed = data.chaseSpeed * 1.5f; // 巡回スピード * 追いかける速さ           

        // オブジェクトを置く
        gimmickFlg = false;
        if (catchFlg)   // オブジェクトを持ってる時
        {
            catchFlg = false;
            Bring();
        }

        // エフェクト削除
        yield return new WaitForSeconds(2.0f);
        if (exclamationEffect) Destroy(exclamationEffect);
    }

    /// <summary>
    /// オブジェクトを運ぶコルーチン
    /// </summary>
    private IEnumerator CatchObj()
    {
        // エフェクト表示
        questionEffect =
            EffectManager.Create(
                new Vector3(transform.position.x, transform.position.y + 4.0f, transform.position.z),
                4);
        // 止まる
        navMesh.isStopped = true;
        navMesh.speed = 0.0f;
        Bring();

        yield return new WaitForSeconds(1.5f);

        // 動く
        navMesh.isStopped = false;
        navMesh.speed = data.speed;

        // エフェクト削除
        if (questionEffect) Destroy(questionEffect);
    }

    /// <summary>
    /// オブジェクトを元に戻すコルーチン
    /// </summary>
    private IEnumerator FrameStop()
    {
        // 止まる
        navMesh.isStopped = true;
        navMesh.speed = 0.0f;
        //------------------------
        // 置くアニメーション開始（仮で驚くモーション）
        animator.SetTrigger("isSurprise");
        //------------------------

        yield return new WaitForSeconds(1.5f);

        gimmickObj.bReset[gimmickNum] = true;
        Bring();
        // 動く
        navMesh.isStopped = false;
        navMesh.speed = data.speed;
        //------------------------
        // 歩行アニメーション開始
        animator.SetBool("isWalk", true);
        //------------------------
    }

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
            navMesh.SetDestination(gimmickObj.resetPos[gimmickNum].position);   // 目的地をオブジェクトの位置に設定
        }
        else if (gimmickObj.bReset[gimmickNum] || chaseNow)
        {
            // はなす
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = parentObj.transform;
            if (!chaseNow)
            {
                gimmickObj.gimmickList[gimmickNum].transform.position = gimmickObj.resetPos[resetNum].transform.position;
            }
        }
        #endregion
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
    private void ReStart() {
        // インスペクターで設定したリスポーン位置に再配置する
        this.gameObject.transform.position = data.respawnTF.position;
    }

    /// <summary>
    /// 外部で設定したデータを流し込む
    /// </summary>
    /// <param name="_data"></param>
    public void SetData(ZooKeeperData.Data _data) {
        data = _data;
    }

    /// <summary>
    /// ポーズ開始
    /// </summary>
    private void Pause()
    {
        // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
        navMesh.isStopped = true;
        // アニメーション
        animator.speed = 0.0f;
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    private void Resumed()
    {
        // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
        navMesh.isStopped = false;
        // アニメーション
        animator.speed = 1.0f;
    }
}
