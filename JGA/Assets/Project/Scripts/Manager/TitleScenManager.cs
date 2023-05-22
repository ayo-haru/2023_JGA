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
// 2023/03/28	(小楠)音追加
// 2023/04/05	(小楠)オプション画面追加
// 2023/04/06	(小楠)オプションのスライド移動追加
// 2023/04/08	(小楠)オプションの処理全部消した
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class TitleScenManager : BaseSceneManager
{
    [SerializeField,Header("ゲームを始める")] private Selectable startButton;
    [SerializeField,Header("オプション")] private Selectable optionButton;
    [SerializeField,Header("ゲームをやめる")] private Selectable exitButton;

    [SerializeField, Header("オプション")] private OptionPanel optionPanel;

    //入力フラグ
    private bool bMouse = true;
    //マウス位置
    private Vector3 mousePos;

    //タイトル画面のボタン
    private enum ETitleSelect {TITLESELECT_START,TITLESELECT_OPTION,TITLESELECT_EXIT,MAX_TITLESELECT};

    //次のシーン
    private int nextScene;

    private AudioSource audioSource;
    private bool bFlag = false;

    private void Awake() {
        /*
         * InitでAddComponentしてるもの
         * ・AudioSource
         * ・UIManager
         */
        Init();

        //セーブデータ読み込み
        MySceneManager.GameData.isContinueGame = SaveSystem.load();

        //もしセーブデータがなかったら次に遷移するシーンはゲーム001
        if (MySceneManager.GameData.isContinueGame) {
            SaveManager.LoadAll();
            nextScene = MySceneManager.GameData.nowScene;
        } else {
            MySceneManager.GameData.nowScene = (int)MySceneManager.SceneState.SCENE_TITLE;
            nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;
        }

        // BGM再生用にオーディオソース取得
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        // BGM再生
        SoundManager.Play(audioSource, SoundManager.EBGM.TITLE_001);
    }

    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;

        bool beforeFlag = bFlag;
        bFlag = optionPanel.IsOpen();
        //オプション画面が開かれているときは処理しない
        if (bFlag) return;
        //オプション画面から戻って来た時の初期化処理
        if (beforeFlag)
        {
            bMouse = (gamepad != null);
            ChangeInput();
        }

        if (gamepad == null) return;
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        //マウス有効の状態でコントローラが押されたらコントローラ入力にする
        //マウス無効でマウスが動いたらマウス入力を有効
        if (bMouse)
        {
            if (gamepad.leftStick.ReadValue() != Vector2.zero || gamepad.aButton.wasReleasedThisFrame) ChangeInput();
        }
        else
        {
            if (mousePos != oldMousePos) ChangeInput();
        }

    }

#region タイトル画面のボタン
    public void StartButton()
    {
        SoundSEDecision();
        SceneChange(nextScene);
        //コントローラ入力の場合マウスカーソルが非表示のままになってしまうので表示する
        if (!bMouse)Cursor.visible = true;
    }
    public void OptionButton()
    {
        SoundSEDecision();
        ControllerNoneSelect();
        //オプション画面を開く
        optionPanel.Open();
    }
    public void ExitButton()
    {
        SoundSEDecision();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
#endregion


    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //マウスカーソル非表示
            Cursor.visible = false;
            ControllerChangeSelect(ETitleSelect.TITLESELECT_START);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;
            ControllerNoneSelect();
        }

        bMouse = !bMouse;
    }

    private void ControllerChangeSelect(ETitleSelect _select)
    {
        ControllerNoneSelect();
        switch (_select)
        {
            case ETitleSelect.TITLESELECT_START:
                startButton.Select();
                break;
            case ETitleSelect.TITLESELECT_OPTION:
                optionButton.Select();
                break;
            case ETitleSelect.TITLESELECT_EXIT:
                exitButton.Select();
                break;
        }
        SoundSESelect();
    }

    private void ControllerNoneSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SoundSESelect()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    public void SoundSEDecision()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }
}
