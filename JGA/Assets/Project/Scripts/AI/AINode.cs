//=============================================================================
// @File	: [AINode.cs]
// @Brief	: AIのノードの定義
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ステートの種類
/// </summary>
public enum EAIState
{
    GUEST_AROUND_WALK,              //客　歩き回る
    GUEST_FOLLOW_PENGUIN,           //客　ペンギンについていく
    GUEST_STAY_PENGUIN_AREA,        //客　ペンギンブースに着いた

    STAFF_AROUND_WALK,              //飼育員　歩き回る
    STAFF_CHASE_PENGUIN,            //飼育員　ペンギン追いかける
    STAFF_CATCH_PENGUIN,            //飼育員　ペンギン捕まえた
    STAFF_GO_TO_GIMICK,             //飼育員　ギミックに向かう
    STAFF_CATCH_GIMICK,             //飼育員　ギミックを片づける

    MAX_AI_STATE,
}
/// <summary>
/// 遷移先と遷移条件
/// </summary>
[System.Serializable]
public struct AIStateTransition
{
    //遷移先
    public EAIState toNode;
    //遷移条件
    public AITransition toNodeTransition;
}
/// <summary>
/// ノード
/// </summary>
[System.Serializable]
public struct AINode
{
    public EAIState stateNum;                       //ステートの識別番号
    public AIState state;                           //ステート

    public List<AIStateTransition> transitions;     //遷移先と遷移条件
}