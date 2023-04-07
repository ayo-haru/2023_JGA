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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class TitleScenManager : BaseSceneManager
{
    [SerializeField,Header("ゲームを始める")] private Button startButton;
    [SerializeField,Header("オプション")] private Button optionButton;
    [SerializeField,Header("ゲームをやめる")] private Button exitButton;
    [SerializeField, Header("BGM")] private Slider bgmSlider;
    [SerializeField, Header("SE")] private Slider seSlider;
    [SerializeField, Header("オプション画面のBackボタン")] private Button optionBackButton;
    [SerializeField, Header("オプション画面のKeyconfigボタン")] private Button optionKeyconfigButton;
    [SerializeField, Header("キーコンフィグ画面のBackボタン")] private Button keyconfigBackButton;

    //入力フラグ
    private bool bMouse = true;
    //マウス位置
    private Vector3 mousePos;
    //フェードパネル
    [SerializeField] private Image fadePanelImage;

    [SerializeField] private RectTransform OptionObject;

    //タイトル画面のボタン
    private enum ETitleSelect {TITLESELECT_START,TITLESELECT_OPTION,TITLESELECT_EXIT,OPTIONSELECT_BGM,OPTIONSELECT_SE,OPTIONSELECT_KEYCONFIG,OPTIONSELECT_BACK,KEYCONFIGSELECT_BACK,MAX_TITLESELECT};

    //タイトルのメニュー
    private enum ETitleMenu { TITLESCREEN_TITLE, TITLESCREEN_OPTION, TITLESCREEN_KEYCONFIG, MAX_TITLESCREEN };
    private ETitleMenu titleMenu = ETitleMenu.TITLESCREEN_TITLE;

    private AudioSource audioSource;

    //オプションスライド移動用変数
    private int nSlide = 0;

    private void Awake() {
        /*
         * InitでAddComponentしてるもの
         * ・AudioSource
         * ・UIManager
         */
        Init();

        // BGM再生用にオーディオソース取得
        audioSource = GetComponent<AudioSource>();

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
        //メニュー移動中は入力受け付けない
        if (nSlide != 0) return;

        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        //マウス有効の状態でコントローラが押されたらコントローラ入力にする
        //マウス無効でマウスが動いたらマウス入力を有効
        if (bMouse)
        {
            if (gamepad.leftStick.ReadValue() != Vector2.zero || gamepad.aButton.wasReleasedThisFrame) ChangeInput();
        }
        else
        {
            if (mousePos != oldMousePos) ChangeInput();
        }
#if false
        //コントローラ入力
        if (!bMouse)
        {
            //上
            //if(gamepad.dpad.up.wasReleasedThisFrame)ControllerUpSelect();
            //下
            //if(gamepad.dpad.down.wasReleasedThisFrame)ControllerDownSelect();
        }
#endif
    }

    private void FixedUpdate()
    {
        if(nSlide != 0)
        {
            OptionSlide();
        }
    }

#region タイトル画面のボタン
    public void StartButton()
    {
        SceneChange(MySceneManager.SceneState.SCENE_GAME);
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
        //コントローラ入力の場合マウスカーソルが非表示のままになってしまうので表示する
        if (!bMouse)Cursor.visible = true;
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
        nSlide = -1920;
        ++titleMenu;
        if(!bMouse)ChangeMenu(titleMenu);

    }
    public void OptionMoveRight()
    {
        //オプションを右に動かす
        nSlide = 1920;
        --titleMenu;
        if(!bMouse)ChangeMenu(titleMenu);
    }
    public void OptionSlide()
    {
        int slideSpeed = 1920 / 30; //←必ず1920が割り切れる数を設定してください
        if(nSlide > 0)
        {
            nSlide -= slideSpeed;
            OptionObject.position = new Vector3(OptionObject.position.x + slideSpeed, OptionObject.position.y, OptionObject.position.z);
        }
        else
        {
            nSlide += slideSpeed;
            OptionObject.position = new Vector3(OptionObject.position.x - slideSpeed, OptionObject.position.y, OptionObject.position.z);
        }
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
            ChangeMenu(titleMenu);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;
            fadePanelImage.raycastTarget = false;
            ControllerNoneSelect();
        }

        bMouse = !bMouse;
    }

    private void ControllerChangeSelect(ETitleSelect _select)
    {
        ControllerNoneSelect();
        switch (_select)
        {
            case ETitleSelect.TITLESELECT_START:
                startButton.Select();
                break;
            case ETitleSelect.TITLESELECT_OPTION:
                optionButton.Select();
                break;
            case ETitleSelect.TITLESELECT_EXIT:
                exitButton.Select();
                break;
            case ETitleSelect.OPTIONSELECT_BGM:
                bgmSlider.Select();
                break;
            case ETitleSelect.OPTIONSELECT_SE:
                seSlider.Select();
                break;
            case ETitleSelect.OPTIONSELECT_KEYCONFIG:
                optionKeyconfigButton.Select();
                break;
            case ETitleSelect.OPTIONSELECT_BACK:
                optionBackButton.Select();
                break;
            case ETitleSelect.KEYCONFIGSELECT_BACK:
                keyconfigBackButton.Select();
                break;
        }
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
#if false
    private void ControllerUpSelect()
    {
        //現在select状態のオブジェクトを取得
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (!selectedObject) return;
        //ボタンコンポーネント取得
        Button selectedButton = selectedObject.GetComponent<Button>();
        if (!selectedButton) return;
        //移動先のボタンを取得
        Selectable nextButton = selectedButton.navigation.selectOnUp;
        if (!nextButton) return;
        //現在の選択をリセットし次のボタンを選択状態にする
        ControllerNoneSelect();
        nextButton.Select();

        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    private void ControllerDownSelect()
    {
        //現在select状態のオブジェクトを取得
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (!selectedObject) return;
        //ボタンコンポーネント取得
        Button selectedButton = selectedObject.GetComponent<Button>();
        if (!selectedButton) return;
        //移動先のボタンを取得
        Selectable nextButton = selectedButton.navigation.selectOnDown;
        if (!nextButton) return;
        //現在の選択をリセットし次のボタンを選択状態にする
        ControllerNoneSelect();
        nextButton.Select();

        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
#endif
    private void ControllerNoneSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectButton()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }

    private void ChangeMenu(ETitleMenu _menu)
    {
        switch (_menu)
        {
            case ETitleMenu.TITLESCREEN_TITLE:
                ControllerChangeSelect(ETitleSelect.TITLESELECT_START);
                break;
            case ETitleMenu.TITLESCREEN_OPTION:
                ControllerChangeSelect(ETitleSelect.OPTIONSELECT_BGM);
                break;
            case ETitleMenu.TITLESCREEN_KEYCONFIG:
                ControllerChangeSelect(ETitleSelect.KEYCONFIGSELECT_BACK);
                break;
        }
    }
}
