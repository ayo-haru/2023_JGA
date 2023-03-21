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

    [Header("到着地点のランダムの半径 m")]
    public float rangeArea;

    [Serializable]
    public class PenguinData
    {
        [Header("歩く速度 m/s")]
        public float walkSpeed;
        [Header("走る速度 m/s")]
        public float runSpeed;
        [Header("アピールの最小時間")]
        public float minAppealTime;
        [Header("アピールの最大時間")]
        public float maxAppealTime;
        [Header("泳ぐ速度 (まだなし)")]
        public float swimSpeed;
        [Header("アイドルの最小時間")]
        public float minIdleTime;
        [Header("アイドルの最大時間")]
        public float maxIdleTime;
    }
    [Serializable]
    public class PenguinUpDownData
    {
        public Vector3 builtStone;
        public Vector3 downStone;
    }
}
