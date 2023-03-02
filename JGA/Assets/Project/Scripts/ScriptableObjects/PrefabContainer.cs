//=============================================================================
// @File	: [PrefabContainer.cs]
// @Brief	: プレハブを登録するためのscriptableobjectをつくる
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SetObj")]
public class PrefabContainer : ScriptableObject {

    [System.Serializable]
    public class Container {
        // 表示名
        public string displayName;

        // ID
        public int ID;

        // 生成するオブジェクト
        public GameObject prefab;
    }


    // 表示するリスト
    [SerializeField]
    public Container[] list = default;
}

