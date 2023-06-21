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
using UnityEditor;

public class TestGameManager : SingletonMonoBehaviour<TestGameManager>
{
	[Header("GimickObjectManager")]
	[SerializeField]
	bool GimickObjectManager = false;
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

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.F1)){
			TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TEST01);
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TEST02);
		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			TestMySceneManager.SubtractScene(TestMySceneManager.SCENE.SCENE_TEST01);
		}

		if (Input.GetKeyDown(KeyCode.F4))
		{
			TestMySceneManager.SubtractScene(TestMySceneManager.SCENE.SCENE_TEST02);
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TESTUI);
		}
#endif
	}

}

