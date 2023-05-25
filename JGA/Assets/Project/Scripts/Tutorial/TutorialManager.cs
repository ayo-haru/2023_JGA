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

public class TutorialManager : MonoBehaviour
{
    enum TutorialState {
        GO_TRASHBOX = 0,
        STHICK,
        PRESS_A,
        GUEST_CAN,
        GUEST_APPEAL,
        PRESS_B,
        FOLLOW_GUEST,
        GUEST_TAKE_EFFECT,
        GUEST_ARRIVAL,
        CORN,
        PRESS_RT,
        HOLD_OBJ,
        TIMER
    }

    //---キャンバス
    GameObject canvas;
    RectTransform canvasRT;

    //---フェードのパネル
    GameObject fade;

    //---プレイヤー
    GameObject player;

    //---チュートリアル用客
    GameObject guest;

    private List<ITurorial> tutorialTask;
    private ITurorial currentTask;

	private static bool isExecution;   // チュートリアルをやるか
    private bool isTaskFin;     // タスクが終了しているか

    [SerializeField]
	private List<GameObject> tutorialText;
    private GameObject currentTextObj;
    private Image currentText;

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        canvas = GameObject.Find("Canvas");
        canvasRT = canvas.GetComponent<RectTransform>();

        fade = GameObject.Find("FadePanel");

        player = GameObject.FindWithTag("Player");
        guest = GameObject.Find("TutorialWait_Guest001");

        // 変数初期化
        isExecution = false;
        isTaskFin = false;

        // チュートリアルの一覧
        tutorialTask = new List<ITurorial>()
        {
            new Move01Tutorial(),
            new Move02Tutorial(),
            new Hit01Tutorial(),
            new Hit02Tutorial(),
            new Appeal01Tutorial(),
            new Appeal02Tutorial(),
            new Follow01Tutorial(),
            new Follow02Tutorial(),
            new Follow03Tutorial(),
            new Hold01Tutorial(),
            new Hold02Tutorial(),
            new Hold03Tutorial(),
            new Timer01Tutorial()
        };

        // 最初のチュートリアルを設定
        SetTaskObj(tutorialTask.First());
        StartCoroutine(SetCurrentTask(tutorialTask.First()));

        //currentTextObj = tutorialText.First();
        //Instantiate(currentTextObj, canvasRT);
        //currentTextObj.transform.SetSiblingIndex(fade.transform.GetSiblingIndex()); // フェードの裏側に来るようにする
        //currentText = currentTextObj.GetComponent<Image>();
	}


	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        //----- チュートリアルを実行するか -----
        if (!isExecution) {
            return;
        }

        // チュートリアルが存在しタスクが実行されていない場合に処理
        if (currentTask != null && !isTaskFin) {
            // 現在のチュートリアルが実行されたか判定
            if (currentTask.CheckTask()) {
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
            case Move01Tutorial:
                break;
            case Move02Tutorial:
                task.SetTaskObj(player);
                break;
            case Hit01Tutorial:
                currentTask.SetTaskObj(player);
                break;
            case Hit02Tutorial:
                break;
            case Appeal01Tutorial:
                break;
            case Appeal02Tutorial:
                currentTask.SetTaskObj(player);
                break;
            case Follow01Tutorial:
                currentTask.SetTaskObj(guest);
                break;
            case Follow02Tutorial:
                break;
            case Follow03Tutorial:
                break;
            case Hold01Tutorial:
                currentTask.SetTaskObj(player);
                break;
            case Hold02Tutorial:
                currentTask.SetTaskObj(player);
                break;
            case Hold03Tutorial:
                break;
            case Timer01Tutorial:
                break;
        }
    }

    /// <summary>
    /// チュートリアル始める
    /// </summary>
    public static void StartTutorial() {
        isExecution = true;
    }

    /// <summary>
    /// チュートリアル止める
    /// </summary>
    public static void StopTutorial() {
        isExecution = false;
    }
}
