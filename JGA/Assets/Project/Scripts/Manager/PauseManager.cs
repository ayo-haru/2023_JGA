//=============================================================================
// @File	: [PauseManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/11	スクリプト作成
//=============================================================================
using System;
using System.Diagnostics;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
	protected override bool dontDestroyOnLoad
	{ get { return true; } }

	private static Subject<string> pauseSubject = new Subject<string>();
	private static Subject<string> resumeSubject = new Subject<string>();

	private static bool _isPaused = false;
	public static bool isPaused { get { return _isPaused; } set { _isPaused = value; } }
	private static bool _noMenu = false;
	public static bool NoMenu { get { return _noMenu; } set { _noMenu = value; } }

	private MyContorller gameInputs;            // 方向キー入力取得

	public static IObservable<string> OnPaused { get { return pauseSubject; } }
	public static IObservable<string> OnResumed { get { return resumeSubject; } }

	private void Start()
	{
		// Input Actionインスタンス生成
		gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Menu.Pause.performed += Pause;

		// Input Actionを有効化
		gameInputs.Enable();
	}

	private void OnDisable() {
		// Input Actionを無効化
		//gameInputs.Disable();
	}

	private void Pause(InputAction.CallbackContext context)
	{
		if (FadeManager.fadeMode != FadeManager.eFade.Default)
		{       // フェード中はポーズの開始の入力を受け付けない
			return;
		}

		_isPaused = !_isPaused;

		if (_isPaused)
		{
			Pause();
		}
		else
		{
			Resume();
		}
	}

	public static void Pause()
	{
		pauseSubject.OnNext("pause");
	}

	public static void Resume()
	{
		resumeSubject.OnNext("resume");
	}
}