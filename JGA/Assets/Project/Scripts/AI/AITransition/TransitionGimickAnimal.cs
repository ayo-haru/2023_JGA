//=============================================================================
// @File	: [TransitionGimickAnimal.cs]
// @Brief	: 遷移条件　ギミック用の動物が存在するか
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/10	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionGimickAnimal : AITransition
{
    //GimickAnimalステート
    [SerializeField] private StateGimmickAnimal aiState;
    private Transform targetAnimal;
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
    public override void InitTransition()
    {
        //コンポーネント、オブジェクト取得
        if (aiState) targetAnimal = aiState.GetTargetAnimal();
    }

    public override bool IsTransition()
    {
        if (!ErrorCheck()) return false;

        //動物が存在するか？
        return !targetAnimal;
    }

    public override bool ErrorCheck()
    {
        if (!aiState) Debug.LogError("ステートが設定されていません");

        return aiState;
    }
}
