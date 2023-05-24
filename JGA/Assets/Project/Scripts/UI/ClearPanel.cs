//=============================================================================
// @File	: [ClearPanel.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/6	スクリプト作成
//=============================================================================
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ClearPanel : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField, Header("Clear")] private Image Clear;

    //ボタンの色
    [SerializeField, Header("ボタンの色")] private Color buttonColor;

    //ボタン
    [SerializeField, Header("NEXT DAY")] private Button nextDayButton;
    [SerializeField, Header("BACK TO TITLE")] private Button backToTitleButton;

    public enum EClearPanelButton {NEXT_DAY,BACK_TO_TITLE,};

    //マウス
    private Vector3 mousePos = Vector3.zero;
    private bool bMouse = true;

    //次のシーン
    private int nextScene = -1;

	void Awake()
	{
        audioSource = GetComponent<AudioSource>();
        nextScene = -1;
        bMouse = true;
        mousePos = Vector3.zero;
	}

    private void OnEnable()
    {
        nextScene = -1;
        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;
        mousePos = Input.mousePosition;
        if (gamepad != null)
        {
            bMouse = true;
            ChangeInput();
        }
        Clear.fillAmount = 0.0f;
    }

    private void Update()
    {
        Clear.fillAmount += Time.deltaTime;
        //マウス、コントローラの値取得
        Gamepad gamepad = Gamepad.current;
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        //コントローラ入力モードで、コントローラがない場合はマウス操作に切り替え
        if (gamepad == null)
        {
            if(!bMouse)ChangeInput();
            return;
        }

        //マウス有効の状態でコントローラが押されたらコントローラ入力にする
        //マウス無効でマウスが動いたらマウス入力を有効
        if (bMouse)
        {
            if (gamepad.leftStick.ReadValue() != Vector2.zero || gamepad.aButton.wasReleasedThisFrame) ChangeInput();
        }
        else
        {
            if (Vector3.Distance(mousePos, oldMousePos) >= 1.0f) ChangeInput();
        }
    }

    #region クリア画面のボタン
    /// <summary>
    /// NEXT DAYボタン
    /// </summary>
    public void NextDayButton()
    {
        SoundDecisionSE();
        if (System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length > MySceneManager.GameData.nowScene)
        {  // 最大シーンではないとき
            nextScene = MySceneManager.GameData.nowScene + 1;
        }else{
            nextScene = MySceneManager.GameData.nowScene;
        }
    }
    /// <summary>
    /// BACK TO TITLEボタン
    /// </summary>
    public void BackToTitleButton()
    {
        SoundDecisionSE();
        nextScene = (int)MySceneManager.SceneState.SCENE_TITLE;
    }
    #endregion
    #region SE鳴らす関数
    public void SoundSelectSE()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }

    public void SoundDecisionSE()
    {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }
    #endregion
    /// <summary>
    /// 入力切替
    /// </summary>
    private void ChangeInput()
    {
        //マウス→コントローラ
        if (bMouse)
        {
            //マウスカーソル非表示
            Cursor.visible = false;

            ColorBlock colors = nextDayButton.colors;
            colors.highlightedColor = Color.white;
            nextDayButton.colors = backToTitleButton.colors = colors;

            //デフォルトのボタンを選択
            ControllerChangeSelect(EClearPanelButton.NEXT_DAY);
        }
        else//コントローラ→マウス
        {
            //マウスカーソル表示
            Cursor.visible = true;

            ColorBlock colors = nextDayButton.colors;
            colors.highlightedColor = buttonColor;
            nextDayButton.colors = backToTitleButton.colors = colors;
            ControllerNoneSelect();
        }

        bMouse = !bMouse;
    }

    public void ControllerChangeSelect(EClearPanelButton _select)
    {
        ControllerNoneSelect();
        switch (_select)
        {
            case EClearPanelButton.NEXT_DAY:
                nextDayButton.Select();
                break;
            case EClearPanelButton.BACK_TO_TITLE:
                backToTitleButton.Select();
                break;
        }
        SoundSelectSE();
    }

    public void ControllerNoneSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public int GetNextScene()
    {
        return nextScene;
    }
}
