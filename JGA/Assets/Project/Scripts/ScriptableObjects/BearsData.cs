//=============================================================================
// @File	: [BearsData.cs]
// @Brief	: 熊のデータ
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/02	スクリプト作成
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/BearsData")]
public class BearsData : ScriptableObject
{
        public List<BearData> dataList;
        public List<Vector3> rangeList;
        public int rangeButtom;
        

        [Header("到着地点のランダムの半径 m")]
        public float rangeArea;

        [Serializable]
        public class BearData
        {
            [Header("歩く速度 m/s")]
            public float walkSpeed;
            [Header("走る速度 m/s")]
            public float runSpeed;
            [Header("回転する速度(角度)")]
            public float rotateAngle;
            [Header("アイドルの最小時間")]
            public float minIdleTime;
            [Header("アイドルの最大時間")]
            public float maxIdleTime;

        }
    
}
