//=============================================================================
// @File	: [OptionPanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/20	スクリプト作成
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class OptionPanel : MonoBehaviour
{
    //オーディオソース
    private AudioSource audioSource;

    //ボタンの色
    [SerializeField, Header("ボタンの色")] private Color buttonColor;

    //ボタン
    [SerializeField, Header("BGM")] private Button bgmButton;
    [SerializeField, Header("SE")] private Button seButton;
    [SerializeField, Header("キーボード&マウスを設定")] private Button keybordOptionButton;
    [SerializeField, Header("コントローラを設定")] private Button controllerOptionButton;
    [SerializeField, Header("戻る")] private Button backButton;

    //ボリュームアイコン
    [SerializeField, Header("BGMボリュームアイコン")] private VolumeIcon bgmVolumeIcon;
    [SerializeField, Header("SEボリュームアイコン")] private VolumeIcon seVolumeIcon;

    public enum EOptionButton { BGM,SE,OPTION_KEYBORD,OPTION_CONTROLLER,BACK,MAX_OPTION_BUTTON};

    //マウス位置
    private Vector3 mousePos = Vector3.zero;

    private bool bMouse = true;

	void Awake()
	{
        audioSource = GetComponent<AudioSource>();
	}

    private void Update()
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
            if (gamepad.leftStick.ReadValue() != Vector2.zero || gamepad.aButton.wasReleasedThisFrame) ChangeInput();
        }
        else
        {
            if (mousePos != oldMousePos) ChangeInput();
        }

        if (bMouse) return;

        //bgmまたはseが選択されている場合、左右でボリュームを変更する
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        if (!selectedObject) return;
        if (bgmButton.gameObject == selectedObject)
        {
            if (gamepad.dpad.right.wasReleasedThisFrame || gamepad.leftStick.ReadValue().x >= 0.9f) BGMVolAddButton();
            if (gamepad.dpad.left.wasReleasedThisFrame || gamepad.leftStick.ReadValue().x <= -0.9f) BGMVolDelButton();
        }
        else if (seButton.gameObject == selectedObject)
        {
            if (gamepad.dpad.right.wasReleasedThisFrame || gamepad.leftStick.ReadValue().x >= 0.9f) SEVolAddButton();
            if (gamepad.dpad.left.wasReleasedThisFrame || gamepad.leftStick.ReadValue().x <= -0.9f) SEVolDelButton();
        }
    }
    /// <summary>
    /// 戻るボタン
    /// </summary>
    public void BackButton()
    {
        SoundDecisionSE();
        //オプション画面を閉じる
    }
    /// <summary>
    /// キーボード&マウス設定ボタン
    /// </summary>
    public void OptionKeybordButton()
    {
        SoundDecisionSE();
        //キーボード＆マウス設定を開く
    }
    /// <summary>
    /// コントローラ設定ボタン
    /// </summary>
    public void OptionControllerButton()
    {
        SoundDecisionSE();
        //コントローラ設定を開く
    }
    /// <summary>
    /// BGM音量上げるボタン
    /// </summary>
    public void BGMVolAddButton()
    {
        SoundDecisionSE();
        bgmVolumeIcon.AddVolume();
    }
    /// <summary>
    /// BGM音量下げるボタン
    /// </summary>
    public void BGMVolDelButton()
    {
        SoundDecisionSE();
        bgmVolumeIcon.DelVolume();
    }
    /// <summary>
    /// SE音量上げるボタン
    /// </summary>
    public void SEVolAddButton()
    {
        SoundDecisionSE();
        seVolumeIcon.AddVolume();
    }
    /// <summary>
    /// SE音量下げるボタン
    /// </summary>
    public void SEVolDelButton()
    {
        SoundDecisionSE();
        seVolumeIcon.DelVolume();
    }

    

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
    /// <summary>
    /// 入力切替
    /// </summary>
    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //マウスカーソル非表示
            Cursor.visible = false;

            ColorBlock colors = keybordOptionButton.colors;
            colors.highlightedColor = Color.white;
            keybordOptionButton.colors = colors;
            controllerOptionButton.colors = colors;
            backButton.colors = colors;

            //デフォルトのボタンを選択
            ControllerChangeSelect(EOptionButton.BGM);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;

            ColorBlock colors = keybordOptionButton.colors;
            colors.highlightedColor = buttonColor;
            keybordOptionButton.colors = colors;
            controllerOptionButton.colors = colors;
            backButton.colors = colors;

            ControllerNoneSelect();
        }

        bMouse = !bMouse;
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
}
