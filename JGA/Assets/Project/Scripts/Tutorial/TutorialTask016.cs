//=============================================================================
// @File	: [TutorialTask001.cs]
// @Brief	: 「段ボールをさっきのお客さん二人の後ろまでもっていく」
//              後ろまで段ボールをもってたらフェード
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

public class TutorialTask016 : ITurorial
{
    //0 　: 段ボール
    //1,2 : 客
    private List<GameObject> gameObjectList = new List<GameObject>();
    private List<Transform> transformList = new List<Transform>();
    //客のデータ
    private GuestData.Data data1 = null;
    private GuestData.Data data2 = null;

    /// <summary>
    /// タスク完了に必要となるオブジェクトを設定する
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddNeedObj(GameObject gameObject) {
        gameObjectList.Add(gameObject);
    }

    /// <summary>
    /// チュートリアルタスクが設定されたときに実行
    /// </summary>
    public void OnTaskSetting() {
        for(int i = 0; i < gameObjectList.Count; ++i)
        {
            transformList.Add(gameObjectList[i].transform);
        }
        //客の情報取得
        AIManager ai;
        if (!gameObjectList[1].TryGetComponent(out ai)) return;
        data1 = ai.GetGuestData();
        if (!gameObjectList[2].TryGetComponent(out ai)) return;
        data2 = ai.GetGuestData();
    }

    /// <summary>
    /// タスクが完了したか
    /// </summary>
    /// <returns></returns>
    public bool CheckTask() {
        if(transformList.Count < 3)
        {
            Debug.LogError("必要なトランスフォームが取得されていません");
            return false;
        }
        if(data1 == null || data2 == null)
        {
            Debug.LogError("客用のデータが取得されていません");
            return false;
        }
        //１人目の客の近くに段ボールあるか
        if (Vector3.Distance(transformList[0].position, 
                                transformList[1].position + transformList[1].forward * data1.soundAreaOffset) > 
                                data1.reactionArea) return false;
        //二人目の客の近くに段ボールあるか
        if (Vector3.Distance(transformList[0].position,
                                transformList[2].position + transformList[2].forward * data2.soundAreaOffset) >
                                data2.reactionArea) return false;

        return true;
    }


    /// <summary>
    /// タスク達成後に何秒で次のタスクに遷移するか
    /// </summary>
    /// <returns></returns>
    public float GetTransitionTime() {
        return 1.5f;
    }

}
