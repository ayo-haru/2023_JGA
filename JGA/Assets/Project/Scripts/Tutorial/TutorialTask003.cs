//=============================================================================
// @File	: [TutorialTask003.cs]
// @Brief	: 「Lスティックで移動」
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

public class TutorialTask003 : ITurorial
{
    private List<GameObject> needObj = new List<GameObject>();
    private Player _player;


    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
        needObj.Add(gameObject);
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        _player = needObj[0].GetComponent<Player>();    // ここではプレイヤーしか使用しないので0番目を直指定
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (_player.IsMove) {
            return true;
        }
        return false;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 2.0f;
    }
}
