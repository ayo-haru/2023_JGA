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
	[SerializeField]
	private string[] AddSceneName =
	{
		"TestAddScene1",
		"TestAddScene2",
		"TestManagerScene",
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

#if UNITY_EDITOR
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

		if (Input.GetKeyDown(KeyCode.F5))
		{

			if (SceneManager.GetActiveScene().name == AddSceneName[2])
			{
				Debug.LogError("<color=#fd7e00>This is a scene that has already been added.</color>");
				return;
			}
			else
			{
				SceneManager.LoadScene(AddSceneName[2], LoadSceneMode.Additive);
			}

		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			SceneManager.UnloadScene(AddSceneName[2]);
		}
#endif

	}
}

