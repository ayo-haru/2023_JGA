//=============================================================================
// @File	: [BoothAnimalManager.cs]
// @Brief	: ブースの動物の一括管理
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/25	スクリプト作成
// 2023/05/08	名前変更＆親子関係の追加
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoothAnimalManager : SingletonMonoBehaviour<BoothAnimalManager>
{
	protected override bool dontDestroyOnLoad { get {return true; } }

	//ペンギンの生成に使うため
	private GameObject penguinObj;
	//ペンギンの数
	[SerializeField] private int penguinCount;
    //ペンギンの総合データ
    [SerializeField] public PenguinsData penguinsData;
	[NonSerialized] public int penguinStartIndex;

	//熊の生成に使う用
	private GameObject bearObj;
	//熊の数
	[SerializeField] private int bearCount;
	//ペンギンの総合データ
	[SerializeField] public BearsData bearData;
	[NonSerialized] public int bearStartIndex;

	//アニマルオブジェクトを入れる用
	private GameObject animalObject;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		animalObject = GameObject.Find("Animals");
        if (!animalObject)
        {
            Debug.LogWarning("Animalsがシーン上にありません");
            return;
        }
		if(!penguinsData)
		{
            Debug.LogWarning("PenguinsDataがシーン上にありません");
			return;
        }
		if (!bearData)
        {
            Debug.LogWarning("BearsDataがシーン上にありません");
            return;
        }

        penguinObj = PrefabContainerFinder.Find(MySceneManager.GameData.animalDatas, "Penguin.prefab");
       

        for (int i = 0;i < penguinCount;i++)
		{
            penguinStartIndex = UnityEngine.Random.Range(0, penguinsData.rangeList.Count);
            var startPoint = UnityEngine.Random.insideUnitCircle * penguinsData.rangeArea;

			//決めた範囲分動かした場所を決定し返す
			var setVector = new Vector3(penguinsData.rangeList[penguinStartIndex].x + startPoint.x,
										penguinsData.rangeList[penguinStartIndex].y,
										penguinsData.rangeList[penguinStartIndex].z + startPoint.y);

			var penguinObject =  Instantiate(penguinObj, setVector, Quaternion.identity);
            penguinObject.gameObject.name = Rename("Penguin_", i);
			penguinObject.transform.parent = animalObject.transform;
        }

		bearObj = PrefabContainerFinder.Find(MySceneManager.GameData.animalDatas, "Bear.prefab");

		for(int i = 0; i < bearCount;i++) 
		{
			bearStartIndex = UnityEngine.Random.Range(0, bearData.rangeList.Count);
			var startPoint = UnityEngine.Random.insideUnitCircle * bearData.rangeArea;

            var setVector = new Vector3(bearData.rangeList[bearStartIndex].x + startPoint.x,
                                        bearData.rangeList[bearStartIndex].y,
                                        bearData.rangeList[bearStartIndex].z + startPoint.y);

			var bearOBject = Instantiate(bearObj, setVector, Quaternion.identity);
			bearOBject.gameObject.name = Rename("Bear_", i);
            bearOBject.transform.parent = animalObject.transform;
        }


		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

	private string Rename(string nameBase,int Number)
	{
		int[] serialNumber = new int[3];
		int count = 10;
		int i = serialNumber.Length - 1;
		Number++;

		serialNumber[i] = Number % count;
		i--;

        while(i >= 0)
		{
            serialNumber[i] = Number / count;
			count *= 10;
			i--;
        }

		for(i = 0; i < serialNumber.Length; i++)
		{
			nameBase += serialNumber[i].ToString();
		}

		return nameBase;
	}

}

