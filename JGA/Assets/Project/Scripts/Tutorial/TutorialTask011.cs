//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「そのままお客さんを光っているところまで連れて行こう」
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
public class TutorialTask011 : ITurorial {
    private int oldGuestCnt;    // 前フレームまでの集めた客の人数

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
        oldGuestCnt = MySceneManager.GameData.guestCnt;
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (oldGuestCnt < MySceneManager.GameData.guestCnt) {   // 客のカウントが増えたら
            return true;
        }

        return false;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 3.0f;
    }

}
