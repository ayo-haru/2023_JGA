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
    public static GameObject Find(ref PrefabContainer _prefabContainer, string _displayName) { // 名前で検索
        ContainerNullCheck(_prefabContainer);

        for (int i = 0; i < _prefabContainer.list.Length; i++) {
            if (_prefabContainer.list[i].displayName == _displayName) {
                return _prefabContainer.list[i].prefab;
            }
        }

        Debug.LogError("<color=red>指定されたオブジェクトが見つかりません</color>(PrefabContainerFind)\n");
        return null;
    }

    private static void ContainerNullCheck(PrefabContainer _prefabContainer) {
        if (_prefabContainer == null) {
            //----- ScriptableObjectの登録したデータの読み込み -----
            //---オブジェクト
            GameData.characterDatas = AddressableLoader<PrefabContainer>.Load("CharacterData");
            //GameData.UIDatas = AddressableLoader<PrefabContainer>.Load("UIData");
            GameData.animalDatas = AddressableLoader<PrefabContainer>.Load("AnimalData");
            //GameData.gimmickDatas = AddressableLoader<PrefabContainer>.Load("GimmickData");
            GameData.stageObjDatas = AddressableLoader<PrefabContainer>.Load("StageObjData");
            //---データ
            GameData.zooKeeperData = new ZooKeeperData[2];
            GameData.zooKeeperData[0] = AddressableLoader<ZooKeeperData>.Load("Stage01_ZooKeeperData");
            GameData.zooKeeperData[1] = AddressableLoader<ZooKeeperData>.Load("Stage02_ZooKeeperData");
            GameData.guestData = new GuestData[2];
            GameData.guestData[0] = AddressableLoader<GuestData>.Load("Stage01_GuestData");
            GameData.guestData[1] = AddressableLoader<GuestData>.Load("Stage02_GuestData");
        }
    }
}
