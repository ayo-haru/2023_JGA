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
    private GameObject player;
    private Player _player;

    private readonly string canName = "Can_";
    

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
        if (!player) return;
        _player = player.GetComponent<Player>();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (!_player)
        {
            Debug.LogError("プレイヤーが取得されていません");
            return false;
        }

        //缶がインタラクトできる状態か
        for(int i = 0; i < _player.InteractObjects.Count; ++i)
        {
            if (_player.InteractObjects[i].StartsWith(canName)) return true;
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
