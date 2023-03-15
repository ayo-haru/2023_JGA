//=============================================================================
// @File	: [PenguinManager.cs]
// @Brief	: ペンギンの情報を一括管理する
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinManager : SingletonMonoBehaviour<PenguinManager>
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
}

