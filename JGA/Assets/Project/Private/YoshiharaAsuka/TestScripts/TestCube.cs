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



	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		objects = null;

	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{

		if (objects == null){
			Debug.Log(objects + "is not Fined");
		}

		if (Input.GetKeyDown(KeyCode.L)){
			objects = GameObject.Find("Sphere");
		}

		if (Input.GetKeyDown(KeyCode.O)){
			objects.transform.Rotate(0, 5, 0);
		}
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
}
