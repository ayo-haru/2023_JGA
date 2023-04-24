//=============================================================================
// @File	: [ResultCamera.cs]
// @Brief	: カメラの処理
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/22	スクリプト作成
// 2023/04/22	カメラの切り替え実装
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCamera : MonoBehaviour
{
	private GameObject mainCamera;
	private GameObject resultCamera;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		mainCamera = GameObject.Find("CameraParent");
		resultCamera = GameObject.Find("ResultCamera");

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

	private void ChangeCamera()
	{
		
		mainCamera.SetActive(!mainCamera.activeSelf);
        resultCamera.SetActive(!resultCamera.activeSelf);

	}

}
