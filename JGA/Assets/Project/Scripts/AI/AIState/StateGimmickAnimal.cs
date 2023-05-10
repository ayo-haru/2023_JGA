//=============================================================================
// @File	: [StateGimmickAnimal.cs]
// @Brief	: ギミック動物に夢中になってるときの処理
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/05	スクリプト作成
// 2023/05/10	動物の方向を取得
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateGimmickAnimal : AIState
{
    //animator
    private Animator animator;
    //感情ui
    [SerializeField] private EmosionUI ui;
    //ナビメッシュエージェント
    private NavMeshAgent agent;
    //注目する動物
    [SerializeField] private MySceneManager.eRoot animal = MySceneManager.eRoot.POLARBEAR;
    //動物のTransform
    private Transform animalTransform;
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
        if(!animator) animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        //ケージポス取得
        GetCagePos();
        //動物のトランスフォーム取得
        if (!GetAnimal()) Debug.LogWarning("動物が取得できませんでした");

        if (!ErrorCheck()) return;

        ui.SetEmotion(EEmotion.HIGH_TENSION);
        animator.SetBool("isWalk", true);


    }

    public override void UpdateState()
    {
        if (!ErrorCheck()) return;
        if (agent.velocity.normalized.magnitude > 0.0f) return;
        //動物の方を見る
        Quaternion rot = Quaternion.LookRotation((animalTransform) ? animalTransform.position : agent.destination);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);

        animator.SetBool("isWalk", false);
    }

    public override void FinState()
    {
        if(ui)ui.SetEmotion(EEmotion.NONE);
    }

    public override bool ErrorCheck()
    {
        if (!ui) Debug.LogError("感情UIが設定されていません");
        if (!agent) Debug.LogError("ナビメッシュエージェントが取得されていません");
        if (!animator) Debug.LogError("アニメーターが取得されていません");
        return ui && agent && animator;
    }

    private bool GetAnimal()
    {
        if (animalTransform != null) return false;

        GameObject Object = GameObject.Find("GuestSharedObject");
        if (!Object) return false;
        GuestSharedObject sharedObject = Object.GetComponent<GuestSharedObject>();
        if (!sharedObject) return false;
        animalTransform = sharedObject.GetAnimalTransform(animal);
        return animalTransform;
    }
    private bool GetCagePos()
    {
        if (!agent) return false;
        StageSceneManager manager = GameObject.Find("StageSceneManager").GetComponent<StageSceneManager>();
        if (!manager) return false;
        //ケージポス取得
        Vector3 pos = manager.GetRootTransform(animal).position;
        //ばらけさせる
        pos.x += Random.Range(-10.0f, 10.0f);
        pos.z += Random.Range(-10.0f, 10.0f);
       //ケージポスを目的地に設定
       agent.SetDestination(pos);

        return true;
    }

    public Transform GetTargetAnimal()
    {
        return animalTransform;
    }
}
