//=============================================================================
// @File	: [StateCoolDown.cs]
// @Brief	: ペンギンから離れそうなときの処理
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/03	スクリプト作成
// 2023/03/03	(小楠)UI追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateCoolDown : AIState
{
    [SerializeField,Min(1)] private float rotSpeed = 1.0f;
    [SerializeField] private Transform target;
    private NavMeshAgent agent;
    [SerializeField] private EmosionUI ui;
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
        if (!agent) agent = GetComponent<NavMeshAgent>();
        agent.speed = 0.0f;

        //???のアイコンを出す
        ui.SetEmotion(EEmotion.QUESTION);
    }

    public override void UpdateState()
    {
        //そっぽ向く
        Vector3 dir = transform.position - target.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        rot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
        transform.rotation = rot;

        //プレイヤーから離れる
        agent.velocity = dir.normalized;
    }

    public override void FinState()
    {
        ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        return true;
    }
}
