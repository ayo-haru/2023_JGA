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
	[SerializeField] private Rigidbody rb;

	[Header("ステータス")]
	[SerializeField, Tooltip("追加速度")]
	private float moveForce = 7;
	[SerializeField, Tooltip("走る際の倍率")]
	private float runMagnification = 1.5f;
	[SerializeField, Tooltip("最高速度")]
	private float maxSpeed = 5;


	[SerializeField] private bool isHold;
	[SerializeField] private bool isRun;

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
		gameInputs.Player.Hit.started += OnHit;
		gameInputs.Player.Hit.performed += OnHit;
		gameInputs.Player.Hit.canceled += OnHit;
		gameInputs.Player.Hold.started += OnHold;
		gameInputs.Player.Hold.performed += OnHold;
		gameInputs.Player.Hold.canceled += OnHold;
		gameInputs.Player.Run.started += OnRun;
		gameInputs.Player.Run.performed += OnRun;
		gameInputs.Player.Run.canceled += OnRun;
		gameInputs.Player.Call.started += OnCall;
		gameInputs.Player.Call.performed += OnCall;
		gameInputs.Player.Call.canceled += OnCall;

		// Input Actionを有効化
		gameInputs.Enable();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		// 制限速度内の場合、移動方向の力を与える
		if (rb.velocity.magnitude < maxSpeed * (isRun ? runMagnification : 1))
			rb.AddForce(new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1));
	}



	/// <summary>
	/// 移動
	/// </summary>
	private void OnMove(InputAction.CallbackContext context)
	{
		moveInputValue = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{

	}

	/// <summary>
	/// つかむ
	/// </summary>
	private void OnHold(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				isHold = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
				isHold = false;
				break;
		}
	}

	/// <summary>
	/// 走る
	/// </summary>
	private void OnRun(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				isRun = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
				isRun = false;
				break;
		}
	}

	/// <summary>
	/// 鳴く
	/// </summary>
	private void OnCall(InputAction.CallbackContext context)
	{

	}

}
