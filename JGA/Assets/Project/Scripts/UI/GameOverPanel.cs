//=============================================================================
// @File	: [GameOverPanel.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/6	スクリプト作成
//=============================================================================
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameOverPanel : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField, Header("GAMEOVER")] private Image gameOver;
    //ボタンの色
    [SerializeField, Header("ボタンの色")] private Color buttonColor;

    //ボタン
    [SerializeField, Header("RETRY")] private Button retryButton;
    private Image retryButtonImage;
    [SerializeField, Header("BACK TO TITLE")] private Button backToTitleButton;
    private Image backToTitleButtonImage;

    public enum EGameOverPanelButton {RETRY,BACK_TO_TITLE,};

    //マウス
    private Vector3 mousePos = Vector3.zero;
    private bool bMouse = true;

    private bool bGamePad;

    //次のシーン
    private int nextScene = -1;
#if UseMyContorller
    private MyContorller gameInputs;            // 方向キー入力取得
#else
    [SerializeField] private InputActionReference actionMove;
#endif

    void Awake()
	{
        audioSource = GetComponent<AudioSource>();
        nextScene = -1;
        bMouse = true;
        mousePos = Vector3.zero;

#if UseMyController
        // Input Actionインスタンス生成
        gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Menu.Move.performed += OnMove;

		// Input Actionを有効化
		gameInputs.Enable();
#else
        actionMove.action.performed += OnMove;
        actionMove.action.canceled += OnMove;
        actionMove.ToInputAction().Enable();
#endif
        retryButtonImage = retryButton.GetComponent<Image>();
        backToTitleButtonImage = backToTitleButton.GetComponent<Image>();
        InitInput();
    }

    private void OnEnable()
    {
        nextScene = -1;
        gameOver.fillAmount = 0.0f;
        InitInput();
    }

    private void Update()
    {

        gameOver.fillAmount += Time.deltaTime;
        //マウスの状態を行進
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

    #region ゲームオーバー画面のボタン
    /// <summary>
    /// NEXT DAYボタン
    /// </summary>
    public void RetryButton()
    {
        SoundDecisionSE();
        nextScene = MySceneManager.GameData.nowScene;
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
    /// <summary>
    /// 入力切替
    /// </summary>
    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //デフォルトのボタンを選択
            ControllerChangeSelect(EGameOverPanelButton.RETRY);
        }
        else//コントローラ→マウス
        {
            ControllerNoneSelect();
        }
        //入力を切り替え
        bMouse = !bMouse;
        //カーソルの表示切替
        Cursor.visible = bMouse;
        //ボタン画像のraycastTarget切り替え
        retryButtonImage.raycastTarget = bMouse;
        backToTitleButtonImage.raycastTarget = bMouse;
    }

    public void InitInput()
    {
        mousePos = Input.mousePosition;
        bMouse = bGamePad = (Gamepad.current) != null;
        ChangeInput();
    }

    public void ControllerChangeSelect(EGameOverPanelButton _select)
    {
        ControllerNoneSelect();
        switch (_select)
        {
            case EGameOverPanelButton.RETRY:
                retryButton.Select();
                break;
            case EGameOverPanelButton.BACK_TO_TITLE:
                backToTitleButton.Select();
                break;
        }
        SoundSelectSE();
    }

    public void ControllerNoneSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public int GetNextScene()
    {
        return nextScene;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!PauseManager.isPaused) return;
        if (!bGamePad) bGamePad = true;
        if (!bMouse) return;

        //マウス→コントローラ
        ChangeInput();
    }
}
