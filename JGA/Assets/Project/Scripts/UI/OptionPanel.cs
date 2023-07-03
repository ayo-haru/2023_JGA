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
// 2023/05/29	(小楠)ゲームパッド繋がってないときにキーボードで操作できないのを修正
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class OptionPanel : MonoBehaviour
{
    private AudioSource audioSource;

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

    private bool bOpen = false;
    
    private MenuSettingItem item;

	void Awake()
	{
        //MenuInputManagerが置かれていなかった場合は生成
        if (MenuInputManager.Instance == null)
        {
            MenuInputManager.Create();
        }

        //コンポーネント取得
        audioSource = GetComponent<AudioSource>();

        keybordOptionButtonImage = keybordOptionButton.GetComponent<Image>();
        controllerOptionButtonImage = controllerOptionButton.GetComponent<Image>();
        backButtonImage = backButton.GetComponent<Image>();
        addBGMButtonImage = addBGMButton.GetComponent<Image>();
        delBGMButtonImage = delBGMButton.GetComponent<Image>();
        addSEButtonImage = addSEButton.GetComponent<Image>();
        delSEButtonImage = delSEButton.GetComponent<Image>();

        //メニュー設定
        List<Image> images = new List<Image>();
        if(keybordOptionButtonImage) images.Add(keybordOptionButtonImage);
        if (controllerOptionButtonImage) images.Add(controllerOptionButtonImage);
        if (backButtonImage) images.Add(backButtonImage);
        if (addBGMButtonImage) images.Add(addBGMButtonImage);
        if (delBGMButtonImage) images.Add(delBGMButtonImage);
        if (addSEButtonImage) images.Add(addSEButtonImage);
        if (delSEButtonImage) images.Add(delSEButtonImage);
        item = new MenuSettingItem(bgmButton, images);
        item.onMove += OnMove;
    }

#if false
    private void Start()
    {
        
    }
    private void Update()
    {
    }

    private void FixedUpdate()
    {

    }
#endif
#region オプション画面ボタン
    /// <summary>
    /// 戻るボタン
    /// </summary>
    public void BackButton()
    {
        bOpen = false;
        MenuInputManager.PopMenu();
        SoundDecisionSE();
    }
    /// <summary>
    /// キーボード&マウス設定ボタン
    /// </summary>
    public void OptionKeybordButton()
    {
        bOpen = false;
        SoundDecisionSE();
    }
    /// <summary>
    /// コントローラ設定ボタン
    /// </summary>
    public void OptionControllerButton()
    {
        bOpen = false;
        SoundDecisionSE();
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

    public void Open()
    {
        if (IsOpen()) return;
        bOpen = true;
    }
    public void NewOpen()
    {
        if (IsOpen()) return;
        bOpen = true;
        MenuInputManager.PushMenu(item);
    }
    public bool IsOpen()
    {
        return bOpen;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
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
