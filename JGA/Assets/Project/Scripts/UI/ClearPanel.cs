//=============================================================================
// @File	: [ClearPanel.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/6	スクリプト作成
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClearPanel : MonoBehaviour
{
    private AudioSource audioSource;

    //ボタン
    [SerializeField, Header("NEXT DAY")] private Button nextDayButton;
    private Image nextDayButtonImage;
    [SerializeField, Header("BACK TO TITLE")] private Button backToTitleButton;
    private Image backToTitleButtonImage;

    //次のシーン
    private int nextScene = -1;

    private MenuSettingItem item;


    void Awake()
	{
        nextScene = -1;

        //MenuInputManagerが置かれていなかった場合は生成
        if (MenuInputManager.Instance == null)
        {
            MenuInputManager.Create();
        }

        audioSource = GetComponent<AudioSource>();
        nextDayButtonImage = nextDayButton.GetComponent<Image>();
        backToTitleButtonImage = backToTitleButton.GetComponent<Image>();

        //メニュー設定
        List<Image> images = new List<Image>();
        if(nextDayButtonImage)images.Add(nextDayButtonImage);
        if(backToTitleButtonImage)images.Add(backToTitleButtonImage);
        item = new MenuSettingItem(nextDayButton, images);
    }

    private void OnEnable()
    {
        nextScene = -1;
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
#region クリア画面のボタン
    /// <summary>
    /// NEXT DAYボタン
    /// </summary>
    public void NextDayButton()
    {
        SoundDecisionSE();
        if (System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length > GameData.nowScene)
        {  // 最大シーンではないとき
            nextScene = GameData.nowScene + 1;
        }else{
            nextScene = GameData.nowScene;
        }
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
