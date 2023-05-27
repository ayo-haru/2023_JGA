//=============================================================================
// @File	: [GuestSharedObject.cs]
// @Brief	: 客が使うゲーム内オブジェクト保存用
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/22	スクリプト作成
// 2023/05/26	段ボール取得
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestSharedObject : MonoBehaviour
{
    //動物のトランスフォーム
    private List<Transform>[] animalsTransform;
    //インタラクトオブジェクト
    private List<BaseObj> interactObjects;
    //段ボール
    private List<BaseObj> carboardObjects;

    [NamedArrayAttribute(new string[] {"ペンギン","馬","象","ライオン","白熊","鳥"})]
    [SerializeField,Header("TargetAnimalsの名前")] private string[] targetAnimalName = { "Penguin","Horse","Elephant","Lion","Bear","Bird",};
    [NamedArrayAttribute(new string[] {"ペンギン","馬","象","ライオン","白熊","鳥"})]
    [SerializeField,Header("CagePosの名前")] private string[] cagePosName = { "Penguin", "Horse", "Elephant", "Lion", "PolarBear", "Bird", };


#if false
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
#endif

    public void Init()
    {
        //コンポーネント取得
        if (animalsTransform == null)
        {
            animalsTransform = new List<Transform>[(int)MySceneManager.eRoot.ENTRANCE - (int)MySceneManager.eRoot.PENGUIN_E];
            for (int i = 0; i < animalsTransform.Length; ++i)
            {
                if (animalsTransform[i] == null) animalsTransform[i] = new List<Transform>();
            }
            //TargetAnimalsを全て取得し名前に応じて配列に格納する
            GameObject[] targetAnimals = GameObject.FindGameObjectsWithTag("TargetAnimals");
            for (int i = 0; i < targetAnimals.Length; ++i)
            {
                for(int j = 0; j < targetAnimalName.Length; ++j)
                {
                    if (!targetAnimals[i].name.StartsWith(targetAnimalName[j])) continue;
                    animalsTransform[j].Add(targetAnimals[i].transform);
                    break;
                }
            }
        }
        if (interactObjects == null)
        {
            interactObjects = new List<BaseObj>();
            carboardObjects = new List<BaseObj>();
            //インタラクトタグのオブジェクトでBaseObjコンポーネントを持っているものは配列に追加
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Interact");
            for (int i = 0; i < objects.Length; ++i)
            {
                BaseObj baseObj = objects[i].GetComponent<BaseObj>();
                if (!baseObj) continue;
                interactObjects.Add(baseObj);
                CardBoard cardBoard = objects[i].GetComponent<CardBoard>();
                if (!cardBoard) continue;
                carboardObjects.Add(baseObj);
            }
        }
    }

    public Transform GetAnimalTransform(MySceneManager.eRoot _root)
    {
        Init();
        if (_root == MySceneManager.eRoot.ENTRANCE) return null;

        int index = (int)_root - (int)MySceneManager.eRoot.PENGUIN_E;
        if (index < 0) index = 0;

        if (animalsTransform[index].Count <= 0) return null;

        return animalsTransform[index][Random.Range(0, animalsTransform[index].Count)];
    }

    public Transform GetAnimalTransform(string _name)
    {
        Init();
        for(int i = 0; i < cagePosName.Length; ++i)
        {
            if (!_name.StartsWith(cagePosName[i])) continue;
            if (animalsTransform[i].Count <= 0) return null;
            return animalsTransform[i][Random.Range(0, animalsTransform[i].Count)];
        }

        return null;
    }

    public List<BaseObj> GetInteractObjects()
    {
        Init();
        return interactObjects;
    }

    public List<BaseObj> GetCarboardObjects()
    {
        Init();
        return carboardObjects;
    }
}
