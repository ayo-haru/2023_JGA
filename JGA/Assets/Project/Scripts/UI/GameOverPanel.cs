//=============================================================================
// @File	: [GameOverPanel.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/6	スクリプト作成
// 2023/05/29	(小楠)ゲームパッド繋がってないときにキーボードで操作できないのを修正
//=============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GameOverPanel : MonoBehaviour
{
    private AudioSource audioSource;

    //ボタン
    [SerializeField, Header("RETRY")] private Button retryButton;
    private Image retryButtonImage;
    [SerializeField, Header("BACK TO TITLE")] private Button backToTitleButton;
    private Image backToTitleButtonImage;
    //次のシーン
    private int nextScene = -1;

    private MenuSettingItem item;

    //[SerializeField] private InputActionReference actionMove;

    void Awake()
	{
        nextScene = -1;

        //MenuInputManagerが置かれていなかった場合は生成
        if (MenuInputManager.Instance == null)
        {
            MenuInputManager.Create();
        }

        audioSource = GetComponent<AudioSource>();
        retryButtonImage = retryButton.GetComponent<Image>();
        backToTitleButtonImage = backToTitleButton.GetComponent<Image>();

        //メニュー設定
        List<Image> images = new List<Image>();
        if(retryButtonImage)images.Add(retryButtonImage);
        if(backToTitleButtonImage)images.Add(backToTitleButtonImage);
        item = new MenuSettingItem(retryButton, images);
    }

    private void OnEnable()
    {
        nextScene = -1;

        List<Image> images = new List<Image>();
        images.Add(retryButtonImage);
        images.Add(backToTitleButtonImage);
        MenuInputManager.PushMenu(item);
    }

    private void OnDisable()
    {
        MenuInputManager.PopMenu();
    }
#if false
    private void Update()
    {

    }
#endif
#region ゲームオーバー画面のボタン
    /// <summary>
    /// NEXT DAYボタン
    /// </summary>
    public void RetryButton()
    {
        SoundDecisionSE();
        nextScene = GameData.nowScene;
    }
    /// <summary>
    /// BACK TO TITLEボタン
    /// </summary>
    public void BackToTitleButton()
    {
        SoundDecisionSE();
        nextScene = (int)MySceneManager.SceneState.SCENE_TITLE;
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
#endregion

    public int GetNextScene()
    {
        return nextScene;
    }
}
