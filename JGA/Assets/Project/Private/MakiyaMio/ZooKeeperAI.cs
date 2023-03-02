//=============================================================================
// @File	: [ZooKeeperAI.cs]
// @Brief	: 飼育員の仮の処理
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: https://yttm-work.jp/unity/unity_0036.html
//            https://www.sejuku.net/blog/83620
//            https://www.matatabi-ux.com/entry/2021/03/18/100000 
//
// [Date]
// 2023/02/28	スクリプト作成
// 2023/02/28	スピードをスライダーで変更可能にした
// 2023/02/28	Raycastの追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZooKeeperAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private TestPlayer testPlayer;

    [SerializeField] private List<Transform> rootList;          // 飼育員の巡回ルートのリスト
    private int rootNum = 0;
    [SerializeField, Range(1.1f, 2.0f)] private float speed;    // 飼育員のスピード
    [SerializeField, Range(0.0f, 2.0f)] private float search;   // 飼育員の索敵範囲

    private SphereCollider sphereCollider;
    private NavMeshAgent navMesh;
    private RaycastHit rayhit;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
    {
        testPlayer = player.GetComponent<TestPlayer>();
        sphereCollider = this.GetComponent<SphereCollider>();
        navMesh = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        speed *= testPlayer.speed;  // ペンギンの移動速度の最低1.1倍~最高2.0倍
        navMesh.speed = speed;
        sphereCollider.radius = search; // colliderのradiusを変更する
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (rootList.Count >= 1)
        {
            navMesh.SetDestination(rootList[rootNum].position);     // 目的地の設定
            if (navMesh.remainingDistance <= 0.1f    // 目標地点までの距離が0.1ｍ以下になったら到着
                 && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
            {
                rootNum = Random.Range(0, rootList.Count);
                navMesh.SetDestination(rootList[rootNum].position);     // 目的地の再設定
            }
        }
        else
        {
            navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
        }
    }

    /// <summary>
    /// ペンギンと飼育員の当たり判定
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
        }
    }

    /// <summary>
    /// 飼育員の索敵範囲にペンギンがいるかどうか
    /// </summary>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var diff = player.transform.position - transform.position;  // 差分
            var distance = diff.magnitude;      // 距離
            var direction = diff.normalized;    // 方向
            
            // rayとコライダーが当たっているか
            if(Physics.Raycast(transform.position, direction, out rayhit, distance))    // rayの開始地点、rayの向き、当たったオブジェクトの情報を格納、rayの発射距離
            {
                // 当たったオブジェクトがペンギンかどうか
                if(rayhit.transform.gameObject == player)
                {
                    navMesh.isStopped = false;
                    navMesh.destination = player.transform.position;    // ペンギンを追従
                }
                else
                {
                    //navMesh.isStopped = true;   // ナビゲーションの停止（true:ナビゲーションOFF　false:ナビゲーションON）
                    navMesh.SetDestination(rootList[rootNum].position);     // 目的地の設定
                }
            }
        }
    }
}