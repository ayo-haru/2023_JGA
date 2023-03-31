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
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
	[Header("パネル移動速度")]
	[SerializeField]
	private float PanelMoveValue = 100;

	private BaseSceneManager bsm;

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

		bsm = FindObjectOfType<BaseSceneManager>();

		if (gameObject.activeSelf)
			gameObject.SetActive(false);

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

	void Pause()
	{
		gameObject.SetActive(true);
	}

	void Resumed()
	{
		gameObject.SetActive(false);
	}

	private void Back()
	{
		Resumed();
		PauseManager.isPaused = false;
	}

	private void ChangeTitle()
	{
		//bsm.SceneChange(MySceneManager.SceneState.SCENE_TITLE);
		//SceneChange(MySceneManager.SceneState.SCENE_TITLE);
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
}
