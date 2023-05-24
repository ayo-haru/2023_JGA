//=============================================================================
// @File	: [ITurorial.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/24	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurorial
{
    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    void SetTaskObj(GameObject gameObject);

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    void OnTaskSetting();

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    bool CheckTask();


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    float GetTransitionTime();

}
