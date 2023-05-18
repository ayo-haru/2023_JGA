//=============================================================================
// @File	: [RotateSun.cs]
// @Brief	: 太陽光の動きをするような処理
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/18	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateSun : MonoBehaviour
{
	// 毎秒の回転速度
	[SerializeField]
	private float rotateSpeed = 0.1f;

	// 0時(真っ暗)の時のライト角度
	[SerializeField]
	private Vector3 rotate = new Vector3(270.0f, 330.0f, 0.0f);

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		transform.localRotation = Quaternion.Euler(rotate);

		var rotateX = transform.localEulerAngles.x - 15.0f * DateTime.Now.Hour;

		/* 角度がマイナスの値なら360を足して常に0~360の値になるよ補間 */
		if (rotateX < 0){
			rotateX += 360.0f;
		}
		transform.localEulerAngles = new Vector3(rotateX, transform.localEulerAngles.y, transform.localEulerAngles.z);



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
		transform.Rotate(-Vector3.right * rotateSpeed * Time.deltaTime);
	}
}
