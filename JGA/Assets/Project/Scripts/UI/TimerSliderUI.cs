//=============================================================================
// @File	: [TimerSliderUI.cs]
// @Brief	: スライダー型のタイマー
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerSliderUI : MonoBehaviour
{
    [SerializeField] private Slider timerSlider;

    //プレイ時間
    [SerializeField, Range(1, 10)] private int playMinutes;
    //ロス時間
    [SerializeField, Range(1, 100), Tooltip("実プレイ時間の何％分減らしたいか指定してください")] private int lossSecondsParcent = 10;
    //開始用フラグ
    private bool bStart = false;
    //SE用フラグ
    private bool bSound = true;
    [SerializeField,Range(1,60)] private int soundSeconds = 10;
    //時間計測用
    private float fTimer = 0.0f;

    [SerializeField] private AudioSource audioSource;

    //HURRY！！
    [SerializeField] private GameObject hurryUI;
    [SerializeField] private float hurryUIPosY = 200.0f;

    //TimerPoint
    [SerializeField] private GameObject timerPointPrefab;       //プレハブ
    [SerializeField] private Transform timerPointsPearent;      //親オブジェクト
    public struct TimerPointObject
    {
        public GameObject timerPoint;
        public float percent;
    }
    private TimerPointObject[] timerPoints;
    [SerializeField] private Sprite timerPointYellow;
    private int nCurrentPoint = -1;

    //残り秒数表示用のテキスト
    [SerializeField] private TextMeshProUGUI text;
#if false
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        
	}
#endif
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		text.text = text.text = string.Format("{0:0}", (int)(playMinutes * 60.0f));
        nCurrentPoint = -1;

        float width = gameObject.GetComponent<RectTransform>().rect.width;
        //イベントの数を取得
        int nEvent = MySceneManager.GameData.events.Length + 2;
        timerPoints = new TimerPointObject[nEvent];
        //イベントの数＋最初と最後の丸を生成
        for (int i = 0; i < nEvent; ++i)
        {
            timerPoints[i].timerPoint = Instantiate(timerPointPrefab);
            timerPoints[i].percent = (i == 0) ? 0.0f : (i == nEvent - 1) ? 1.0f : MySceneManager.GameData.events[i - 1].percent / 100.0f;
            timerPoints[i].timerPoint.transform.parent = timerPointsPearent;
            timerPoints[i].timerPoint.transform.localPosition = new Vector3(timerPoints[i].percent * width + -width / 2, 0.0f, 0.0f);
        }
    }
	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (!bStart || IsFinish() || PauseManager.isPaused) return;

        fTimer += Time.deltaTime;
        CountDown();
	}
#if false
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{

	}
#endif
    public void LossTime()
    {
        fTimer += (playMinutes * 60.0f * (lossSecondsParcent / 100.0f));
        CountDown();
    }
    private void CountDown()
    {
        //スライダー更新
        timerSlider.value = fTimer / (playMinutes * 60.0f);

        //TimerPointの位置を経過していたら画像を変更
        for(int i = nCurrentPoint + 1; i < timerPoints.Length; ++i)
        {
            if (timerSlider.value < timerPoints[i].percent) continue;
            if (timerPoints[i].timerPoint.TryGetComponent(out Image image)) image.sprite = timerPointYellow;
            if(i > 0 && i < timerPoints.Length - 1)SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
            nCurrentPoint = i;
        }

        //残り秒数の表示更新
        text.text = string.Format("{0:0}", (int)(playMinutes * 60.0f - fTimer));

        if (!bSound) return;
        if (fTimer < (playMinutes * 60.0f - soundSeconds)) return;
        
        //制限時間間近の場合は音鳴らす
        SoundManager.Play(audioSource, SoundManager.ESE.COUNTDOWN_001);
        bSound = false;

        //HURRY!!を生成
        GameObject ui = Instantiate(hurryUI);
        ui.transform.SetParent(gameObject.transform.parent);
        ui.transform.localPosition = new Vector3(0.0f, transform.localPosition.y - hurryUIPosY, 0.0f);
    }
    public bool IsFinish()
    {
        return fTimer >= playMinutes * 60.0f;
    }
    public void CountStart()
    {
        bStart = true;
    }
    public void CountStop()
    {
        bStart = false;
    }

    public int CurrentTimerPoint()
    {
        return nCurrentPoint;
    }
}
