//=============================================================================
// @File	: [ZooData.cs]
// @Brief	: 動物の種類
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: https://youtu.be/E5NSgXNgKvY
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZooData", menuName = "自作データ/ZooData")]  // 右クリックでデータが作成できるようになる
public class ZooData : ScriptableObject
{
   new public string name;          // 動物の名前
   public float DesireUpSpeed;      // 欲求の上がる速度
   public float DesireDownSpeed;    // 欲求の下がる速度
}
