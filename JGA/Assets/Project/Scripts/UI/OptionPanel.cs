//=============================================================================
// @File	: [OptionPanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/20	スクリプト作成
// 2023/04/08	(小楠)オプションのスライド移動追加
// 2023/05/28	(小楠)スクリプト綺麗にした
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class OptionPanel : MonoBehaviour
{
    private AudioSource audioSource;
    private RectTransform rt;

    //ボタンの色
    [SerializeField, Header("ボタンの色")] private Color buttonColor;

    //ボタン
    [SerializeField, Header("BGM")] private Button bgmButton;
    [SerializeField, Header("SE")] private Button seButton;
    [SerializeField, Header("キーボード&マウスを設定")] private Button keybordOptionButton;
    private Image keybordOptionButtonImage;
    [SerializeField, Header("コントローラを設定")] private Button controllerOptionButton;
    private Image controllerOptionButtonImage;
    [SerializeField, Header("戻る")] private Button backButton;
    private Image backButtonImage;
    [SerializeField, Header("BGM増やすボタン")] private Button addBGMButton;
    private Image addBGMButtonImage;
    [SerializeField, Header("BGM減らすボタン")] private Button delBGMButton;
    private Image delBGMButtonImage;
    [SerializeField, Header("SE増やすボタン")] private Button addSEButton;
    private Image addSEButtonImage;
    [SerializeField, Header("SE減らすボタン")] private Button delSEButton;
    private Image delSEButtonImage;

    //ボリュームアイコン
    [SerializeField, Header("BGMボリュームアイコン")] private VolumeIcon bgmVolumeIcon;
    [SerializeField, Header("SEボリュームアイコン")] private VolumeIcon seVolumeIcon;

    public enum EOptionButton { BGM,SE,OPTION_KEYBORD,OPTION_CONTROLLER,BACK,MAX_OPTION_BUTTON};

    //マウス
    private Vector3 mousePos = Vector3.zero;
    private bool bMouse = true;

    private bool bGamePad;
    [SerializeField] private InputActionReference actionMove;

    public enum EOptionSlide { MIN_SLIDE = -2,LEFT, CENTER, RIGHT,MAX_SLIDE };
    private int nSlide = (int)EOptionSlide.RIGHT;
    [SerializeField, Range(1.0f, 100.0f)] private float slideSpeed = 10.0f;
    private float screenWidth = 1920.0f;
    [SerializeField] private bool bSlide = false;
    


	void Awake()
	{
        audioSource = GetComponent<AudioSource>();
        rt = GetComponent<RectTransform>();

        //初期位置設定
        Vector3 pos = rt.localPosition;
        pos.x = screenWidth;
        rt.localPosition = pos;

        //イベント登録
        actionMove.action.performed += OnMove;
        actionMove.action.canceled += OnMove;
        actionMove.ToInputAction().Enable();

        //ボタンの加増取得
        keybordOptionButtonImage = keybordOptionButton.GetComponent<Image>();
        controllerOptionButtonImage = controllerOptionButton.GetComponent<Image>();
        backButtonImage = backButton.GetComponent<Image>();
        addBGMButtonImage = addBGMButton.GetComponent<Image>();
        delBGMButtonImage = delBGMButton.GetComponent<Image>();
        addSEButtonImage = addSEButton.GetComponent<Image>();
        delSEButtonImage = delSEButton.GetComponent<Image>();
    }

    private void Update()
    {
        if (nSlide != (int)EOptionSlide.CENTER) return;
        if (bSlide && rt.localPosition != Vector3.zero) return;
        if (!IsOpen()) return;

        //マウスの状態を更新
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;
        //ゲームパットの状態を更新
        bGamePad = Gamepad.current != null;

        if (bMouse) return;

        //ゲームパッドがない又はマウスが動かされたらマウス入力に切り替え
        if (!bGamePad || Vector3.Distance(oldMousePos, mousePos) >= 1.0f)
        {
            ChangeInput();
        }
    }

    private void FixedUpdate()
    {
        if (!bSlide) return;
        if (nSlide >= (int)EOptionSlide.MAX_SLIDE || nSlide <= (int)EOptionSlide.MIN_SLIDE) return;
        if (rt.localPosition.x == screenWidth * nSlide) return;
        rt.localPosition = Vector3.MoveTowards(rt.localPosition, new Vector3(screenWidth * nSlide, rt.localPosition.y, rt.localPosition.z), slideSpeed);
    }
    #region オプション画面ボタン
    /// <summary>
    /// 戻るボタン
    /// </summary>
    public void BackButton()
    {
        SoundDecisionSE();
        //オプション画面を閉じる
        nSlide = (int)EOptionSlide.RIGHT;
        ControllerNoneSelect();
    }
    /// <summary>
    /// キーボード&マウス設定ボタン
    /// </summary>
    public void OptionKeybordButton()
    {
        SoundDecisionSE();
        //キーボード＆マウス設定を開く
        nSlide = (int)EOptionSlide.LEFT;
        ControllerNoneSelect();
    }
    /// <summary>
    /// コントローラ設定ボタン
    /// </summary>
    public void OptionControllerButton()
    {
        SoundDecisionSE();
        //コントローラ設定を開く
        nSlide = (int)EOptionSlide.LEFT;
        ControllerNoneSelect();
    }
    /// <summary>
    /// BGM音量上げるボタン
    /// </summary>
    public void BGMVolAddButton()
    {
        if (bgmVolumeIcon.AddVolume()){
            SoundSelectSE();
        }else{
            SoundDecisionSE();
        }
    }
    /// <summary>
    /// BGM音量下げるボタン
    /// </summary>
    public void BGMVolDelButton()
    {
        if (bgmVolumeIcon.DelVolume()){
            SoundSelectSE();
        }else{
            SoundDecisionSE();
        }
    }
    /// <summary>
    /// SE音量上げるボタン
    /// </summary>
    public void SEVolAddButton()
    {
        if (seVolumeIcon.AddVolume()){
            SoundSelectSE();
        }else{
            SoundDecisionSE();
        }
    }
    /// <summary>
    /// SE音量下げるボタン
    /// </summary>
    public void SEVolDelButton()
    {
        if (seVolumeIcon.DelVolume()){
            SoundSelectSE();
        }else{
            SoundDecisionSE();
        }
    }
    #endregion
    #region SE鳴らす関数
    public void SoundSelectSE()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }

    public void SoundDecisionSE()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }
    public void SoundSlideSE()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    #endregion
    /// <summary>
    /// 入力初期化
    /// </summary>
    public void InitInput()
    {
        mousePos = Input.mousePosition;
        bMouse = bGamePad = (Gamepad.current) != null;
        ChangeInput();
    }
    /// <summary>
    /// 入力切替
    /// </summary>
    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //デフォルトのボタンを選択
            ControllerChangeSelect(EOptionButton.BGM);
        }
        else//コントローラ→マウス
        {
            ControllerNoneSelect();
        }

        //入力を切り替え
        bMouse = !bMouse;
        Cursor.visible = bMouse;
        keybordOptionButtonImage.raycastTarget = bMouse;
        controllerOptionButtonImage.raycastTarget = bMouse;
        backButtonImage.raycastTarget = bMouse;
        addBGMButtonImage.raycastTarget = bMouse;
        delBGMButtonImage.raycastTarget = bMouse;
        addSEButtonImage.raycastTarget = bMouse;
        delSEButtonImage.raycastTarget = bMouse;
    }

    public void ControllerChangeSelect(EOptionButton _select)
    {
        ControllerNoneSelect();
        switch (_select)
        {
            case EOptionButton.BGM:
                bgmButton.Select();
                break;
            case EOptionButton.SE:
                seButton.Select();
                break;
            case EOptionButton.OPTION_KEYBORD:
                keybordOptionButton.Select();
                break;
            case EOptionButton.OPTION_CONTROLLER:
                controllerOptionButton.Select();
                break;
            case EOptionButton.BACK:
                backButton.Select();
                break;
        }
        SoundSelectSE();
    }

    public void ControllerNoneSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Open()
    {
        if (nSlide == (int)EOptionSlide.CENTER) return;
        nSlide = (int)EOptionSlide.CENTER;
        InitInput();
    }
    public bool IsOpen()
    {
        return nSlide == (int)EOptionSlide.CENTER;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOpen()) return;
        if (!bGamePad) bGamePad = true;
        if (bMouse) ChangeInput();

        //左右の入力があるか？
        Vector2 move = context.ReadValue<Vector2>();
        if (move.x < 1.0f && move.x > -1.0f) return;

        //BGMボタンが選択されているか？
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if(selectedObject == bgmButton.gameObject)
        {
            if (move.x >= 1.0f) BGMVolAddButton();
            if (move.x <= -1.0f) BGMVolDelButton();
        }
        //SEボタンが選択されているか
        if(selectedObject == seButton.gameObject)
        {
            if (move.x >= 1.0f) SEVolAddButton();
            if (move.x <= -1.0f) SEVolDelButton();
        }
    }
}
