//=============================================================================
// @File	: [PenguinsData.cs]
// @Brief	: ペンギンのデータ用
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/PenguinsData")]
public class PenguinsData : ScriptableObject
{
    public List<PenguinData> dataList;
    public List<Vector3> rangeList;
    public List<PenguinUpDownData> updownList;

    [Serializable]
    public class PenguinData
    {
        [Header("歩く速度 m/s")]
        public float walkSpeed;

        public float runSpeed;

        public float minAppealTime;
        public float maxAppealTime;

        public float swimSpeed;

        public float minIdleTime;
        public float maxIdleTime;
    }
    [Serializable]
    public class PenguinUpDownData
    {
        public Vector3 builtStone;
        public Vector3 downStone;
    }
}
