//=============================================================================
// @File	: [TestCube.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour
{
	[SerializeField] GameObject objects;
	[SerializeField] List<GameObject> gameObjects = new List<GameObject>();



	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		objects = null;
		gameObjects = GimickObjectManager.Instance.GetGimickObjectAll();

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

		GimickObjectManager.Instance.GetGimickObjectType<Radio>();
		objects = GimickObjectManager.Instance.GetGimickObject<Radio>();

	}
}
