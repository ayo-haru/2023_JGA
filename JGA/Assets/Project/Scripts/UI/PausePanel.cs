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
	[Header("有効パネル")]
	[SerializeField]
	private GameObject pausePanel;
	[SerializeField]
	private GameObject optionPanel;
	[SerializeField]
	private GameObject keyConfigPanel;


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

		if (gameObject.activeSelf)
			gameObject.SetActive(false);

		rect = GetComponent<RectTransform>();
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
		//SceneChange(MySceneManager.SceneState.SCENE_TITLE);
	}

	public void ChangePanel(GameObject panelObj)
	{
		ChangePanel(panelObj.name);
	}
	public void ChangePanel(string panelName)
	{
		pausePanel.SetActive(panelName.Equals(pausePanel.name));
		optionPanel.SetActive(panelName.Equals(optionPanel.name));
		keyConfigPanel.SetActive(panelName.Equals(keyConfigPanel.name));

		if (pausePanel.activeSelf)
			rect.localPosition = new Vector3(0, 0, 0);
		if (optionPanel.activeSelf)
			rect.localPosition = new Vector3(-1920 * 1, 0, 0);
		if (keyConfigPanel.activeSelf)
			rect.localPosition = new Vector3(-1920 * 2, 0, 0);

	}
}
