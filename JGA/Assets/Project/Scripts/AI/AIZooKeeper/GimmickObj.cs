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
    public List<GameObject> gimmickList;    // ギミックオブジェクトリスト
    public List<Transform> resetPos;        // ギミックオブジェクト初期位置
    public List<bool> bReset;               // 元の位置にあるかフラグ
    private int x1;
    private int z1;
    private int x2;
    private int z2;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
        // 元の位置にあるかフラグ
        for (int i = 0; i < gimmickList.Count; i++)
        {
            x1 = (int)gimmickList[i].transform.position.x;
            z1 = (int)gimmickList[i].transform.position.z;
            x2 = (int)resetPos[i].transform.position.x;
            z2 = (int)resetPos[i].transform.position.z;
            if (x1 != x2 || z1 != z2)
            {
                bReset.Add(false);  // 元の位置にない
            }
            else
            {
                bReset.Add(true);   // 元の位置にある
            }
        }
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        // オブジェクトが元の位置にあるか
        for (int i = 0; i < gimmickList.Count; i++)
        {
            x1 = (int)gimmickList[i].transform.position.x;
            z1 = (int)gimmickList[i].transform.position.z;
            x2 = (int)resetPos[i].transform.position.x;
            z2 = (int)resetPos[i].transform.position.z;
            if(x1 != x2 || z1 != z2)
            {
                bReset[i] = false;
            }
        }
    }
}
