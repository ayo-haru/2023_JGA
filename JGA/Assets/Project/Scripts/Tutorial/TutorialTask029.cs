//=============================================================================
// @File	: [TutorialTask029.cs]
// @Brief	: 「このゲームには、制限時間がある」
// @Author	: Ichida Mai
// @Editer	: Yoshihara Asuka
// @Detail	: 
// 
// [Date]
// 2023/05/26	スクリプト作成
// 2023/05/26	チュートリアル用処理作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTask029 : ITurorial
{
    private float timer;    // UIをだしてから遷移するまでの時間

    private readonly float MAX_TIME = 3.0f; // 遷移するまでの時間の定数

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
        return 0.0f;
    }

}
