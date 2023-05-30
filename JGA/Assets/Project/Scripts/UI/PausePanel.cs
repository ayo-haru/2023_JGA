//=============================================================================
// @File	: [PausePanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/16	スクリプト作成
// 2023/05/29	(小楠)ゲームパッド繋がってないときにキーボードで操作できないのを修正
//=============================================================================
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
	[Header("パネル移動速度")]
	[SerializeField]
	private float PanelMoveValue = 100;

	[Header("有効パネル")]
	[SerializeField]
	private GameObject pausePanel;
	[SerializeField]
	private GameObject optionPanel;
	[SerializeField]
	private GameObject keyConfigPanel;

	private GameObject ActivePanel;


	[Header("Pauseパネル - オブジェクト")]
	[SerializeField]
	private Button backButton;
    [SerializeField]
    private Image backButtonImage;
	[SerializeField]
	private Button optionButton;
    [SerializeField]
    private Image optionButtonImage;
	[SerializeField]
	private Button titleButton;
    [SerializeField]
    private Image titleButtonImage;

	private RectTransform rect;
	[SerializeField]
	private AudioSource audioSource;

    [SerializeField] private InputActionReference actionMove;

    //入力モード　true マウス　false コントローラ
	private bool bMouseMode;
    //マウス座標
    private Vector3 mousePos;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		// ボタンの処理を登録
		backButton.onClick.AddListener(Back);
		//OpitonButton.onClick.AddListener(ChangePanel);
		titleButton.onClick.AddListener(ChangeTitle);

        //パネル有効化
		pausePanel.SetActive(true);
		optionPanel.SetActive(true);
		keyConfigPanel.SetActive(true);

		rect = GetComponent<RectTransform>();

        //最初は非表示
		if (gameObject.activeSelf)gameObject.SetActive(false);

        actionMove.action.performed += OnMove;
        actionMove.action.canceled += OnMove;
        
	}

    private void OnEnable()
    {
        actionMove.ToInputAction().Enable();

        ActivePanel = pausePanel;
        InitInput();
    }

    private void OnDisable()
    {
        actionMove.ToInputAction().Disable();
    }

    private void OnDestroy()
    {
        actionMove.action.performed -= OnMove;
        actionMove.action.canceled -= OnMove;
    }

    private void FixedUpdate()
	{
		if (ActivePanel == pausePanel && rect.localPosition != new Vector3(0, 0, 0))
			rect.localPosition = Vector3.MoveTowards(rect.localPosition, new Vector3(0, 0, 0), PanelMoveValue);
		if (ActivePanel == optionPanel && rect.localPosition != new Vector3(-1920 * 1, 0, 0))
			rect.localPosition = Vector3.MoveTowards(rect.localPosition, new Vector3(-1920 * 1, 0, 0), PanelMoveValue);
		if (ActivePanel == keyConfigPanel && rect.localPosition != new Vector3(-1920 * 2, 0, 0))
			rect.localPosition = Vector3.MoveTowards(rect.localPosition, new Vector3(-1920 * 2, 0, 0), PanelMoveValue);
	}

	private void Update()
	{
        if (ActivePanel != pausePanel) return;

        //マウスの状態を更新
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        if (bMouseMode) return;

        //マウスが動かされたらマウス入力に切り替え
        if (Vector3.Distance(oldMousePos, mousePos) >= 1.0f)
        {
            ChangeInput();
        }
    }

	private void OnMove(InputAction.CallbackContext context)
	{
        if (!gameObject.activeSelf) return;
        if (!PauseManager.isPaused)return;
        if (!bMouseMode) return;
        if (ActivePanel != pausePanel) return;

        //マウス→コントローラ
        ChangeInput();
	}

	void Pause()
	{
		if (!PauseManager.NoMenu)
			gameObject.SetActive(true);
	}

	void Resumed()
	{
		ActivePanel = pausePanel;
		rect.localPosition = new Vector3(0, 0, 0);
		gameObject.SetActive(false);
	}

	private void Back()
	{
		PauseManager.Resume();
		PauseManager.isPaused = false;
	}

	private void ChangeTitle()
	{
		SaveManager.SaveAll();  // タイトル戻る前にセーブ
		
		MySceneManager.GameData.oldScene = MySceneManager.GameData.nowScene;
		MySceneManager.GameData.nowScene = (int)MySceneManager.SceneState.SCENE_TITLE;

        MySceneManager.SceneChange(MySceneManager.SceneState.SCENE_TITLE);
	}

	public void ChangePanel(GameObject panelObj)
	{
		ChangePanel(panelObj.name);
	}
	public void ChangePanel(string panelName)
	{
		if (panelName.Equals(pausePanel.name))
        {
            ActivePanel = pausePanel;
            InitInput();
        }

		if (panelName.Equals(optionPanel.name))
			ActivePanel = optionPanel;

		if (panelName.Equals(keyConfigPanel.name))
			ActivePanel = keyConfigPanel;
	}

	public void DecisionSound()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
	}

	public void SelectSound()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
	}

	public void SlideSound()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.SLIDE_001);
	}

    public void InitInput()
    {
        mousePos = Input.mousePosition;
        bMouseMode = true;
        ChangeInput();
    }

    public void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouseMode)
        {
            EventSystem.current.SetSelectedGameObject(null);
            backButton.Select();
        }else{//コントローラ→　マウス
            EventSystem.current.SetSelectedGameObject(null);
        }

        //入力切替
        bMouseMode = !bMouseMode;
        //ボタンのraycastTarget切り替え
        backButtonImage.raycastTarget = bMouseMode;
        optionButtonImage.raycastTarget = bMouseMode;
        titleButtonImage.raycastTarget = bMouseMode;
        Cursor.visible = bMouseMode;
    }
}
