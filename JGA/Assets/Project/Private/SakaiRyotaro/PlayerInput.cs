//=============================================================================
// @File	: [PlayerInput.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/05	スクリプト作成
//=============================================================================
using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : PlayerAction
{
	private static Subject<string> MoveSubject = new Subject<string>();
	private static Subject<string> AppealSubject = new Subject<string>();
	private static Subject<string> HitSubject = new Subject<string>();
	private static Subject<string> HoldSubject = new Subject<string>();
	private static Subject<string> RunSubject = new Subject<string>();

	[SerializeField] private InputActionReference actionMove;
	[SerializeField] private InputActionReference actionAppeal;
	[SerializeField] private InputActionReference actionHit;
	[SerializeField] private InputActionReference actionHold;
	[SerializeField] private InputActionReference actionRun; //PC only

	public static IObservable<string> OnMove { get { return MoveSubject; } }
	public static IObservable<string> OnAppeal { get { return AppealSubject; } }
	public static IObservable<string> OnHit { get { return HitSubject; } }
	public static IObservable<string> OnHold { get { return HoldSubject; } }
	public static IObservable<string> OnRun { get { return RunSubject; } }

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	public override void AwakeState(PlayerManager pm)
	{
		base.AwakeState(pm);

		//--- Input Actionイベント登録
		actionMove.action.performed += Move;
		actionMove.action.canceled += Move;
		actionAppeal.action.performed += Appeal;
		actionAppeal.action.canceled += Appeal;
		actionHit.action.performed += Hit;
		actionHold.action.performed += Hold;
		actionHold.action.canceled += Hold;
		actionRun.action.performed += Run;
		actionRun.action.canceled += Run;

		// Input Actionを有効化
		actionMove.ToInputAction().Enable();
		actionAppeal.ToInputAction().Enable();
		actionHit.ToInputAction().Enable();
		actionHold.ToInputAction().Enable();
		actionRun.ToInputAction().Enable();
	}

	public void Disable()
	{
		// Input Actionを無効化
		actionMove.ToInputAction().Disable();
		actionAppeal.ToInputAction().Disable();
		actionHit.ToInputAction().Disable();
		actionHold.ToInputAction().Disable();
		actionRun.ToInputAction().Disable();
	}

	public void Enable()
	{
		// Input Actionを無効化
		actionMove.ToInputAction().Enable();
		actionAppeal.ToInputAction().Enable();
		actionHit.ToInputAction().Enable();
		actionHold.ToInputAction().Enable();
		actionRun.ToInputAction().Enable();
	}

	private void OnDestroy()
	{
		actionMove.action.performed -= Move;
		actionMove.action.canceled -= Move;
		actionAppeal.action.performed -= Appeal;
		actionAppeal.action.canceled -= Appeal;
		actionHit.action.performed -= Hit;
		actionHold.action.performed -= Hold;
		actionHold.action.canceled -= Hold;
		actionRun.action.performed -= Run;
		actionRun.action.canceled -= Run;

		Disable();

		actionMove = null;
		actionAppeal = null;
		actionHit = null;
		actionHold = null;
		actionRun = null;
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	public override void StartState()
	{

	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	public override void FixedUpdateState()
	{

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	public override void UpdateState()
	{

	}

	private void Move(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Canceled)
			_playerManager.MoveInputValue = Vector2.zero;
		else
			_playerManager.MoveInputValue = context.ReadValue<Vector2>();
	}

	private void Appeal(InputAction.CallbackContext context)
	{

	}

	private void Hit(InputAction.CallbackContext context)
	{

	}

	private void Hold(InputAction.CallbackContext context)
	{

	}

	private void Run(InputAction.CallbackContext context)
	{

	}
}
