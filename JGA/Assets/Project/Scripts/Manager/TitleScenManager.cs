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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleScenManager : BaseSceneManager
{
    //ゲームを始めるボタン
    [SerializeField] private Button startButton;
    private Image startImage;
    //オプションボタン
    [SerializeField] private Button optionButton;
    private Image optionImage;
    //ゲームをやめるボタン
    [SerializeField] private Button exitButton;
    private Image exitImage;
    //入力フラグ
    private bool bMouse = true;
    //マウス位置
    private Vector3 mousePos;
    //フェードパネル
    [SerializeField] private Image fadePanelImage;

    [SerializeField,Tooltip("オプション画面のBackボタン")] private Button optionBackButton;
    [SerializeField, Tooltip("オプション画面のKeyconfigボタン")] private Button optionKeyconfigButton;
    [SerializeField,Tooltip("キーコンフィグ画面のBackボタン")] private Button keyconfigBackButton;
    [SerializeField] private RectTransform OptionObject;

    //タイトル画面のボタン
    private enum ETitleSelect {TITLESELECT_START,TITLESELECT_OPTION,TITLESELECT_EXIT,MAX_TITLESELECT};
    private ETitleSelect select = ETitleSelect.TITLESELECT_START;

    //タイトルのメニュー
    private enum ETitleMenu { TITLESCREEN_TITLE, TITLESCREEN_OPTION, TITLESCREEN_KEYCONFIG, MAX_TITLESCREEN };
    private ETitleMenu titleMenu = ETitleMenu.TITLESCREEN_TITLE;

    private AudioSource audioSource;

    private void Awake() {
        /*
         * InitでAddComponentしてるもの
         * ・AudioSource
         * ・UIManager
         */
        Init();

        // BGM再生用にオーディオソース取得
        audioSource = GetComponent<AudioSource>();

        startImage = startButton.GetComponent<Image>();
        optionImage = optionButton.GetComponent<Image>();
        exitImage = exitButton.GetComponent<Image>();

        //オプション画面、キーコンフィグ画面のボタンのイベントを追加
        optionBackButton.onClick.AddListener(OptionBackButton);
        optionKeyconfigButton.onClick.AddListener(OptionKeyconfigButton);
        keyconfigBackButton.onClick.AddListener(KeyconfigBackButton);
    }

    private void Start() {
        // BGM再生
        SoundManager.Play(audioSource, SoundManager.EBGM.TITLE_001);
    }

    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        //マウス有効の状態でコントローラが押されたらコントローラ入力にする
        //マウス無効でマウスが動いたらマウス入力を有効
        if (bMouse)
        {
            if (gamepad.dpad.up.wasReleasedThisFrame || gamepad.dpad.down.wasReleasedThisFrame || gamepad.aButton.isPressed) ChangeInput();
        }
        else
        {
            if (mousePos != oldMousePos) ChangeInput();
        }

        //コントローラ入力
        if (!bMouse)
        {
            ETitleSelect old = select;

            if(gamepad.dpad.up.wasReleasedThisFrame)
            {
                if(select > 0)--select;
            }
            //左スティック　↓
            if(gamepad.dpad.down.wasReleasedThisFrame)
            {
                if (select < ETitleSelect.MAX_TITLESELECT - 1) ++select;
            }
            //Aボタン
            if (gamepad.aButton.isPressed)
            {
                switch (select)
                {
                    case ETitleSelect.TITLESELECT_START:StartButton();break;
                    case ETitleSelect.TITLESELECT_OPTION:OptionButton();break;
                    case ETitleSelect.TITLESELECT_EXIT:ExitButton();break;
                }
            }

            if(select != old)ControllerChangeSelect(select);
        }
    }

    #region タイトル画面のボタン
    public void StartButton()
    {
        SceneChange(MySceneManager.SceneState.SCENE_GAME);
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }
    public void OptionButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
        //オプション画面を開く
        OptionMoveLeft();
    }
    public void ExitButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region オプション画面のボタン
    public void OptionBackButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
        //オプション閉じる
        OptionMoveRight();
    }

    public void OptionKeyconfigButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
        //キーコンフィグ画面を開く
        OptionMoveLeft();
    }
    #endregion

    #region キーコンフィグ画面のボタン
    public void KeyconfigBackButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
        //キーコンフィグ画面を閉じる
        OptionMoveRight();
    }
    #endregion

    #region オプション移動関数
    public void OptionMoveLeft()
    {
        //オプションを左に動かす
        OptionObject.position = new Vector3(OptionObject.position.x - 1920.0f, OptionObject.position.y, OptionObject.position.z);
    }
    public void OptionMoveRight()
    {
        //オプションを右に動かす
        OptionObject.position = new Vector3(OptionObject.position.x + 1920.0f, OptionObject.position.y, OptionObject.position.z);
    }
    #endregion

    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //マウスカーソル非表示
            Cursor.visible = false;
            fadePanelImage.raycastTarget = true;
            ControllerChangeSelect(select);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;
            fadePanelImage.raycastTarget = false;
            ControllerResetSelect();
        }

        bMouse = !bMouse;
    }

    private void ControllerChangeSelect(ETitleSelect _select)
    {
        ControllerResetSelect();
        switch (_select)
        {
            case ETitleSelect.TITLESELECT_START:
                startImage.color = Color.gray;
                break;
            case ETitleSelect.TITLESELECT_OPTION:
                optionImage.color = Color.gray;
                break;
            case ETitleSelect.TITLESELECT_EXIT:
                exitImage.color = Color.gray;
                break;
        }
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    private void ControllerResetSelect()
    {
        startImage.color = Color.white;
        optionImage.color = Color.white;
        exitImage.color = Color.white;
    }

    public void SelectButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
}
