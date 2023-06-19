//=============================================================================
// @File	: [PauseManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/03/11	スクリプト作成
// 2023/03/16	シーン遷移直後ポーズがかからないの修正
//				フェードにポーズの処理が乗っかっていたので切り離し
//=============================================================================
using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : SingletonMonoBehaviour<PauseManager> {
    //protected override bool dontDestroyOnLoad
    //{ get { return true; } }

    private static Subject<string> pauseSubject = new Subject<string>();
    private static Subject<string> resumeSubject = new Subject<string>();

    private static bool _isPaused = false;
    public static bool isPaused { get { return _isPaused; } set { _isPaused = value; } }
    private static bool _noMenu = false;
    public static bool NoMenu { get { return _noMenu; } set { _noMenu = value; } }

    [SerializeField] private InputActionReference actionPause;

    //フェードを監視
    private ReactiveProperty<FadeManager.eFade> fadeMode = new ReactiveProperty<FadeManager.eFade>(FadeManager.eFade.Default);

    public static IObservable<string> OnPaused { get { return pauseSubject; } }
    public static IObservable<string> OnResumed { get { return resumeSubject; } }

    private void Start() {
        // Actionイベント登録
        actionPause.action.performed += OnPause;

        // Input Actionを有効化
        actionPause.ToInputAction().Enable();

        // フェード用変数初期化
        fadeMode.Value = FadeManager.fadeMode;
        fadeMode.Subscribe(_ => { 
            if (fadeMode.Value == FadeManager.eFade.Default) {
                isPaused = false;
                NoMenu = false;
                Resume();
            }
        }).AddTo(this);
    }

    private void OnDisable() {
        // Input Actionを無効化
        actionPause.ToInputAction().Disable();
    }

    private void OnDestroy() {
        // Actionイベント消去
        actionPause.action.performed -= OnPause;

        // Input Actionを無効化
        actionPause.ToInputAction().Disable();
    }

    private void Update() {
        // 監視対象の変数を更新
        fadeMode.Value = FadeManager.fadeMode;

        // フェード中ポーズのための判定
        if (FadeManager.fadeMode != FadeManager.eFade.Default) {
            if (!isPaused) {
                isPaused = true;
                NoMenu = true;
                Pause();
            }
        }
    }

    private void OnPause(InputAction.CallbackContext context) {
        if (FadeManager.fadeMode != FadeManager.eFade.Default) {
            // フェード中はポーズの開始の入力を受け付けない
            return;
        }

        _isPaused = !_isPaused;

        if (_isPaused) {
            Pause();
        } else {
            Resume();
        }
    }

    public static void Pause() {
        pauseSubject.OnNext("pause");
    }

    public static void Resume() {
        resumeSubject.OnNext("resume");
    }
}