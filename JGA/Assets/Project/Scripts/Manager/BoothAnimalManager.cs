//=============================================================================
// @File	: [BoothAnimalManager.cs]
// @Brief	: ブースの動物の一括管理
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/25	スクリプト作成
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		penguinObj = PrefabContainerFinder.Find(MySceneManager.GameData.animalDatas, "Penguin.prefab");

        for (int i = 0;i < penguinCount;i++)
		{
            penguinStartIndex = UnityEngine.Random.Range(0, penguinsData.rangeList.Count);
            var startPoint = UnityEngine.Random.insideUnitCircle * BoothAnimalManager.Instance.penguinsData.rangeArea;

			//決めた範囲分動かした場所を決定し返す
			var setVector = new Vector3(penguinsData.rangeList[penguinStartIndex].x + startPoint.x,
										penguinsData.rangeList[penguinStartIndex].y,
										penguinsData.rangeList[penguinStartIndex].z + startPoint.y);

			Instantiate(penguinObj, setVector, Quaternion.identity);

        }
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
}

