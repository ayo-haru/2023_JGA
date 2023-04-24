//=============================================================================
// @File	: [GuestSharedObject.cs]
// @Brief	: 客が使うゲーム内オブジェクト保存用
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/22	スクリプト作成
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
                int index = -1;
                if (targetAnimals[i].name.StartsWith("Penguin")) index = (int)MySceneManager.eRoot.PENGUIN_E;
                if (targetAnimals[i].name.StartsWith("Horse")) index = (int)MySceneManager.eRoot.HORSE;
                if (targetAnimals[i].name.StartsWith("Elephant")) index = (int)MySceneManager.eRoot.ELEPHANT;
                if (targetAnimals[i].name.StartsWith("Lion")) index = (int)MySceneManager.eRoot.LION;
                if (targetAnimals[i].name.StartsWith("PolarBear")) index = (int)MySceneManager.eRoot.POLARBEAR;
                if (targetAnimals[i].name.StartsWith("Bird")) index = (int)MySceneManager.eRoot.BIRD;

                if (index == -1) continue;
                animalsTransform[index - (int)MySceneManager.eRoot.PENGUIN_E].Add(targetAnimals[i].transform);
            }
        }
        if (interactObjects == null)
        {
            interactObjects = new List<BaseObj>();
            //インタラクトタグのオブジェクトでBaseObjコンポーネントを持っているものは配列に追加
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Interact");
            for (int i = 0; i < objects.Length; ++i)
            {
                BaseObj baseObj = objects[i].GetComponent<BaseObj>();
                if (!baseObj) continue;
                interactObjects.Add(baseObj);
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
        int index = -(int)MySceneManager.eRoot.PENGUIN_E;
        if (_name.StartsWith("Penguin")) index += (int)MySceneManager.eRoot.PENGUIN_E;
        if (_name.StartsWith("Horse")) index += (int)MySceneManager.eRoot.HORSE;
        if (_name.StartsWith("Elephant")) index += (int)MySceneManager.eRoot.ELEPHANT;
        if (_name.StartsWith("Lion")) index += (int)MySceneManager.eRoot.LION;
        if (_name.StartsWith("PolarBear")) index += (int)MySceneManager.eRoot.POLARBEAR;
        if (_name.StartsWith("Bird")) index += (int)MySceneManager.eRoot.BIRD;

        if (index < 0) return null;
        if (animalsTransform[index].Count <= 0) return null;

        return animalsTransform[index][Random.Range(0, animalsTransform[index].Count)];
    }

    public List<BaseObj> GetInteractObjects()
    {
        Init();
        return interactObjects;
    }
}
