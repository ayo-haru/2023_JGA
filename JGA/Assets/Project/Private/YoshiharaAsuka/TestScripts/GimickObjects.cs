//=============================================================================
// @File	: [GimickObjects.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/16	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GimickobjectsData")]
public class GimickObjectsData : ScriptableObject
{
	public List<BaseObj> scritableGimickObjectsList = new List<BaseObj>();
}
