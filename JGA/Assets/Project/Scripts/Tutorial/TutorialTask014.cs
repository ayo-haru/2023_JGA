//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「お客さんが二人いる。後ろにあった段ボールを使ってみよう」
//              段ボールの近くに行ったら文字フェード
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

public class TutorialTask014 : ITurorial
{
    //0  : プレイヤー
    //1~ : 段ボール
    private List<GameObject> gameObjectList = new List<GameObject>();
    private List<Transform> transformList = new List<Transform>();

    private readonly float distance = 2.0f;

    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
        gameObjectList.Add(gameObject);
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        for(int i = 0; i < gameObjectList.Count; ++i)
        {
            transformList.Add(gameObjectList[i].transform);
        }
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        //プレイヤーと段ボール合わせて２つないと距離を比較できないため
        if (transformList.Count <= 1)
        {
            Debug.LogError("トランスフォームが１つ以下です");
            return false;
        }

        //プレイヤーと段ボールの距離比較
        for(int i = 1; i < transformList.Count; ++i)
        {
            if (Vector3.Distance(transformList[0].position, transformList[i].position) <= distance) return true;
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
