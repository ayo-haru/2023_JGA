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
    private GameObject player;
    private Player _player;

    private readonly float distance = 2.0f;

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
            Debug.LogError("プレイヤーのスクリプトが取得されていません");
            return false;
        }
        
        for(int i = 0; i < _player.InteractObjects.Count; ++i)
        {
            if (_player.InteractObjects[i].StartsWith("CardBoard_")) return true;
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
