//=============================================================================
// @File	: [Hold01Tutorial.cs]
// @Brief	: 左にコーンがある行ってみよう
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

public class Hold01Tutorial:ITurorial
{
    private GameObject player;
    private Transform playerTF;


    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
        player = gameObject;
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        playerTF = player.GetComponent<Transform>();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (-46.0f < playerTF.position.x && playerTF.position.x < -36.0f) {
            if (-37.0f < playerTF.position.z && playerTF.position.z < -27.0f) {
                return true;
            }
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
