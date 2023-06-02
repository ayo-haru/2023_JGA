//=============================================================================
// @File	: [KeyConfigPanel.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/15	スクリプト作成
//=============================================================================
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyConfigPanel : MonoBehaviour
{
	public enum SettingMode
	{
		KEYBOARD,
		GAMEPAD,
	};

	[SerializeField] TMP_Text	keyConfigText;
	[SerializeField] GameObject	LayoutKeyboard;
	[SerializeField] GameObject	LayoutGamePad;
	[SerializeField] GameObject	WaitPanel;

	[SerializeField] Button KeyMoveButton;
	[SerializeField] Button PadMoveButton;

	SettingMode settingMode = SettingMode.KEYBOARD;

	void Awake()
	{
		ChangePanel(settingMode);
	}

	private void Update()
	{
		// ゲームパッドを検出
		//if (settingMode == SettingMode.KEYBOARD && Gamepad.current != null)
		//{
		//	//// ゲームパッド設定画面確認
		//	// switchingPanel.SetActive(true);

		//	settingMode = SettingMode.GAMEPAD;
		//	ChangePanel(settingMode);
		//}

	}

	public void StartKeyConfig()
	{
		EventSystem.current.SetSelectedGameObject(null);
		switch (settingMode)
		{
			case SettingMode.KEYBOARD:
				KeyMoveButton.Select();
				break;
			case SettingMode.GAMEPAD:
				PadMoveButton.Select();
				break;
		}
	}

	public void ChangePanel(GameObject obj)
	{
		if (obj == LayoutKeyboard)
		{
			settingMode = SettingMode.KEYBOARD;
			ChangePanel(settingMode);
			keyConfigText.text = "KeyBoard";
		}
		else if (obj == LayoutGamePad)
		{
			settingMode = SettingMode.GAMEPAD;
			ChangePanel(settingMode);
			keyConfigText.text = "GamePad";
		}
		else
		{
			Debug.LogError($"オブジェクト指定が間違っています");
		}
	}

	public void ChangePanel(SettingMode mode)
	{
		settingMode = mode;
		LayoutKeyboard.SetActive(mode == SettingMode.KEYBOARD);
		LayoutGamePad.SetActive(mode == SettingMode.GAMEPAD);
	}

	//private void FixedUpdate()
	//{
	//}
}
