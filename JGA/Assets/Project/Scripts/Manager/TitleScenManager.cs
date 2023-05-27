//=============================================================================
// @File	: [TitleScenManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/13	スクリプト作成
// 2023/03/21	(小楠)ボタン操作追加
// 2023/03/28	(小楠)音追加
// 2023/04/05	(小楠)オプション画面追加
// 2023/04/06	(小楠)オプションのスライド移動追加
// 2023/04/08	(小楠)オプションの処理全部消した
// 2023/05/26	(小楠)はじめからと続きからを追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScenManager : BaseSceneManager {
    [SerializeField, Header("はじめから")] private Selectable startButton;
    private BoxCollider startButtonCollider;
    [SerializeField, Header("つづきから")] private Selectable continueButton;
    private BoxCollider continueButtonCollider;
    [SerializeField, Header("オプション")] private Selectable optionButton;
    private BoxCollider optionButtonCollider;
    [SerializeField, Header("ゲームをやめる")] private Selectable exitButton;
    private BoxCollider exitButtonCollider;

    [SerializeField, Header("オプション")] private OptionPanel optionPanel;

    //入力フラグ
    private bool bMouse = true;
    //マウス位置
    private Vector3 mousePos;

    private bool bGamePad;
    [SerializeField] private InputActionReference actionMove;

    //タイトル画面のボタン
    private enum ETitleSelect { TITLESELECT_START, TITLESELECT_CONTINUE,TITLESELECT_OPTION, TITLESELECT_EXIT, MAX_TITLESELECT };

    //次のシーン
    private int nextScene;

    private AudioSource audioSource;
    private bool bFlag = false;

    private void Awake() {
        /*
         * InitでAddComponentしてるもの
         * ・AudioSource
         * ・UIManager
         */
        Init();

        //セーブデータ読み込み
        MySceneManager.GameData.isContinueGame = SaveSystem.load();

        //もしセーブデータがなかったら次に遷移するシーンはゲーム001
        //if (MySceneManager.GameData.isContinueGame) {
        //    SaveManager.LoadAll();  // セーブデータロード
        //    nextScene = MySceneManager.GameData.nowScene;   // 次のシーンを更新
        //} else {
        //    MySceneManager.GameData.nowScene = (int)MySceneManager.SceneState.SCENE_TITLE;
        //    nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;
        //}

        // BGM再生用にオーディオソース取得
        audioSource = GetComponent<AudioSource>();

        actionMove.action.performed += OnMove;
        actionMove.action.canceled += OnMove;
        actionMove.ToInputAction().Enable();

        startButtonCollider = startButton.GetComponent<BoxCollider>();
        continueButtonCollider = continueButton.GetComponent<BoxCollider>();
        optionButtonCollider = optionButton.GetComponent<BoxCollider>();
        exitButtonCollider = exitButton.GetComponent<BoxCollider>();
    }

    private void Start() {
        //BGM再生
       SoundManager.Play(audioSource, SoundManager.EBGM.TITLE_001);
        //フェード中だったら待機して音を止める
        StartCoroutine(WaitFade());

        InitInput();
    }

    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        bool beforeFlag = bFlag;
        bFlag = optionPanel.IsOpen();
        //オプション画面が開かれているときは処理しない
        if (bFlag) return;
        //オプション画面から戻って来た時の初期化処理
        if (beforeFlag) InitInput();

        //マウスの状態を更新
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;
        //ゲームパットの状態を更新
        bGamePad = Gamepad.current != null;

        if (bMouse) return;

        //ゲームパッドがない又はマウスが動かされたらマウス入力に切り替え
        if (!bGamePad || Vector3.Distance(oldMousePos, mousePos) >= 1.5f)
        {
            ChangeInput();
        }
    }

    #region タイトル画面のボタン
    public void StartButton() {
        SoundSEDecision();
        MySceneManager.GameData.oldScene = (int)MySceneManager.SceneState.SCENE_TITLE;  // 今のシーンをひとつ前のシーンとして保存
        MySceneManager.GameData.nowScene = nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;   // 次のシーンを更新
        //初めから用に初期化
        MySceneManager.GameData.isContinueGame = false; // startでセーブデータがあったとしてもfalseにする
        SaveManager.SaveInitDataAll();  // データをリセットして保存
        
        SceneChange(nextScene);
        //コントローラ入力の場合マウスカーソルが非表示のままになってしまうので表示する
        if (!bMouse) Cursor.visible = true;
    }

    public void ContinueButton()
    {
        SoundSEDecision();
        if (MySceneManager.GameData.isContinueGame) {   // セーブデータが存在してたか
            // セーブデータ有りなのでデータロード
            SaveManager.LoadAll();  // セーブデータロード
            nextScene = MySceneManager.GameData.nowScene;   // 次のシーンを更新。セーブデータのロードをしてnowSceneには前回のシーン番号が入っているからnextにnowを代入
        } else {
            // セーブデータがなかったので「はじめから」と同じように振る舞う
            MySceneManager.GameData.nowScene = nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;
        }
        MySceneManager.GameData.oldScene = (int)MySceneManager.SceneState.SCENE_TITLE;  // 今のシーンをひとつ前のシーンとして保存
        SceneChange(nextScene);
        //コントローラ入力の場合マウスカーソルが非表示のままになってしまうので表示する
        if (!bMouse) Cursor.visible = true;
    }

    public void OptionButton() {
        SoundSEDecision();
        ControllerNoneSelect();
        //オプション画面を開く
        optionPanel.Open();
    }
    public void ExitButton() {
        SoundSEDecision();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    public void InitInput()
    {
        mousePos = Input.mousePosition;
        bMouse = bGamePad = (Gamepad.current) != null;
        ChangeInput();
    }

    private void ChangeInput() {
        //マウス→コントローラ
        if (bMouse) {
            ControllerChangeSelect(ETitleSelect.TITLESELECT_START);
        } else//コントローラ→マウス
          {
            ControllerNoneSelect();
        }

        bMouse = !bMouse;
        Cursor.visible = bMouse;
        startButtonCollider.enabled = bMouse;
        continueButtonCollider.enabled = bMouse;
        optionButtonCollider.enabled = bMouse;
        exitButtonCollider.enabled = bMouse;
    }

    private void ControllerChangeSelect(ETitleSelect _select) {
        ControllerNoneSelect();
        switch (_select) {
            case ETitleSelect.TITLESELECT_START:
                startButton.Select();
                break;
            case ETitleSelect.TITLESELECT_CONTINUE:
                continueButton.Select();
                break;
            case ETitleSelect.TITLESELECT_OPTION:
                optionButton.Select();
                break;
            case ETitleSelect.TITLESELECT_EXIT:
                exitButton.Select();
                break;
        }
        SoundSESelect();
    }

    private void ControllerNoneSelect() {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SoundSESelect() {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    public void SoundSEDecision() {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }

    IEnumerator WaitFade() {
        audioSource.Pause();
        yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.Default);
        audioSource.UnPause();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!bGamePad) bGamePad = true;
        if (optionPanel.IsOpen()) return;
        if (!bMouse) return;

        //マウス→コントローラ
        ChangeInput();
    }

}
