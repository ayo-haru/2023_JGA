//=============================================================================
// @File	: [AIManager.cs]
// @Brief	: AI管理クラス
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/02	(小楠)客用のデータ持たせた
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    //ノードリスト
    [SerializeField] private AINode[] nodeList;
    //現在のステート
    private int currentState = 0;

    //使用する客データ
    [SerializeField] private GuestData data;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{

    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        //エラーチェック
        #region
        if (nodeList.Length <= 0) Debug.LogError("ノードが設定されていません");
        for (int i = 0; i < nodeList.Length - 1; ++i)
        {
            for (int j = i + 1; j < nodeList.Length; ++j)
            {
                if (nodeList[i].stateNum != nodeList[j].stateNum) continue;
                Debug.LogError(i + "番目と" + j + "番目に同じステートが設定されています");
            }
            if (nodeList[i].state) continue;
            Debug.LogWarning(i + "番目のステートの処理が設定されていません");
        }
        #endregion
        currentState = 0;
        if (nodeList[currentState].state) nodeList[currentState].state.InitState();
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        //ステートの切り替え
        for (int i = 0; i < nodeList[currentState].transitions.Count; ++i)
        {
            if (!nodeList[currentState].transitions[i].toNodeTransition.IsTransition()) continue;
            ChangeState(nodeList[currentState].transitions[i].toNode);
            break;
        }


        //ステートの更新処理
        if (nodeList[currentState].state) nodeList[currentState].state.UpdateState();
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{

    }
    /// <summary>
    /// ステートの切り替え
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(EAIState nextState)
    {
        int next = GetNodeToState(nextState);
        if (next >= 0) currentState = next;
        if (nodeList[currentState].state) nodeList[currentState].state.InitState();
        for(int i = 0; i < nodeList[currentState].transitions.Count; ++i)
        {
            nodeList[currentState].transitions[i].toNodeTransition.InitTransition();
        }
    }

    /// <summary>
    /// ステート識別番号から配列の要素番号を取得
    /// </summary>
    /// <param name="nState"></param>
    /// <returns></returns>
    private int GetNodeToState(EAIState nState)
    {
        for (int i = 0; i < nodeList.Length; ++i)
        {
            if (nState != nodeList[i].stateNum) continue;
            return i;
        }

        Debug.LogError("存在しないノードです");
        return -1;
    }

    public GuestData GetGuestData()
    {
        return data;
    }
}
