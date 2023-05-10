//=============================================================================
// @File	: [PrefabContainerFinder.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabContainerFinder : MonoBehaviour
{
    private static List<PrefabContainer> prefabList = new List<PrefabContainer>();

    /// <summary>
    /// IDからプレハブを検索
    /// </summary>
    /// <param name="_prefabContainer"></param>
    /// <param name="_ID"></param>
    /// <returns></returns>
    public static GameObject Find(PrefabContainer _prefabContainer, int _ID) {   // IDで検索
        return _prefabContainer.list[_ID].prefab;
    }

    /// <summary>
    /// 名前からプレハブを検索
    /// </summary>
    /// <param name="_prefabContainer"></param>
    /// <param name="_displayName"></param>
    /// <returns></returns>
    public static GameObject Find(PrefabContainer _prefabContainer, string _displayName) { // 名前で検索
        for (int i = 0; i < _prefabContainer.list.Length; i++) {
            if (_prefabContainer.list[i].displayName == _displayName) {
                return _prefabContainer.list[i].prefab;
            }
        }

        Debug.LogError("<color=red>指定されたオブジェクトが見つかりません</color>(PrefabContainerFind)\n");
        return null;
    }
}
