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
    public List<PenguinRange> rangeList;

    [Serializable]
    public class PenguinData
    {
        public float walkSpeed;

        public float runSpeed;

        public float minAppealTime;
        public float maxAppealTime;

        public float swimSpeed;

        public float minIdleTime;
        public float maxIdleTime;
    }
    [Serializable]
    public class PenguinRange
    {
        public Vector3 firstPos;
        public Vector3 secondPos;
        public Vector3 thirdPos;
        public Vector3 endPos;
    }

}
