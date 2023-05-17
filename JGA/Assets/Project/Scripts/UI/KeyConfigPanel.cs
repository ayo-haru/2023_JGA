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
	enum SettingMode
	{
		KEYBOARD,
		GAMEPAD,
	};

	[SerializeField] GameObject			LayoutKeyboard;
	[SerializeField] GameObject			LayoutGamePad;
	[SerializeField] GameObject			WaitPanel;

	SettingMode settingMode = SettingMode.KEYBOARD;

	void Awake()
	{
	}

	private void Update()
	{
		// ゲームパッドを検出
		if (settingMode == SettingMode.KEYBOARD && Gamepad.current != null)
		{
		}

	}

	private void FixedUpdate()
	{
	}
}
