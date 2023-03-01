//=============================================================================
// @File	: [TestNavMesh.cs]
// @Brief	: お試しNavMesh
// @Author	: MAKIYTA MIO
// @Editer	: 
// @Detail	: https://youtu.be/E5NSgXNgKvY
// 
// [Date]
// 2023/02/26	スクリプト作成
// 2023/02/27	navMeshを使って移動する処理の追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMesh : MonoBehaviour
{
    [SerializeField] private List<ZooData> zooDatabase = new List<ZooData>();    // ZooData取得

    [SerializeField] private Transform goalPosition;

    [SerializeField] private Transform penguinPosition;     // ペンギンエリアの座標
    [SerializeField] private Transform bearPosition;        // クマエリアの座標
    [SerializeField] private Transform dolphinPosition;     // イルカエリアの座標
    [SerializeField] private Transform fishPosition;        // 魚エリアの座標

    private NavMeshAgent navMesh;

    enum statezoo
    {
        donothing,  // 何も見ていない
        goToPenguin,// ペンギンの場所
        penguin,    // ペンギン
        goToBear,   // クマの場所
        bear,       // クマ
        goToDolphin,// イルカの場所
        dolphin,    // イルカ
        goToFish,   // 魚の場所
        fish,       // 魚
    }

    enum desiretype
    {
        penguin,    // ペンギン
        bear,       // クマ
        dolphin,    // イルカ
        fish,       // 魚
    }

    class Desire
    {
        public desiretype desiretype { get; private set; }   // 欲求の種類
        public float value;     // 欲求の値

        public Desire(desiretype _desiretype)
        {
            desiretype = _desiretype;
            value = 0.0f;
        }
    }

    // Desireクラスのリスト
    class Desires
    {
        public List<Desire> desireList { get; private set; } = new List<Desire>();

        // desiretypeの参照
        public Desire GetDesire(desiretype desiretype)
        {
            foreach(Desire desire in desireList)
            {
                if(desire.desiretype == desiretype)
                {
                    return desire;
                }
            }
            return null;
        }

        // 欲求値が高い順にソートする
        public void SortDesire()
        {
            desireList.Sort((desire1, desire2) => desire2.value.CompareTo(desire1.value));  // インデックス0が一番大きい欲求になる
        }

        // コンストラクタ
        public Desires()
        {
            int desireNum = System.Enum.GetNames(typeof(desiretype)).Length;    // 要素数取得
            for(int i = 0; i < desireNum; i++)
            {
                desiretype desiretype = (desiretype)System.Enum.ToObject(typeof(desiretype), i);
                Desire newDesire = new Desire(desiretype);
                desireList.Add(newDesire);
            }
        }
    }

    Desires desires = new Desires();

    // ペンギン
    [SerializeField] private float penguinDesireUpSpeed = 15f;    // 欲求の上がる速度
    [SerializeField] private float penguinDesireDownSpeed = 5f;   // 欲求の下がる速度

    // クマ
    [SerializeField] private float bearDesireUpSpeed = 8f;    // 欲求の上がる速度
    [SerializeField] private float bearDesireDownSpeed = 2f;  // 欲求の下がる速度

    // イルカ
    [SerializeField] private float dolphinDesireUpSpeed = 20f;    // 欲求の上がる速度
    [SerializeField] private float dolphinDesireDownSpeed = 5f;   // 欲求の下がる速度

    // 魚
    [SerializeField] private float fishDesireUpSpeed = 25f;    // 欲求の上がる速度
    [SerializeField] private float fishDesireDownSpeed = 5f;   // 欲求の下がる速度

    statezoo currentzoo = statezoo.donothing;
    bool zooenter = true;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        navMesh = GetComponent<NavMeshAgent>();
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if(currentzoo != statezoo.penguin)
        {
            desires.GetDesire(desiretype.penguin).value += Time.deltaTime / penguinDesireUpSpeed;
        }
        if(currentzoo != statezoo.bear)
        {
            desires.GetDesire(desiretype.bear).value += Time.deltaTime / bearDesireUpSpeed;
        }
        if(currentzoo != statezoo.dolphin)
        {
            desires.GetDesire(desiretype.dolphin).value += Time.deltaTime / dolphinDesireUpSpeed;
        }
        if(currentzoo != statezoo.fish)
        {
            desires.GetDesire(desiretype.fish).value += Time.deltaTime / fishDesireUpSpeed;
        }

        switch(currentzoo)
        {
            case statezoo.donothing:
                if(zooenter)
                {
                    zooenter = false;

                    navMesh.SetDestination(goalPosition.position);
                }
                ChoiceAction();
                break;

            #region ペンギン
            case statezoo.goToPenguin:
                if(zooenter)
                {
                    zooenter = false;

                    navMesh.SetDestination(penguinPosition.position);
                }

                if(navMesh.remainingDistance <= 0.1f    // 目標地点までの距離が0.1ｍ以下になったら到着
                    && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    ChangeStateZoo(statezoo.penguin);
                    return;
                }
                break;

            case statezoo.penguin:  // ペンギン
                if (zooenter)
                {
                    zooenter = false;

                    //ZooData penguinData = zooDatabase[0];  // ZooDataのペンギンデータ取得

                    desires.GetDesire(desiretype.penguin).value = 1;    // 欲求の値を1で固定
                }
                desires.GetDesire(desiretype.penguin).value -= Time.deltaTime / penguinDesireDownSpeed;
                if(desires.GetDesire(desiretype.penguin).value <= 0)
                {
                    if(!ChoiceAction())
                    {
                        ChangeStateZoo(statezoo.donothing);
                    }
                }
                break;
            #endregion

            #region クマ
            case statezoo.goToBear:
                if (zooenter)
                {
                    zooenter = false;

                    navMesh.SetDestination(bearPosition.position);
                }

                if (navMesh.remainingDistance <= 0.1f    // 目標地点までの距離が0.1ｍ以下になったら到着
                    && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    ChangeStateZoo(statezoo.bear);
                    return;
                }
                break;

            case statezoo.bear:  // クマ
                if (zooenter)
                {
                    zooenter = false;
                    desires.GetDesire(desiretype.bear).value = 1;    // 欲求の値を1で固定
                }
                desires.GetDesire(desiretype.bear).value -= Time.deltaTime / bearDesireDownSpeed;
                if(desires.GetDesire(desiretype.bear).value <= 0)
                {
                    if (!ChoiceAction())
                    {
                        ChangeStateZoo(statezoo.donothing);
                    }
                }
                break;
            #endregion

            #region イルカ
            case statezoo.goToDolphin:
                if (zooenter)
                {
                    zooenter = false;

                    navMesh.SetDestination(dolphinPosition.position);
                }

                if (navMesh.remainingDistance <= 0.1f    // 目標地点までの距離が0.1ｍ以下になったら到着
                    && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    ChangeStateZoo(statezoo.dolphin);
                    return;
                }
                break;

            case statezoo.dolphin:  // イルカ
                if (zooenter)
                {
                    zooenter = false;
                    desires.GetDesire(desiretype.dolphin).value = 1;    // 欲求の値を1で固定
                }
                desires.GetDesire(desiretype.dolphin).value -= Time.deltaTime / dolphinDesireDownSpeed;
                if(desires.GetDesire(desiretype.dolphin).value <= 0)
                {
                    if (!ChoiceAction())
                    {
                        ChangeStateZoo(statezoo.donothing);
                    }
                }
                break;
            #endregion

            #region 魚
            case statezoo.goToFish:
                if (zooenter)
                {
                    zooenter = false;

                    navMesh.SetDestination(fishPosition.position);
                }

                if (navMesh.remainingDistance <= 0.1f    // 目標地点までの距離が0.1ｍ以下になったら到着
                    && !navMesh.pathPending)            // 経路計算中かどうか（計算中：true　計算完了：false）
                {
                    ChangeStateZoo(statezoo.fish);
                    return;
                }
                break;

            case statezoo.fish:  // 魚
                if (zooenter)
                {
                    zooenter = false;
                    desires.GetDesire(desiretype.fish).value = 1;    // 欲求の値を1で固定
                }
                desires.GetDesire(desiretype.fish).value -= Time.deltaTime / fishDesireDownSpeed;
                if(desires.GetDesire(desiretype.fish).value <= 0)
                {
                    if (!ChoiceAction())
                    {
                        ChangeStateZoo(statezoo.donothing);
                    }
                }
                break;
                #endregion
        }
    }

    /// <summary>
    /// ステートの変更をする関数
    /// </summary>
    private void ChangeStateZoo(statezoo newState)
    {
        currentzoo = newState;  // ステート更新
        zooenter = true;        // たった今ステートが切り替わりました
    }

    /// <summary>
    /// プレイヤーの行動を決める関数
    /// </summary>
    private bool ChoiceAction()
    {
        desires.SortDesire();   // ソートする
        if (desires.desireList[0].value >= 1)
        {
            Desire desire = desires.desireList[0];
            switch (desire.desiretype)
            {
                case desiretype.penguin:
                    ChangeStateZoo(statezoo.goToPenguin);
                    return true;
                case desiretype.bear:
                    ChangeStateZoo(statezoo.goToBear);
                    return true;
                case desiretype.dolphin:
                    ChangeStateZoo(statezoo.goToDolphin);
                    return true;
                case desiretype.fish:
                    ChangeStateZoo(statezoo.goToFish);
                    return true;
            }
        }
        return false;
    }
 
}
