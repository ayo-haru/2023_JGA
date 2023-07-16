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
// 2023/05/26	(小楠)はじめからと続きからを追加
// 2023/05/29	(小楠)ゲームパッド繋がってないときにキーボードで操作できないのを修正
// 2023/07/14	(小楠)使ってない変数とか消したりした　UIのアニメーション開始の呼び出しを変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScenManager : BaseSceneManager {
    [SerializeField, Header("はじめから")] private Selectable startButton;
    private BoxCollider startButtonCollider;
    [SerializeField, Header("つづきから")] private Selectable continueButton;
    private BoxCollider continueButtonCollider;
    [SerializeField, Header("オプション")] private Selectable optionButton;
    private BoxCollider optionButtonCollider;
    [SerializeField, Header("ゲームをやめる")] private Selectable exitButton;
    private BoxCollider exitButtonCollider;

    [SerializeField] private GameObject optionObject;
    [SerializeField, Header("オプション")] private OptionPanel optionPanel;

    //続きからボタン隠す用
    private SpriteRenderer continueHideImage;

    //次のシーン
    private int nextScene;

    private AudioSource audioSource;

    private void Awake() {
        /*
         * InitでAddComponentしてるもの
         * ・AudioSource
         * ・UIManager
         */
        Init();

        //セーブデータ読み込み
        GameData.isContinueGame = SaveSystem.load();

        //もしセーブデータがなかったら次に遷移するシーンはゲーム001
        //if (MySceneManager.GameData.isContinueGame) {
        //    SaveManager.LoadAll();  // セーブデータロード
        //    nextScene = MySceneManager.GameData.nowScene;   // 次のシーンを更新
        //} else {
        //    MySceneManager.GameData.nowScene = (int)MySceneManager.SceneState.SCENE_TITLE;
        //    nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;
        //}


        //MenuInputManagerが置かれていなかった場合は生成
        if (MenuInputManager.Instance == null)
        {
            MenuInputManager.Create();
        }

        // BGM再生用にオーディオソース取得
        audioSource = GetComponent<AudioSource>();

        //ボタンの当たり判定と画像のコンポーネント取得
        startButtonCollider = startButton.GetComponent<BoxCollider>();
        continueButtonCollider = continueButton.GetComponent<BoxCollider>();
        optionButtonCollider = optionButton.GetComponent<BoxCollider>();
        exitButtonCollider = exitButton.GetComponent<BoxCollider>();
        continueHideImage = continueButton.transform.GetChild(0).GetComponent<SpriteRenderer>();

        //セーブデータが存在しない場合は続きからボタンを無効化
        if (!GameData.isContinueGame)
        {
            //続きからボタン無効化
            continueButton.interactable = false;
            //ナビゲーション変更
            Navigation navigation = startButton.navigation;
            navigation.selectOnDown = optionButton;
            startButton.navigation = navigation;
            navigation = optionButton.navigation;
            navigation.selectOnUp = startButton;
            optionButton.navigation = navigation;
        }
        else
        {
            //続きから隠すようの画像を非表示にする
            if(continueHideImage)continueHideImage.enabled = false;
        }

    }
    private void OnDestroy()
    {
        if(MenuInputManager.Instance)MenuInputManager.PopMenu();
    }

    private void Start() {
        //BGM再生
        SoundManager.Play(audioSource, SoundManager.EBGM.TITLE_001);
        //フェード中だったら待機して音を止める
        StartCoroutine(WaitFade());
        //フェードアウトが終わったらアニメーションを再生
        StartCoroutine(StartUIAnimation());

        List<BoxCollider> boxColliders = new List<BoxCollider>();
        boxColliders.Add(startButtonCollider);
        boxColliders.Add(continueButtonCollider);
        boxColliders.Add(optionButtonCollider);
        boxColliders.Add(exitButtonCollider);
        MenuInputManager.PushMenu(new MenuSettingItem(startButton, boxColliders));
    }

#if false
    /// <summary>
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {

    }
#endif

#region タイトル画面のボタン
    public void StartButton() {
        GameData.oldScene = (int)MySceneManager.SceneState.SCENE_TITLE;  // 今のシーンをひとつ前のシーンとして保存
        GameData.nowScene = nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;   // 次のシーンを更新
        //初めから用に初期化
        GameData.isContinueGame = false; // startでセーブデータがあったとしてもfalseにする
        SaveManager.SaveInitDataAll();  // データをリセットして保存
        SceneChange(nextScene);

        SoundSEDecision();
    }

    public void ContinueButton()
    {
        //非有効だったら実行しない
        if (!continueButton.IsInteractable()) return;

        SaveManager.LoadAll();  // セーブデータロード
        nextScene = GameData.nowScene;   // 次のシーンを更新。セーブデータのロードをしてnowSceneには前回のシーン番号が入っているからnextにnowを代入
        GameData.oldScene = (int)MySceneManager.SceneState.SCENE_TITLE;  // 今のシーンをひとつ前のシーンとして保存
        SceneChange(nextScene);
#if false
        if (MySceneManager.GameData.isContinueGame) {   // セーブデータが存在してたか
            // セーブデータ有りなのでデータロード
            SaveManager.LoadAll();  // セーブデータロード
            nextScene = MySceneManager.GameData.nowScene;   // 次のシーンを更新。セーブデータのロードをしてnowSceneには前回のシーン番号が入っているからnextにnowを代入
        } else {
            // セーブデータがなかったので「はじめから」と同じように振る舞う
            MySceneManager.GameData.nowScene = nextScene = (int)MySceneManager.SceneState.SCENE_GAME_001;
        }
#endif
        SoundSEDecision();
    }

    public void OptionButton() {
        //オプション画面を開く
        optionObject.SetActive(true);
        optionPanel.NewOpen();
        SoundSEDecision();
    }
    public void ExitButton() {
        SoundSEDecision();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
#endregion
#region SE鳴らす関数
    public void SoundSESelect() {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
    }
    public void SoundSEDecision() {
        if (!audioSource) return;
        SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
    }
#endregion
    IEnumerator WaitFade() {
        audioSource.Pause();
        yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.Default);
        audioSource.UnPause();
    }

    IEnumerator StartUIAnimation()
    {
        yield return new WaitUntil(() => FadeManager.fadeMode != FadeManager.eFade.FadeOut);

        ButtonWrite buttonWrite;
        if (startButton.TryGetComponent(out buttonWrite)) buttonWrite.StartWriteAnimation();
        if (continueButton.TryGetComponent(out buttonWrite)) buttonWrite.StartWriteAnimation();
        if (optionButton.TryGetComponent(out buttonWrite)) buttonWrite.StartWriteAnimation();
        if (exitButton.TryGetComponent(out buttonWrite)) buttonWrite.StartWriteAnimation();

        if (!continueHideImage) yield break;
        if (!continueHideImage.enabled) yield break; 
        if (continueHideImage.TryGetComponent(out buttonWrite)) buttonWrite.StartWriteAnimation();
    }
}
