//=============================================================================
// @File	: [Test.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : SingletonMonoBehaviour<TestManager>
{
	protected override bool dontDestroyOnLoad { get {return true; } }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

	public void Test()
    {
		Debug.Log("我シングルトンなり");
    }
}

