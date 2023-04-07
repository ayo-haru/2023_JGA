//=============================================================================
// @File	: [StateGimmickAnimal.cs]
// @Brief	: ギミック動物に夢中になってるときの処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/05	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateGimmickAnimal : AIState
{
    //感情ui
    [SerializeField] private EmosionUI ui;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    [SerializeField] private MySceneManager.eRoot animal = MySceneManager.eRoot.POLARBEAR;
#if false
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
		
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
#endif
    public override void InitState()
    {
        //コンポーネント取得
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!ErrorCheck()) return;

        ui.SetEmotion(EEmotion.HIGH_TENSION);
        StageSceneManager manager = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
        if (manager)
        {
            Vector3 pos = manager.GetRootTransform(animal).position;
            pos.x += Random.Range(-10.0f, 10.0f);
            pos.z += Random.Range(-10.0f, 10.0f);
            agent.SetDestination(pos);
        }
    }

    public override void UpdateState()
    {
        if (!ErrorCheck()) return;
        //動物の方を見る
        //動物のトランスフォーム取得方法は検討中
        //仮でagentの目的地の方を向かせます
        Quaternion rot = Quaternion.LookRotation(agent.destination);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
    }

    public override void FinState()
    {
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!ui) Debug.LogError("感情UIが設定されていません");
        if (!agent) Debug.LogError("ナビメッシュエージェントが取得されていません");
        return ui && agent;
    }
}