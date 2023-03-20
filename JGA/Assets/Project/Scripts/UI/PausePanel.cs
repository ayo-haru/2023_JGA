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
	[SerializeField]
	private Button backButton;
	[SerializeField]
	private Button OpitonButton;
	[SerializeField]
	private Button TitleButton;
	[SerializeField]
	private Button ExitButton;

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
		TitleButton.onClick.AddListener(ChangeTitle);

		if (gameObject.activeSelf)
			gameObject.SetActive(false);
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
}
