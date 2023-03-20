//=============================================================================
// @File	: [OptionPanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/20	スクリプト作成
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
	[SerializeField]
	private GameObject MainPanel;

	[SerializeField]
	private Slider BgmSlider;
	[SerializeField]
	private Slider SeSlider;
	[SerializeField]
	private Button KeyConfigButton;
	[SerializeField]
	private Button BackButton;

	void Awake()
	{

		// ボタンの処理を登録
		BackButton.onClick.AddListener(Back);
	}

	private void Back()
	{
	}
}
