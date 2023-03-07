//=============================================================================
// @File	: [GimmickObj.cs]
// @Brief	: ギミックオブジェクトのリスト
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/06	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickObj : MonoBehaviour
{
    [SerializeField] private List<GameObject> gimmickObj;
    public List<GameObject> gimmickList;      // ギミックオブジェクトの位置

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        for (int i = 0; i < gimmickObj.Count; i++)
        {
            gimmickList.Add(Instantiate(gimmickObj[i].gameObject, gimmickObj[i].transform.position, Quaternion.identity));
            gimmickList[i].name = gimmickObj[i].name;   // 名前を同じにする
        }
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
}
