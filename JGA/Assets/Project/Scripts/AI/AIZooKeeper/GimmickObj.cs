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
    public List<Vector3> resetPos;        // ギミックオブジェクト初期位置
    public List<bool> bReset;               // 元の位置にあるかフラグ
    private int x1;
    private int z1;
    private int x2;
    private int z2;
    private Vector3 pos;

    void Awake()
    {
        if (gimmickList.Count >= 1)
        {
            // 初期位置取得
            for (int i = 0; i < gimmickList.Count; i++)
            {
                pos = gimmickList[i].transform.position;
                resetPos.Add(pos);
                bReset.Add(true);
            }
        }
        else
        {
            Debug.LogWarning("ギミックリストが空です。(GimmickObj.cs)");
        }
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
        if (gimmickList.Count >= 1)
        {
            // オブジェクトが元の位置にあるか
            for (int i = 0; i < gimmickList.Count; i++)
            {
                x1 = Mathf.FloorToInt(gimmickList[i].transform.position.x);
                z1 = Mathf.FloorToInt(gimmickList[i].transform.position.z);
                x2 = Mathf.FloorToInt(resetPos[i].x);
                z2 = Mathf.FloorToInt(resetPos[i].z);
                if (x1 != x2 || z1 != z2)
                {
                    bReset[i] = false;
                }
            }
        }
    }
}
