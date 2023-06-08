//=============================================================================
// @File	: [TestGameManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : SingletonMonoBehaviour<TestGameManager>
{

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
		if (Input.GetKeyDown(KeyCode.Space)){
			TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TEST01);
		}
	}
}

