//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「缶がある。段ボールの上までもっていこう」
//              客が反応したら
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

public class TutorialTask018 : ITurorial
{
    private List<GameObject> gameobjectList = new List<GameObject>();
    private AIManager guest1 = null;
    private AIManager guest2 = null;

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
        if (gameobjectList.Count < 2) return;
        guest1 = gameobjectList[0].GetComponent<AIManager>();
        guest2 = gameobjectList[1].GetComponent<AIManager>();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if(!guest1 || !guest2)
        {
            Debug.LogError("AIManagerが取得されていません");
            return false;
        }
        return guest1.GetNowState() == EAIState.GUEST_FOLLOW_PENGUIN && guest2.GetNowState() == EAIState.GUEST_FOLLOW_PENGUIN;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 1.5f;
    }

}
