//=============================================================================
// @File	: [PausePanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/16	スクリプト作成
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
	private Button OpitonButton;
	[SerializeField]
	private Button TitleButton;

	private RectTransform rect;
	[SerializeField]
	private AudioSource audioSource;

	private MyContorller gameInputs;            // 方向キー入力取得

	private bool bGamePad;
	private bool bNoMouseMode;

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
		TitleButton.onClick.AddListener(ChangeTitle);

		pausePanel.SetActive(true);
		optionPanel.SetActive(true);
		keyConfigPanel.SetActive(true);

		rect = GetComponent<RectTransform>();

		if (gameObject.activeSelf)
			gameObject.SetActive(false);

		Button[] buttons = GetComponentsInChildren<Button>();
		for (int i = 0; i < buttons.Length; i++)
		{
			//var e = buttons[i].AddComponent<EventTrigger>();
			//EventTrigger.Entry entry = e.Entry;
			//entry.eventID = EventTriggerType.PointerDown;
			//entry.callback.AddListener((eventDate) => DecisionSound());
			//e.triggers.Add(entry);
		}

		// Input Actionインスタンス生成
		gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Menu.Move.performed += OnMove;

		// Input Actionを有効化
		gameInputs.Enable();

		if (Gamepad.current == null)
			bGamePad = false;
		else
		{
			bNoMouseMode = true;
			backButton.Select();        // 最初に選択状態にしたいボタンの設定
			bGamePad = true;
		}

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
		if (Gamepad.current == null)
			bGamePad = false;
		else if (!bGamePad)
		{
			bNoMouseMode = true;
			backButton.Select();        // 最初に選択状態にしたいボタンの設定
			bGamePad = true;
		}

		Debug.Log($"select:{EventSystem.current.currentSelectedGameObject}");
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		if (!PauseManager.isPaused)
			return;

		Vector2 move = context.ReadValue<Vector2>();

		Debug.Log($"move:{move}");

		if (!bNoMouseMode)
		{
			bNoMouseMode = true;
			backButton.Select();        // 最初に選択状態にしたいボタンの設定
		}
		else
		{
			var select = EventSystem.current.currentSelectedGameObject;
			Debug.Log($"select:{select}");

			//if (move.x == 1.0f)
			//{

			//}
			//if (move.x == -1.0f)
			//{

			//}
			//if (move.y == 1.0f)
			//{
			//	if (select == backButton.gameObject)
			//	{
			//		TitleButton.Select();
			//	}
			//	else if (select == OpitonButton.gameObject)
			//	{
			//		backButton.Select();
			//	}
			//	else if (select == TitleButton.gameObject)
			//	{
			//		OpitonButton.Select();
			//	}
			//}
			//if (move.y == -1.0f)
			//{
			//	if (select == backButton.gameObject)
			//	{
			//		OpitonButton.Select();
			//	}
			//	else if (select == OpitonButton.gameObject)
			//	{
			//		TitleButton.Select();
			//	}
			//	else if (select == TitleButton.gameObject)
			//	{
			//		backButton.Select();
			//	}
			//}
		}
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
		Resumed();
		PauseManager.isPaused = false;
	}

	private void ChangeTitle()
	{
		MySceneManager.SceneChange(MySceneManager.SceneState.SCENE_TITLE);
	}

	public void ChangePanel(GameObject panelObj)
	{
		ChangePanel(panelObj.name);
	}
	public void ChangePanel(string panelName)
	{
		if (panelName.Equals(pausePanel.name))
			ActivePanel = pausePanel;

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
}
