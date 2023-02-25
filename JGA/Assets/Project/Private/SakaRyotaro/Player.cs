//=============================================================================
// @File	: [Player.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/25	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	[SerializeField] private float moveForce;

	[SerializeField] private Rigidbody rb;
	private MyContorller gameInputs;
	private Vector2 moveInputValue;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		// Input Actionインスタンス生成
		gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Player.Move.started += OnMove;
		gameInputs.Player.Move.performed += OnMove;
		gameInputs.Player.Move.canceled += OnMove;
		gameInputs.Player.Call.performed += OnCall;

		// Input Actionを有効化
		gameInputs.Enable();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		// 移動方向の力を与える
		rb.AddForce(new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce);
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		// Moveアクションの入力取得
		moveInputValue = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// 鳴く
	/// </summary>
	private void OnCall(InputAction.CallbackContext context)
	{

	}

}
