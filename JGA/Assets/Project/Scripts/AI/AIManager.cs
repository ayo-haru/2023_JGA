//=============================================================================
// @File	: [AIManager.cs]
// @Brief	: AI管理クラス
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/03/02	(小楠)客用のデータ持たせた
// 2023/03/03	(小楠)ステートの終了処理追加
// 2023/03/11	(小楠)乱数の初期化を追加
// 2023/03/23	(小楠)ポーズの処理追加
// 2023/03/25	(伊地田)自動生成、直置き両方に対応
// 2023/03/30	(小楠)penguinTFのリスト化に対応
// 2023/04/07	(小楠)反応する範囲の可視化
// 2023/04/10	(小楠)視界範囲の可視化
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIManager : MonoBehaviour
{
    //ノードリスト
    [SerializeField] private AINode[] nodeList;
    //現在のステート
    private int currentState = 0;

    //ナビメッシュエージェント
    private NavMeshAgent agent;

    [Space(100)]
    [Header("デバッグ用直置きしたプレハブか？\nチェックいれて下のdataを設定すると\n直置きでも使えるよ")]
    [SerializeField] private bool isDebug = false;
    // 使用する客データ
    [SerializeField] private GuestData.Data data;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        //ポーズ時の動作を登録
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(gameObject);
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        //デバッグ用直置きしたとき用のデータセット。
        //自動生成はStageSceneManagerで行っている
        if (isDebug) {
            StageSceneManager _StageSceneManager = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
            data.rootTransforms = new List<Transform>();
            for (int i = 0; i < data.roots.Length; i++) {
                MySceneManager.eRoot index = data.roots[i];
                data.rootTransforms.Add(_StageSceneManager.GetRootTransform(index));
            }

            data.penguinTF = new List<Transform>();
            data.penguinTF.Add(_StageSceneManager.GetRootTransform(MySceneManager.eRoot.PENGUIN_N));
            data.penguinTF.Add(_StageSceneManager.GetRootTransform(MySceneManager.eRoot.PENGUIN_S));
            data.penguinTF.Add(_StageSceneManager.GetRootTransform(MySceneManager.eRoot.PENGUIN_W));
            data.penguinTF.Add(_StageSceneManager.GetRootTransform(MySceneManager.eRoot.PENGUIN_E));

            //表示名変更
            gameObject.name = data.name;
        }

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
        for (int i = 0; i < nodeList[currentState].transitions.Count; ++i)
        {
            nodeList[currentState].transitions[i].toNodeTransition.InitTransition();
        }
        //乱数初期化
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (PauseManager.isPaused) return;
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
#if false
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{

    }
#endif
    /// <summary>
    /// ステートの切り替え
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(EAIState nextState)
    {
        //ステートの終了処理
        nodeList[currentState].state.FinState();
        //ノード切り替え
        int next = GetNodeToState(nextState);
        if (next >= 0) currentState = next;
        //ステート初期化処理
        if (nodeList[currentState].state) nodeList[currentState].state.InitState();
        //遷移条件初期化処理
        for(int i = 0; i < nodeList[currentState].transitions.Count; ++i)
        {
            nodeList[currentState].transitions[i].toNodeTransition.InitTransition();
        }
        //乱数初期化
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
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

    public GuestData.Data GetGuestData()
    {
        return data;
    }
    public void SetGuestData(GuestData.Data _guestData) {
        data = _guestData;
    }


    private void Pause()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (agent) agent.isStopped = true;
        
    }

    private void Resumed()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (agent) agent.isStopped = false;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        //アピール反応エリア
        Handles.color = new Color(0, 1, 0, 0.3f);
        Handles.DrawSolidArc(transform.position,Vector3.up,transform.forward,360.0f,data.reactionArea);
        //音反応エリア
        Handles.color = new Color(0, 0.5f, 0, 0.3f);
        Handles.DrawSolidArc(transform.position + transform.forward * data.soundAreaOffset, Vector3.up, transform.forward, 360.0f, data.reactionArea);
        //視界エリア
        Handles.color = new Color(0, 0, 1, 0.3f);
        Handles.DrawSolidArc(transform.position, Vector3.up,
            Quaternion.Euler(0f, -data.viewAngle, 0f) * transform.forward, data.viewAngle * 2.0f, data.rayLength);
#endif
    }
}
