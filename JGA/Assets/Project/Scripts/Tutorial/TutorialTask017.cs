//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「段ボールの上におけるモノを探してみよう」
//              缶に触れれるくらいになったら文字フェード
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/05/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTask017 : ITurorial
{
    //0  : プレイヤー
    //1~ : 缶
    private List<GameObject> gameobjectList = new List<GameObject>();
    private List<Transform> transformList = new List<Transform>();

    private float distance = 2.0f;

    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
        gameobjectList.Add(gameObject);
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        for(int i = 0; i < gameobjectList.Count; ++i)
        {
            transformList.Add(gameobjectList[i].transform);
        }
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if(transformList.Count < 2)
        {
            Debug.LogError("必要なトランスフォームが取得されていません");
            return false;
        }

        for(int i = 1; i < transformList.Count; ++i)
        {
            //缶との距離が近いか
            if (Vector3.Distance(transformList[0].position, transformList[i].position) > distance) continue;
            //プレイヤーが缶の方向を向いているか
            if (Vector3.Dot(transformList[0].forward, transformList[i].position - transformList[0].position) < 0) continue;
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
