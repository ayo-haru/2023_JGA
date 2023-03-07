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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZooKeeperAI : MonoBehaviour
{
    //[SerializeField] private GameObject player;
    //private TestPlayer testPlayer;

    [SerializeField] private Animator animator;
    [SerializeField] private List<Transform> rootList;          // 飼育員の巡回ルートのリスト
    private int rootNum = 0;
    [SerializeField, Range(1.1f, 50.0f)] private float speed;    // 飼育員のスピード
    [SerializeField, Range(0.0f, 50.0f)] private float search;   // 飼育員の索敵範囲
    private SphereCollider sphereCollider;

    private NavMeshAgent navMesh;
    private RaycastHit rayhit;

    private GimmickObj gimmickObj;
    private bool gimmickFlg = false;    // ギミックオブジェクトに当たったか
    private bool catchFlg = false;      // ギミックオブジェクトを掴んだか
    private int resetNum = -1;
    private int gimmickNum = -1;


    [SerializeField] GameObject ReSpawnZone;    // リスポーンする位置

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
    {
        // インスペクターで設定したリスポーン位置に初期配置する
        this.gameObject.transform.position = ReSpawnZone.transform.position;

        //testPlayer = player.GetComponent<TestPlayer>();
        sphereCollider = this.GetComponent<SphereCollider>();
        navMesh = GetComponent<NavMeshAgent>();
        //gimmickObj = this.GetComponent<GimmickObj>();
        gimmickObj = transform.root.gameObject.GetComponent<GimmickObj>();  // 親オブジェクトのスクリプト取得
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        navMesh.speed = speed;
        sphereCollider.radius = search; // colliderのradiusを変更する

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
        if (!gimmickFlg && collision.gameObject.tag == "Interact")
        {
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (!gimmickObj.bReset[i]) // 元の位置になかったら
                {
                    if (collision.gameObject.name == gimmickObj.gimmickList[i].name)
                    {
                        // resetPosのnameと同じ位置に戻す
                        for (int j = 0; j < gimmickObj.resetPos.Count; j++)
                        {
                            if (gimmickObj.resetPos[j].name == gimmickObj.gimmickList[i].name)
                            {
                                navMesh.SetDestination(gimmickObj.resetPos[j].position);   // 目的地をオブジェクトの位置に設定
                                gimmickFlg = true;
                                catchFlg = true;
                                resetNum = j;
                                gimmickNum = i;
                                Bring();
                            }
                        }
                    }
                }
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
        // ペンギンとの当たり判定
        if (other.CompareTag("Player"))
        {
            var pos = other.transform.position - transform.position;
            var distance = pos.magnitude;   // 距離
            var direction = pos.normalized; // 方向

            // rayとコライダーが当たっているか
            if(Physics.Raycast(transform.position, direction, out rayhit, distance))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離
            {
                // 当たったオブジェクトがペンギンかどうか
                if(rayhit.collider.gameObject.tag == "Player")
                {
                    navMesh.isStopped = false;
                    navMesh.destination = other.transform.position;    // ペンギンを追従
                }
                else
                {
                    if (gimmickFlg) // オブジェクトを運んでいるか
                    {
                        navMesh.SetDestination(gimmickObj.resetPos[resetNum].position);    // 目的地をオブジェクトの位置に設定
                    }
                    else
                    {
                        navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
                    }
                }
            }
        }
        #endregion

        #region ギミックオブジェクト
        // 索敵範囲にギミックオブジェクトが入ったら目的地をオブジェクトに設定
        if (!gimmickFlg && other.gameObject.tag == "Interact")
        {
            for (int i = 0; i < gimmickObj.gimmickList.Count; i++)
            {
                if (!gimmickObj.bReset[i]) // 元の位置になかったら
                {
                    if (other.gameObject.name == gimmickObj.gimmickList[i].name)
                    {
                        navMesh.SetDestination(gimmickObj.gimmickList[i].transform.position);   // 目的地をオブジェクトの位置に設定
                    }
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
                if(rootList.Count - 1 > rootNum)
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
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = true;   // 物理演算の影響を受けないようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = false;
            gimmickObj.gimmickList[gimmickNum].transform.parent = this.transform;
        }
        else
        {
            // はなす
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().isKinematic = false;   // 物理演算の影響を受けるようにする
            gimmickObj.gimmickList[gimmickNum].GetComponent<Rigidbody>().useGravity = true;
            gimmickObj.gimmickList[gimmickNum].transform.parent = null;
            gimmickObj.gimmickList[gimmickNum].transform.position = gimmickObj.resetPos[resetNum].transform.position;
            gimmickObj.bReset[gimmickNum] = true;
        }
    }


    /// <summary>
    /// 初期配置に戻す
    /// </summary>
    private void ReStart() {
        // インスペクターで設定したリスポーン位置に再配置する
        this.gameObject.transform.position = ReSpawnZone.transform.position;
    }

}
