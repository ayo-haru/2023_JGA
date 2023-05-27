//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「次は左側に行ってみよう」
//              噴水の手前の直進が終わるらへんにいったら文字フェード
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: https://yttm-work.jp/collision/collision_0006.html
// 
// [Date]
// 2023/05/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTask013 : ITurorial
{
    //プレイヤー
    private GameObject player;
    //プレイヤーのTransform
    private Transform playerTransform;
    //プレイヤーの半径
    private readonly float r = 2.0f;
    //ゴール地点
    private readonly Vector3 p0 = new Vector3(-45.42f,1.0f,-64.32f);
    private readonly Vector3 p1 = new Vector3(-64.32f,1.0f,-45.42f);
    //線分のベクトル
    private Vector3 lineStartToEnd;

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
        playerTransform = player.transform;
        lineStartToEnd = p1 - p0;
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if (!playerTransform)
        {
            Debug.LogError("プレイヤーのトランスフォームが取得されていません");
            return false;
        }

        Vector3 center = playerTransform.position;
        //ベクトルの計算
        Vector2 lineStartToCircleCenter = center - p0;
        Vector2 lineEndToCircleCenter = center - p1;

        Vector3 distance = Vector3.Cross(lineStartToEnd, lineStartToCircleCenter);
        return distance.magnitude > r;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 1.5f;
    }

}
