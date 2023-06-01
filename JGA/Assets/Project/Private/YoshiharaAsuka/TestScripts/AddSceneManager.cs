//=============================================================================
// @File	: [AddSceneManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/01	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddSceneManager : SingletonMonoBehaviour<AddSceneManager>
{
	private string[] AddSceneName =
	{
		"TestAddScene1",
		"TestAddScene2",
	};

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	[System.Obsolete]
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			SceneManager.LoadScene(AddSceneName[0], LoadSceneMode.Additive);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			SceneManager.LoadScene(AddSceneName[1], LoadSceneMode.Additive);
		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			SceneManager.UnloadScene(AddSceneName[0]);
		}

		if (Input.GetKeyDown(KeyCode.F4)) {
			SceneManager.UnloadScene(AddSceneName[1]);
		}
	}
}

