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
using System.Linq;

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
        guestRootPos = new Transform[guestRootPosName.Length];
        for(int i = 0; i < guestRootPosName.Length; ++i)
        {
            GameObject rootPos = GameObject.Find(guestRootPosName[i]);
            guestRootPos[i] = (rootPos) ? rootPos.transform : null;
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
            //客を生成
            GameObject guest = Create(_guestList[i]);
            if (!guest) continue;
            //生成できてたら名前を変更する
            guest.name = _guestList[i].name;
        }
    }

    /// <summary>
    /// ランダム生成客(これ１回呼び出すと一体生成)
    /// </summary>
    private void SpawnRondomGuest()
    {
        //-----データ初期化-----
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

        //-----ルートを決める-----
        //ルートの数を決める
        int rootNum = UnityEngine.Random.Range(2, guestRootMax + 1);
        //ランダムでルートを入れる
        guestData.roots = new GameData.eRoot[rootNum];
        for (int i = 0; i < rootNum; ++i)
        {
            GameData.eRoot root;
            do
            {
                root = (GameData.eRoot)UnityEngine.Random.Range(4, (int)GameData.eRoot.ENTRANCE);
            } while (guestData.roots.Contains(root));
            guestData.roots[i] = root;
        }

        //-----客を生成-----
        GameObject guest = Create(guestData);
        if (!guest) return;
        guest.name = guestData.name + String.Format("{0:D3}", guestSum);

        ++guestSum;
        ++GameData.randomGuestCnt;
    }
    /// <summary>
    /// 客生成処理
    /// </summary>
    private GameObject Create(GuestData.Data data)
    {
        //-----ペンギンブースの座標設定-----
        data.penguinTF = new List<Transform>();
        for(int i = 0; i<= (int)GameData.eRoot.PENGUIN_E; ++i)
        {
            if (!guestRootPos[i]) continue;
            data.penguinTF.Add(guestRootPos[i]);
        }
        if(data.penguinTF.Count <= 0)
        {
            Debug.Log("ペンギンブース取得出来なかったため、生成できませんでした");
            return null;
        }
        //-----エントランスの座標設定-----
        if (guestRootPos[(int)GameData.eRoot.ENTRANCE])
        {
            data.entranceTF = guestRootPos[(int)GameData.eRoot.ENTRANCE];
        }else{
            Debug.Log("エントランスが取得できなかったため、生成出来ませんでした");
            return null;
        }

        // -----指定された列挙定数から目的地のブースの実際のpositionを設定-----
        data.rootTransforms = new List<Transform>();
        for (int j = 0; j < data.roots.Length; j++)
        {
            if (!guestRootPos[(int)data.roots[j]]) continue;
            data.rootTransforms.Add(guestRootPos[(int)data.roots[j]]);
        }
        if(data.rootTransforms.Count <= 0)
        {
            Debug.Log("ルートが設定出来なかったため、生成できませんでした");
            return null;
        }

        //-----生成-----
        GameObject instance = null;
        //プレハブの中からランダムで1つ生成
        int index = UnityEngine.Random.Range(0, guestPrefab.Count);
        instance = Instantiate(guestPrefab[index], data.rootTransforms[0].position, Quaternion.identity);
        //親オブジェクト設定
        if (guestParent)
        {
            instance.transform.SetParent(guestParent.transform);
        }
        //AIマネージャにデータ渡す
        if(instance.TryGetComponent(out AIManager ai))
        {
            ai.SetGuestData(data);
        }else{
            Destroy(instance);
            instance = null;
            Debug.LogError("AIマネージャが取得できなかったため、生成できませんでした");
        }

        return instance;
    }
}
