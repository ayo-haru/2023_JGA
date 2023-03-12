//=============================================================================
// @File	: [TestPause.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/12	スクリプト作成
//=============================================================================
using UniRx;
using UnityEngine;

public class TestPause : MonoBehaviour
{
	[SerializeField]
	private GameObject PauseUI;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		if (PauseUI.activeSelf)
			PauseUI.SetActive(false);
	}

	void Pause()
	{
		PauseUI.SetActive(true);
	}

	void Resumed()
	{
		PauseUI.SetActive(false);
	}
}
