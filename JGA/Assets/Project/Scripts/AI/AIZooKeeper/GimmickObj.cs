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
    public List<GameObject> gimmickList;    // ギミックリスト
    public List<Vector3> resetPos;          // ギミック初期位置
    public List<Quaternion> resetRot;       // ギミック初期回転
    public List<bool> bReset;               // 元の位置にあるか
    public List<bool> bBring;               // オブジェクトを運んでいるか

    void Awake()
    {
        Vector3 pos;
        Quaternion rot;
        if (gimmickList.Count >= 1)
        {
            // ギミックの初期位置、フラグ取得
            for (int i = 0; i < gimmickList.Count; i++)
            {
                pos = gimmickList[i].transform.position;
                rot = gimmickList[i].transform.rotation;
                resetPos.Add(pos);
                resetRot.Add(rot);
                bReset.Add(true);
                bBring.Add(false);
            }
        }
        else
        {
            Debug.LogWarning("ギミックリストが空です。(GimmickObj.cs)");
        }
    }

    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
    {
        if (gimmickList.Count <= 0) return;
        GimmickPos();
    }

    /// <summary>
    /// ギミックが元の位置にあるか
    /// </summary>
    private void GimmickPos()
    {
        int x1, z1, x2, z2;

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
            else
            {
                bReset[i] = true;
            }
        }
    }
}
