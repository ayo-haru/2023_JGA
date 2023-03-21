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
    //オプションボタン
    [SerializeField] private Button optionButton;
    //ゲームをやめるボタン
    [SerializeField] private Button exitButton;
    //入力フラグ
    private bool bMouse = true;

    private Vector3 mousePos;

    private enum ETitleSelect {TITLESELECT_START,TITLESELECT_OPTION,TITLESELECT_EXIT,MAX_TITLESELECT};
    private ETitleSelect select = ETitleSelect.TITLESELECT_START;

    private void Awake() {
        Init();

        
    }

    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
        if (Input.GetKeyUp(KeyCode.Return)) {
			SceneChange(MySceneManager.SceneState.SCENE_GAME);
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
}
