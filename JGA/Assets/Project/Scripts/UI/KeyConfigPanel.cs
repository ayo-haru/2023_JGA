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
using System.Collections.Generic;

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
        //MenuInputManagerが置かれていなかった場合は生成
        if (MenuInputManager.Instance == null)
        {
            MenuInputManager.Create();
        }

        ChangePanel(settingMode);
	}
#if false
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
#endif
	public void StartKeyConfig()
	{
		EventSystem.current.SetSelectedGameObject(null);
		switch (settingMode)
		{
			case SettingMode.KEYBOARD:
                MenuInputManager.PushMenu(new MenuSettingItem(KeyMoveButton));
				break;
			case SettingMode.GAMEPAD:
                MenuInputManager.PushMenu(new MenuSettingItem(PadMoveButton));
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

    public void CloseKeyConfig()
    {
        MenuInputManager.PopMenu();
    }
}
