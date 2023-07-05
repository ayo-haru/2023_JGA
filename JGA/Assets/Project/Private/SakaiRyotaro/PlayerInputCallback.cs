//=============================================================================
// @File	: [PlayerInputCallback.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/07/03	スクリプト作成
//=============================================================================
using System;
using UniRx;
using UnityEngine.InputSystem;

public class PlayerInputCallback : SingletonMonoBehaviour<PlayerInputCallback>
{
	private static Subject<string> appealSubject = new Subject<string>();
	private static Subject<string> hitSubject = new Subject<string>();
	private static Subject<string> holdSubject = new Subject<string>();
	private static Subject<string> runSubject = new Subject<string>();

	private static bool _isAppeal = false;
	private static bool _isHit = false;
	private static bool _isHold = false;
	private static bool _isRun = false;
	public static bool isAppeal { get { return _isAppeal; } set { _isAppeal = value; } }
	public static bool isHit { get { return _isHit; } set { _isHit = value; } }
	public static bool isHold { get { return _isHold; } set { _isHold = value; } }
	public static bool isRun { get { return _isRun; } set { _isRun = value; } }

	public static IObservable<string> OnAppeal { get { return appealSubject; } }
	public static IObservable<string> OnHit { get { return hitSubject; } }
	public static IObservable<string> OnHold { get { return holdSubject; } }
	public static IObservable<string> OnRun { get { return runSubject; } }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	private void Start()
	{

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{

	}

	public void SetAppeal(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			// アピール開始
			case InputActionPhase.Performed:
				_isAppeal = true;
				break;

			// アピール終了
			case InputActionPhase.Canceled:
				_isAppeal = false;
				break;
			default:
				return;
		}

		Appeal();
	}

	public void SetHit(InputAction.CallbackContext context)
	{
		Hit();
	}

	public void SetHold(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				_isHold = true;
				break;
			case InputActionPhase.Canceled:
				_isHold = false;
				break;
			default:
				return;
		}

		Hold();
	}

	public void SetRun(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				_isRun = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
				_isRun = false;
				break;
			default:
				return;
		}

		Run();
	}

	public static void Appeal()
	{
		appealSubject.OnNext("Appeal");
	}

	public static void Hit()
	{
		hitSubject.OnNext("Hit");
	}

	public static void Hold()
	{
		holdSubject.OnNext("Hold");
	}

	public static void Run()
	{
		runSubject.OnNext("Run");
	}


}

