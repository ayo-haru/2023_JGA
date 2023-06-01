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
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
	//protected override bool dontDestroyOnLoad
	//{ get { return true; } }

	private static Subject<string> pauseSubject = new Subject<string>();
	private static Subject<string> resumeSubject = new Subject<string>();

	private static bool _isPaused = false;
	public static bool isPaused { get { return _isPaused; } set { _isPaused = value; } }
	private static bool _noMenu = false;
	public static bool NoMenu { get { return _noMenu; } set { _noMenu = value; } }

	[SerializeField] private InputActionReference actionPause;

	public static IObservable<string> OnPaused { get { return pauseSubject; } }
	public static IObservable<string> OnResumed { get { return resumeSubject; } }

	private void Start()
	{
		// Actionイベント登録
		actionPause.action.performed += Pause;

		// Input Actionを有効化
		actionPause.ToInputAction().Enable();
	}

	private void OnDisable()
	{
		// Input Actionを無効化
		actionPause.ToInputAction().Disable();
	}

	private void OnDestroy()
	{
		// Actionイベント消去
		actionPause.action.performed -= Pause;

		// Input Actionを無効化
		actionPause.ToInputAction().Disable();
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