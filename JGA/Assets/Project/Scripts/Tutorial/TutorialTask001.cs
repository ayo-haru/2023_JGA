//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「目立ちたがりのペンギン君、檻から脱走してしまいました」
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTask001 : ITurorial
{
    private float timer;    // UIをだしてから遷移するまでの時間

    private readonly float MAX_TIME = 3.0f; // 遷移するまでの時間の定数

    // 生成する壁
    private GameObject wallPrefab;
    private GameObject tutorialWall_001;
    private GameObject tutorialWall_002;
    private GameObject tutorialWall_003;
    private GameObject tutorialWall_004;
    private GameObject tutorialWall_005;


    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        timer = MAX_TIME;

        //----- 壁を生成 -----
        wallPrefab = PrefabContainerFinder.Find(MySceneManager.GameData.stageObjDatas, "TutorialWall.prefab");

        tutorialWall_001 = GameObject.Instantiate(wallPrefab, new Vector3(23.5f, 0.0f, -42.0f), Quaternion.Euler(0.0f, -45.0f, 0.0f));
        tutorialWall_001.name = "TutorialWall_001";
        tutorialWall_002 = GameObject.Instantiate(wallPrefab, new Vector3(-46.5f, 0.0f, -18.0f), Quaternion.Euler(0.0f, 45.0f, 0.0f));
        tutorialWall_002.name = "TutorialWall_002";
        tutorialWall_003 = GameObject.Instantiate(wallPrefab, new Vector3(0.0f, 0.0f, -65.5f), Quaternion.Euler(0.0f, 90.0f, 0.0f));
        tutorialWall_003.name = "TutorialWall_003";
        tutorialWall_004 = GameObject.Instantiate(wallPrefab, new Vector3(-38.0f, 0.0f, -39.0f), Quaternion.Euler(0.0f, 135.0f, 0.0f));
        tutorialWall_004.name = "TutorialWall_004";
        tutorialWall_005 = GameObject.Instantiate(wallPrefab, new Vector3(-98.0f, 0.0f, -107.0f), Quaternion.Euler(0.0f, 135.0f, 0.0f));
        tutorialWall_005.name = "TutorialWall_005";
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        timer -= Time.deltaTime;
        if (timer < 0) {
            return true;
        }

        return false;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 1.5f;
    }

}
