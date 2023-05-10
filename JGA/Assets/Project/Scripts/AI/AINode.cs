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
    GUEST_ATTENSION_PENGUIN,        //客　ペンギンに注目する
    GUEST_FOLLOW_PENGUIN,           //客　ペンギンについていく
    GUEST_STAY_PENGUIN_AREA,        //客　ペンギンブースに着いた
    GUEST_STOP_FOLLOW,              //客　追従やめる
    GUEST_STAY_ANIMAL_GIMICK,       //客　動物用ギミック
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
