//=============================================================================
// @File	: [TitleScenManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/13	スクリプト作成
// 2023/03/21	(小楠)ボタン操作追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleScenManager : BaseSceneManager
{
    //ゲームを始めるボタン
    [SerializeField] private Button startButton;
    private Image startImage;
    //オプションボタン
    [SerializeField] private Button optionButton;
    private Image optionImage;
    //ゲームをやめるボタン
    [SerializeField] private Button exitButton;
    private Image exitImage;
    //入力フラグ
    private bool bMouse = true;

    private Vector3 mousePos;
    //フェードパネル
    [SerializeField] private Image fadePanel;

    private enum ETitleSelect {TITLESELECT_START,TITLESELECT_OPTION,TITLESELECT_EXIT,MAX_TITLESELECT};
    private ETitleSelect select = ETitleSelect.TITLESELECT_START;

    private void Awake() {
        Init();

        startImage = startButton.GetComponent<Image>();
        optionImage = optionButton.GetComponent<Image>();
        exitImage = exitButton.GetComponent<Image>();
    }

    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return;
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        //マウス有効の状態でコントローラが押されたらコントローラ入力にする
        //マウス無効でマウスが動いたらマウス入力を有効
        if (bMouse)
        {
            if (gamepad.dpad.up.wasReleasedThisFrame || gamepad.dpad.down.wasReleasedThisFrame || gamepad.aButton.isPressed) ChangeInput();
        }
        else
        {
            if (mousePos != oldMousePos) ChangeInput();
        }

        //コントローラ入力
        if (!bMouse)
        {
            ETitleSelect old = select;

            if(gamepad.dpad.up.wasReleasedThisFrame)
            {
                if(select > 0)--select;
            }
            //左スティック　↓
            if(gamepad.dpad.down.wasReleasedThisFrame)
            {
                if (select < ETitleSelect.MAX_TITLESELECT - 1) ++select;
            }
            //Aボタン
            if (gamepad.aButton.isPressed)
            {
                switch (select)
                {
                    case ETitleSelect.TITLESELECT_START:StartButton();break;
                    case ETitleSelect.TITLESELECT_OPTION:OptionButton();break;
                    case ETitleSelect.TITLESELECT_EXIT:ExitButton();break;
                }
            }

            if(select != old)ControllerChangeSelect(select);
        }
    }

    public void StartButton()
    {
        SceneChange(MySceneManager.SceneState.SCENE_GAME);
    }
    public void OptionButton()
    {
        //オプション画面を開く
    }
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //マウスカーソル非表示
            Cursor.visible = false;
            fadePanel.raycastTarget = true;
            ControllerChangeSelect(select);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;
            fadePanel.raycastTarget = false;
            ControllerResetSelect();
        }

        bMouse = !bMouse;
    }

    private void ControllerChangeSelect(ETitleSelect _select)
    {
        ControllerResetSelect();
        switch (_select)
        {
            case ETitleSelect.TITLESELECT_START:
                startImage.color = Color.gray;
                break;
            case ETitleSelect.TITLESELECT_OPTION:
                optionImage.color = Color.gray;
                break;
            case ETitleSelect.TITLESELECT_EXIT:
                exitImage.color = Color.gray;
                break;
        }
    }
    private void ControllerResetSelect()
    {
        startImage.color = Color.white;
        optionImage.color = Color.white;
        exitImage.color = Color.white;
    }
}
