//=============================================================================
// @File	: [Hold02Tutorial.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/25	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold02Tutorial:ITurorial
{
    private GameObject player;
    private Player _player;


    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void SetTaskObj(GameObject gameObject) {
        player = gameObject;
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        _player = player.GetComponent<Player>();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (_player.IsHold) {
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
