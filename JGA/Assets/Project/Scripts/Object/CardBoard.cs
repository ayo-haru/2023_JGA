//=============================================================================
// @File	: [CardBoard.cs]
// @Brief	: 段ボールの処理を記載
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/30	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoard : BaseObject
{
	Animator _animator;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		Init();
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		_animator = GetComponent<Animator>();
		Debug.Log(objState);
	}


	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		TestChangeState();
	}



	void TestChangeState()
	{
		if (Input.GetKey(KeyCode.F1))
		{
			objState = (int)OBJState.HIT;
			Debug.Log(objState);
		}

		if (Input.GetKey(KeyCode.F2))
		{
			objState = (int)OBJState.HOLD;
			Debug.Log(objState);
		}

	}
}
