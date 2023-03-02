//=============================================================================
// @File	: [GuestData.cs]
// @Brief	: 客のデータ
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateGestData")]
public class GuestData : ScriptableObject
{
    //移動速度
    [Range(1,3)] public float speed = 1;
    //ペンギンへの関心度
    [Range(1, 3),Tooltip("ペンギンへの関心度")] public int interest = 1;
    //反応する範囲
    [Range(6, 20), Tooltip("ペンギンに反応してくれる範囲")] public float reactionArea = 6;
    //ペンギンとの距離
    [Range(2, 5),Tooltip("ペンギンとの距離")] public float distance = 2;
    //待機時間
    [Min(0)] public float waitTime = 0;
}
