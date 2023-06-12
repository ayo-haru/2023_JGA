//=============================================================================
// @File	: [GuestManager.cs]
// @Brief	: 客マネージャ
// @Author	: OgusuYuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GuestManager : MonoBehaviour
{
    //客一覧
    private List<GameObject> guestList = new List<GameObject>();
    //客のプレハブ
    private List<GameObject> guestPrefab = new List<GameObject>();
    private string[] guestPrefabName = { "Guest001.prefab" , "Guest002.prefab" , "Guest003.prefab" ,};
    //客の巡回ルート
    private Transform[] guestRootPos;
    private string[] guestRootPosName = {
        "PenguinCagePos_N", "PenguinCagePos_S", "PenguinCagePos_W", "PenguinCagePos_E",
        "RestSpotPos01" , "RestSpotPos02" ,
        "HorseCagePos01" , "HorseCagePos02" , "HorseCagePos03" ,
        "ZebraCagePos01" , "ZebraCagePos02" , "ZebraCagePos03" ,
        "PolarBearCagePos" ,
        "BearCagePos01" , "BearCagePos02" ,
        "PandaCagePos" ,
        "EntrancePos" ,};
    //客の生成人数（ランダムデータ）
    private int guestSum = 0;
    //客の親オブジェクト
    private GameObject guestParent;

    [SerializeField] private int guestRootMax = 3;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        // 生成する客のプレハブ
        for(int i = 0; i < guestPrefabName.Length; ++i)
        {
            guestPrefab.Add(PrefabContainerFinder.Find(GameData.characterDatas, guestPrefabName[i]));
        }
        //客のルート取得
        for(int i = 0; i < guestRootPosName.Length; ++i)
        {
            GameObject rootPos = GameObject.Find(guestRootPosName[i]);
            if (!rootPos) continue;
            guestRootPos[i] = rootPos.transform;
        }
        //親取得
        guestParent = GameObject.Find("Guests");
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
    /// <summary>
    /// 客の追加
    /// </summary>
    /// <param name="guest"></param>
    public void AddGuest(GameObject guest)
    {
        guestList.Add(guest);
    }

    /// <summary>
    /// 固定客生成
    /// </summary>
    private void SpawnFixGuest()
    {
        GuestData.Data[] _guestList = GameData.guestData[GameData.nowScene - 1].dataList;   // 設定された人数分生成する

        for (int i = 0; i < _guestList.Length; i++)
        {
            // 指定された列挙定数から目的地のブースの実際のpositionを設定
            _guestList[i].rootTransforms = new List<Transform>();
            for (int j = 0; j < _guestList[i].roots.Length; j++)
            {
                _guestList[i].rootTransforms.Add(guestRootPos[(int)_guestList[i].roots[j]]);
            }
            // ペンギンブースの座標を入れる
            _guestList[i].penguinTF = new List<Transform>();
            _guestList[i].penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_N]);
            _guestList[i].penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_S]);
            _guestList[i].penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_W]);
            _guestList[i].penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_E]);
            // エントランスの座標を入れる
            _guestList[i].entranceTF = guestRootPos[(int)GameData.eRoot.ENTRANCE];

            // 生成
            GameObject guestInstace;
            int GuestIndex = UnityEngine.Random.Range(0, 3);
            guestInstace = Instantiate(guestPrefab[GuestIndex], _guestList[i].rootTransforms[0].position, Quaternion.identity);
            if (guestParent)
            {
                guestInstace.transform.SetParent(guestParent.transform);
            }
            guestInstace.name = _guestList[i].name; // 表示名変更

            // データの流し込み
            if(guestInstace.TryGetComponent(out AIManager ai))
            {
                ai.SetGuestData(_guestList[i]);
            }
        }
    }

    /// <summary>
    /// ランダム生成客(これ１回呼び出すと一体生成)
    /// </summary>
    private void SpawnRondomGuest()
    {
        GuestData.Data guestData = new GuestData.Data
        { 
            name = "FGuest",
            entranceTF = guestRootPos[(int)GameData.eRoot.ENTRANCE],
            isRandom = true,
            speed = 3,
            followSpeed = 0.7f,
            inBoothSpeed = 0.5f,
            rayLength = 10.0f,
            viewAngle = 60.0f,
            reactionArea = 25,
            distance = 2,
            firstCoolDownTime = 3.0f,
            secondCoolDownTime = 5.0f,
            waitTime = 0,
            cageDistance = 10.0f
        };

        // ペンギンブースの座標を入れる
        guestData.penguinTF = new List<Transform>();
        guestData.penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_N]);
        guestData.penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_S]);
        guestData.penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_W]);
        guestData.penguinTF.Add(guestRootPos[(int)GameData.eRoot.PENGUIN_E]);

        // エントランスの座標を入れる
        guestData.entranceTF = guestRootPos[(int)GameData.eRoot.ENTRANCE];

        //----- 目的地のブースをランダムで設定(固定客は作ってない) -----
        int randomRootSum, randomRootNum;

        // ルートをいくつ設定するかをランダムで決定
        randomRootSum = UnityEngine.Random.Range(2, guestRootMax + 1);    // ランダムの最大値は1大きくしないと設定した数が含まれない

        guestData.rootTransforms = new List<Transform>();
        for (int j = 0; j < randomRootSum; j++)
        {   //　乱数で算出したルートの数だけルートを決める
            // ルートをランダムで設定
            do
            {
                randomRootNum = UnityEngine.Random.Range(4, (int)GameData.eRoot.ENTRANCE);  // ペンギンのルートが入っているところより大きく、エントランスより小さく
            } while (guestData.rootTransforms.Contains(guestRootPos[randomRootNum]));
            guestData.rootTransforms.Add(guestRootPos[randomRootNum]);
        }

        //----- 生成 -----
        // それぞれのカウントを加算
        guestSum++;
        GameData.randomGuestCnt++;

        GameObject guestInstace;
        int GuestIndex = UnityEngine.Random.Range(0, 3);
        guestInstace = Instantiate(guestPrefab[GuestIndex], guestRootPos[(int)GameData.eRoot.ENTRANCE].position, Quaternion.identity);

        if (guestParent)
        {
            guestInstace.transform.parent = guestParent.transform;   // 親にする
        }
        guestInstace.name = guestData.name + String.Format("{0:D3}", guestSum); // 表示名変更
                                                                                // データの流し込み
        guestInstace.GetComponent<AIManager>().SetGuestData(guestData);
    }


}
