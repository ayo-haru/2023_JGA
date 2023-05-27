//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「これでお客さんを連れてこれた、どんどんお客さんを集めよう」
//              プレイヤーが移動したら文字フェード
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/05/26	スクリプト作成
// 2023/05/27	プレイヤーが移動したらの処理を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTask012 : ITurorial
{
    private GameObject player;
    private Player _player;

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
        _player = player.GetComponent<Player>();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (!_player)
        {
            Debug.LogError("プレイヤースクリプトが取得されていません");
            return false;
        }

        return _player.IsMove;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 1.5f;
    }

}
