//=============================================================================
// @File	: [ZKManager.cs]
// @Brief	: 飼育員の生成用マネージャー
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/09	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZKManager : SingletonMonoBehaviour<ZKManager>
{
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // シーンを読み込む
        TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TEST01);
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
        ZooKeeperSpawn();
    }

    /// <summary>
    /// 飼育員生成
    /// </summary>
    private void ZooKeeperSpawn()
    {
        GameObject zkObj = PrefabContainerFinder.Find(GameData.characterDatas, "ZooKeeper.prefab");     // プレハブ取得
        GameObject parent = GameObject.Find("ZooKeepers");      // 親オブジェクト取得
        ZooKeeperData.Data[] data = GameData.zooKeeperData[GameData.nowScene - 1].list;

        for (int i = 0; i < data.Length; i++)
        {
            // スポーン座標取得
            GameObject zkSpawn = GameObject.Find(data[i].name + "Spawn");
            if (zkSpawn == null)
            {
                Debug.LogWarning(data[i].name + "Spawn 無し(ZKManager.cs)");
            }
            else
            {
                // スポーン地点取得
                data[i].respawnTF = zkSpawn.GetComponent<Transform>();
            }

            // 巡回ルート座標取得



            // 生成
            if (zkSpawn)
            {
                GameObject gameObject = Instantiate(zkObj, zkSpawn.transform.position, Quaternion.identity);
                // 親変更
                if (parent)
                    gameObject.transform.parent = parent.transform;
                // 名前変更
                gameObject.name = data[i].name;
                //gameObject.GetComponent<ZooKeeperAI>().SetData(data[i]);
            }
        }
    }
}
