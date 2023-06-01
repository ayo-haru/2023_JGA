//=============================================================================
// @File	: [ZooKeeperData.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/18	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CreateZookeeperData")]
public class ZooKeeperData : ScriptableObject{
    [System.Serializable]
    public class Data {
        [Header("飼育員の名前\n(初期位置のﾌﾟﾚﾊﾌﾞの名前はName+Spawnになる)")]
        public string name = "Zookeeper";
        [Header("飼育員の巡回ルート")]
        public GameData.eRoot[] roots;
        [HideInInspector]　// 巡回ルートのトランスフォーム
        public List<Transform> rootTransforms;
        [HideInInspector] // スポーン位置のトランスフォーム
        public Transform respawnTF;
        [Header("オブジェクトをもとに戻す人か戻さない人か")]
        public ZooKeeperAI.Status status;
        [Header("飼育員のスピード")]
        [Range(1.1f, 2.0f)] public float speed = 1.1f;
        [Header("飼育員の追いかけるスピード")]
        [Range(1.1f, 2.0f)] public float chaseSpeed = 1.1f;
        [Header("飼育員の索敵範囲")]
        [Range(0.0f, 50.0f)] public float search = 15.0f;
        [Header("飼育員の索敵範囲の角度")]
        [Range(1.0f, 180.0f)] public float searchAngle = 45.0f;
        [Header("飼育員の索敵範囲の距離")]
        [Range(1.0f, 50.0f)] public float searchDistance = 10.0f;
    }

    [SerializeField]
    public Data[] list = { new Data {name = "Zookeeper", status = ZooKeeperAI.Status.returnObj ,speed = 5.0f, chaseSpeed = 1.1f, search = 15.0f ,searchAngle = 45.0f, searchDistance = 10.0f } };
}
