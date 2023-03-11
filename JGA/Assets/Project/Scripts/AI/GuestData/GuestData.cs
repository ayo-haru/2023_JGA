//=============================================================================
// @File	: [GuestData.cs]
// @Brief	: 客のデータ
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
// 2023/03/11	(小楠)ケージとの距離を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CreateGestData")]
public class GuestData : ScriptableObject
{
    //移動速度
    [Range(1,3)] public float speed = 1;
    //視線の長さ
    [Min(1)] public float rayLength = 10.0f;
    //反応する範囲
    [Range(15, 20), Tooltip("ペンギンに反応してくれる範囲(注目度がMAXの時)")] public float reactionArea = 15;
    //ペンギンとの距離
    [Range(2, 4),Tooltip("ペンギンとの距離")] public float distance = 2;
    //感情の変化時間
    [Range(1, 5)] public float firstCoolDownTime = 3.0f;
    [Range(1, 5)] public float secondCoolDownTime = 5.0f;
    //待機時間
    [Min(0)] public float waitTime = 0;
    //ケージとの距離（最大値）
    [Min(1)] public float cageDistance = 10.0f;
}
