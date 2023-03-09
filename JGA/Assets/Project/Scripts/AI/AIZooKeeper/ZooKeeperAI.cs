//=============================================================================
// @File	: [ZooKeeperAI.cs]
// @Brief	: 飼育員の仮の処理
// @Author	: MAKIYA MIO
// @Editer	: 
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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class ZooKeeperAI : MonoBehaviour
{
    private enum Status
    {
        returnObj,      // オブジェクトを元に戻す
        notReturnObj,   // オブジェクトを元に戻さない
    }
    [SerializeField] Status status; // ステータスをプルダウン化

    [SerializeField] private Animator animator;
    [SerializeField] private List<Transform> rootList;          // 飼育員の巡回ルートのリスト
    private int rootNum = 0;
    [SerializeField, Range(1.1f, 50.0f)] private float speed;           // 飼育員のスピード
    [SerializeField, Range(1.1f, 2.0f)] private float chaseSpeed;       // 飼育員の追いかけるスピード
    [SerializeField, Range(0.0f, 50.0f)] private float search;          // 飼育員の索敵範囲
    [SerializeField, Range(1.0f, 180.0f)] private float searchAngle;    // 飼育員の索敵範囲の角度
    [SerializeField] private bool chaseNow = false;    // ペンギンを追いかけているフラグ
    private SphereCollider sphereCollider;
    //private float angle = 45.0f;

    private Transform playerPos;
    private NavMeshAgent navMesh;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private bool gimmickFlg = false;    // ギミックオブジェクトに当たったか
    private bool catchFlg = false;      // ギミックオブジェクトを掴んだか
    private int resetNum = -1;
    private int gimmickNum = -1;


    [SerializeField] GameObject ReSpawnZone;    // リスポーンする位置

    /// <summary>
    /// Inspectorから自身の値が変更されたときに自動で呼ばれる
    /// </summary>
    private void OnValidate()
    {
        sphereCollider = this.GetComponent<SphereCollider>();
        sphereCollider.radius = search; // colliderのradiusを変更する
    }

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
    {
        // インスペクターで設定したリスポーン位置に初期配置する
        this.gameObject.transform.position = ReSpawnZone.transform.position;

        if (sphereCollider == null)
        {
            sphereCollider = this.GetComponent<SphereCollider>();
        }
        navMesh = GetComponent<NavMeshAgent>();
        gimmickObj = transform.root.gameObject.GetComponent<GimmickObj>();  // 親オブジェクトのスクリプト取得
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        playerPos = GameObject.FindWithTag("Player").GetComponent<Transform>();
        navMesh.speed = speed;
        //sphereCollider.radius = search; // colliderのradiusを変更する
        
        // オブジェクトを元に戻す/戻さない
        //switch (status)
        //{
        //    case Status.returnObj:
        //        break;
        //    case Status.notReturnObj:
        //        break;
        //}

        // 巡回ルートに要素があるか
        if (rootList.Count >= 1)
        {
            rootNum = 0;
            navMesh.SetDestination(rootList[rootNum].position); // 目的地の設定
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
        // ペンギンを追いかけているか
        if (chaseNow)
        {
            navMesh.speed = speed * chaseSpeed; // 巡回スピード * 追いかける速さ
        }
        else
        {
            navMesh.speed = speed;
        }
        Move();
    }

    private void Update() {
        if (MySceneManager.GameData.isCatchPenguin) {
            ReStart();
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

        #region ギミックオブジェクト
        if (status == Status.returnObj && !gimmickFlg && collision.gameObject.tag == "Interact")
        {
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (!gimmickObj.bReset[i]) // 元の位置になかったら
                {
                    if (collision.gameObject == gimmickObj.gimmickList[i])
                    {
                        // resetPosのnameと同じ位置に戻す
                        navMesh.SetDestination(gimmickObj.resetPos[i].position);   // 目的地をオブジェクトの位置に設定
                        gimmickFlg = true;
                        catchFlg = true;
                        resetNum = i;
                        gimmickNum = i;
                        Bring();
                    }
                }
            }
        }
        #endregion

        #region ペンギンブース
        if (collision.gameObject.name == "PenguinBooth")
        {
            if (rootList.Count >= 1)
            {
                navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
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
            var distance = 10.0f;               // 距離
            var direction = transform.forward;  // 方向
            float targetAngle = Vector3.Angle(this.transform.forward, pos);
            Debug.DrawRay(transform.position, pos * distance, Color.red);

            // 視界の角度内に収まっているかどうか
            if (targetAngle < searchAngle)
            {
                // Rayが当たっているか
                if (Physics.Raycast(transform.position, pos, out rayhit, distance))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離
                {
                    // 当たったオブジェクトがペンギンかどうか
                    if(rayhit.collider == other)
                    {
                        chaseNow = true;
                        navMesh.destination = other.transform.position;    // ペンギンを追従
                    }
                }
                else
                {
                    // Ray当たってない
                    chaseNow = false;
                    if (gimmickFlg) // オブジェクトを運んでいるか
                    {
                        navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
                    }
                    else
                    {
                        if (rootList.Count >= 1)
                        {
                            navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
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
                if (gimmickFlg) // オブジェクトを運んでいるか
                {
                    navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
                }
                else
                {
                    if (rootList.Count >= 1)
                    {
                        navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
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
        if (status == Status.returnObj && !gimmickFlg && other.gameObject.tag == "Interact")
        {
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (!gimmickObj.bReset[i]) // 元の位置になかったら
                {
                    if (other.gameObject == gimmickObj.gimmickList[i])
                    {
                        navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);   // 目的地をオブジェクトの位置に設定
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
            if (gimmickFlg) // オブジェクトを運んでいるか
            {
                navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
            }
            else
            {
                if (rootList.Count >= 1)
                {
                    navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
                }
                else
                {
                    navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 飼育員の移動
    /// </summary>
    private void Move()
    {
        // オブジェクトを元の位置に戻す
        if (gimmickFlg)
        {
            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", true);
            //--------------------------------

            if (navMesh.remainingDistance <= 2.0f    // 目標地点までの距離が2.0ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                gimmickFlg = false;
                catchFlg = false;
                Bring();
                // リストの順番に巡回する
                if (rootList.Count - 1 > rootNum)
                {
                    rootNum += 1;
                }
                else
                {
                    rootNum = 0;
                }
                navMesh.SetDestination(rootList[rootNum].position); // 目的地の再設定
            }
        }
        else if (rootList.Count >= 1)
        {
            // プロトタイプ用-------------------
            // 歩行アニメーション再生
            animator.SetBool("isWalk", true);
            //--------------------------------

            if (navMesh.remainingDistance <= 1.0f    // 目標地点までの距離が1.0ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                // リストの順番に巡回する
                if (rootList.Count - 1 > rootNum)
                {
                    rootNum += 1;
                }
                else
                {
                    rootNum = 0;
                }
                navMesh.SetDestination(rootList[rootNum].position); // 目的地の再設定
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
    }

    /// <summary>
    /// オブジェクトを運ぶ
    /// </summary>
    private void Bring()
    {
        if (catchFlg)
        {
            // 掴む
            //-----------------------------
            // ここで掴むアニメーションで動くと思う
            //-----------------------------
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = true;   // 物理演算の影響を受けないようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = false;
            gimmickObj.gimmickList[gimmickNum].transform.parent = this.transform;
        }
        else
        {
            // はなす
            //-----------------------------
            // ここではなすアニメーションで動くと思う
            //-----------------------------
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = null;
            gimmickObj.gimmickList[gimmickNum].transform.position = gimmickObj.resetPos[resetNum].transform.position;
            gimmickObj.bReset[gimmickNum] = true;
        }
    }

    /// <summary>
    /// 視界範囲内（扇状視界）を可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        Handles.color = new Color(0, 0, 1, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up, 
            Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2.0f, 10.0f);
                                                                                //↑distance
    }

    /// <summary>
    /// 初期配置に戻す
    /// </summary>
    private void ReStart() {
        // インスペクターで設定したリスポーン位置に再配置する
        this.gameObject.transform.position = ReSpawnZone.transform.position;
    }

}
