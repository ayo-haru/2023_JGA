//=============================================================================
// @File	: [TutorialManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/22	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class TutorialManager : MonoBehaviour {
    enum TutorialState {
    }

    //---キャンバス
    GameObject canvas;
    RectTransform canvasRT;

    //---フェードのパネル
    GameObject fade;

    //---プレイヤー
    GameObject player;

    private List<ITurorial> tutorialTask;   // タスク一覧
    private ITurorial currentTask;  // 現在のタスク

    private static ReactiveProperty<bool> isExecution = new ReactiveProperty<bool>(false);   // チュートリアルをやるか
    private bool isTaskFin;                     // タスクが終了しているか

    [SerializeField]
    private List<GameObject> tutorialText;  // 表示するテキストのプレハブ
    private GameObject currentTextObj;  // 現在のテキストのプレハブ
    private Image currentText;  // 現在のテキストのImageコンポーネント


    //**デバッグ用***************************************************************
#if UNITY_EDITOR
    private bool debugTaskFin;
#endif
    //*****************************************************************
    private void Awake() {
        //----- イベント登録 -----
        //isExecution.Subscribe(_ => { });

        //----- 変数初期化 -----
        isTaskFin = false;
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start() {
        canvas = GameObject.Find("Canvas");
        canvasRT = canvas.GetComponent<RectTransform>();

        fade = GameObject.Find("FadePanel");

        player = GameObject.FindWithTag("Player");

        // チュートリアルの一覧
        tutorialTask = new List<ITurorial>()
        {
            new TutorialTask001(),
            new TutorialTask002(),
            new TutorialTask003(),
            new TutorialTask004(),
            new TutorialTask005(),
            new TutorialTask006(),
            new TutorialTask007(),
            new TutorialTask008(),
            new TutorialTask009(),
            new TutorialTask010(),
            new TutorialTask011(),
            new TutorialTask012(),
            new TutorialTask013(),
            new TutorialTask014(),
            new TutorialTask015(),
            new TutorialTask016(),
            new TutorialTask017(),
            new TutorialTask018(),
            new TutorialTask019(),
            new TutorialTask020(),
            new TutorialTask021(),
            new TutorialTask022(),
            new TutorialTask023(),
            new TutorialTask024(),
            new TutorialTask025(),
            new TutorialTask026(),
            new TutorialTask027(),
        };

        // 最初のチュートリアルを設定
        SetTaskObj(tutorialTask.First());
        StartCoroutine(SetCurrentTask(tutorialTask.First()));

        //**デバッグ用***************************************************************
#if UNITY_EDITOR
        debugTaskFin = false;
#endif
        //*****************************************************************
    }


    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        //----- チュートリアルを実行するか -----
        if (!isExecution.Value) {
            return;
        }

        //**デバッグ用***************************************************************
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.F2)) {
            debugTaskFin = true;
        } else {
            debugTaskFin = false;
        }
#endif
        //*****************************************************************

        // チュートリアルが存在しタスクが実行されていない場合に処理
        if (currentTask != null && !isTaskFin) {
            // 現在のチュートリアルが実行されたか判定
            if (currentTask.CheckTask() || debugTaskFin) {
                isTaskFin = true;

                DOVirtual.DelayedCall(currentTask.GetTransitionTime(), () => {

                    // １秒かけて非表示
                    currentText.DOFade(endValue: 0.0f, duration: 1.0f);

                    tutorialTask.RemoveAt(0);
                    tutorialText.RemoveAt(0);

                    var nextTask = tutorialTask.FirstOrDefault();
                    if (nextTask != null) {
                        SetTaskObj(nextTask);
                        StartCoroutine(SetCurrentTask(nextTask, 1f));
                    }
                });
            }
        }
    }

    /// <summary>
    /// 新しいチュートリアルタスクを設定する
    /// </summary>
    /// <param name="task"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    protected IEnumerator SetCurrentTask(ITurorial task, float time = 0) {
        // timeが指定されている場合は待機
        yield return new WaitForSeconds(time);

        currentTask = task;
        isTaskFin = false;

        // 説明文を設定
        if (currentTextObj) {
            Destroy(currentTextObj);
        }
        currentTextObj = tutorialText.FirstOrDefault();
        currentTextObj = Instantiate(currentTextObj, canvasRT);
        currentTextObj.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする
        currentText = currentTextObj.GetComponent<Image>();


        // チュートリアルタスク設定時用の関数を実行
        task.OnTaskSetting();

        // １秒かけて表示
        currentText.DOFade(endValue: 1.0f, duration: 1.0f);
    }

    void SetTaskObj(ITurorial task) {
        switch (task) {
            case TutorialTask001:
                break;
            case TutorialTask002:
                break;
            case TutorialTask003:
                task.AddNeedObj(player);
                break;
            case TutorialTask004:
                task.AddNeedObj(player);
                break;
            case TutorialTask005:
                task.AddNeedObj(player);
                break;
            case TutorialTask006:
                task.AddNeedObj(player);
                break;
            case TutorialTask007:
                task.AddNeedObj(player);
                break;
            case TutorialTask008:
                break;
            case TutorialTask009:
                task.AddNeedObj(player);
                break;
            case TutorialTask010:
                task.AddNeedObj(player);
                break;
            case TutorialTask011:
                break;
            case TutorialTask012:
                task.AddNeedObj(player);
                break;
            case TutorialTask013:
                task.AddNeedObj(player);
                task.AddNeedObj(GameObject.Find("CardBoard_001"));
                task.AddNeedObj(GameObject.Find("CardBoard_002"));
                task.AddNeedObj(GameObject.Find("CardBoard_007"));
                break;
            case TutorialTask014:
                task.AddNeedObj(player);
                task.AddNeedObj(GameObject.Find("CardBoard_001"));
                task.AddNeedObj(GameObject.Find("CardBoard_002"));
                task.AddNeedObj(GameObject.Find("CardBoard_007"));
                break;
            case TutorialTask015:
                task.AddNeedObj(player);
                break;
            case TutorialTask016:
                task.AddNeedObj(player);
                task.AddNeedObj(GameObject.Find("TutorialWait_Guest002"));
                task.AddNeedObj(GameObject.Find("TutorialWait_Guest003"));
                break;
            case TutorialTask017:
                task.AddNeedObj(player);
                break;
            case TutorialTask018:
                task.AddNeedObj(GameObject.Find("TutorialWait_Guest002"));
                task.AddNeedObj(GameObject.Find("TutorialWait_Guest003"));
                break;
            case TutorialTask019:
                break;
            case TutorialTask020:
                break;
            case TutorialTask021:
                break;
            case TutorialTask022:
                break;
            case TutorialTask023:
                break;
            case TutorialTask024:
                break;
            case TutorialTask025:
                break;
            case TutorialTask026:
                break;
            case TutorialTask027:
                break;
        }
    }

    /// <summary>
    /// チュートリアル始める
    /// </summary>
    public static void StartTutorial() {
        isExecution.Value = true;
    }

    /// <summary>
    /// チュートリアル止める
    /// </summary>
    public static void StopTutorial() {
        isExecution.Value = false;
    }

    /// <summary>
    /// 実行されているかを取得
    /// </summary>
    /// <returns></returns>
    public bool GetExecution() {
        return isExecution.Value;
    }
}
