//=============================================================================
// @File	: [TimerSliderUI.cs]
// @Brief	: スライダー型のタイマー
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/07	スクリプト作成
// 2023/05/11	最初と最後の丸消した
// 2023/05/26	Hurryの生成を変更
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
    [SerializeField,Range(1,60),Header("残り何秒で音を鳴らすか")] private int soundSeconds = 10;
    [SerializeField] private AudioSource audioSource;
    //HURRY！！
    [SerializeField] private GameObject hurryUI;

    public struct TimerPointObject
    {
        public float percent;
    }
    private TimerPointObject[] timerPoints;
    [SerializeField] private Sprite timerPointYellow;
    private int nCurrentPoint = 0;

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
        nCurrentPoint = 0;

        float width = gameObject.GetComponent<RectTransform>().rect.width;
        //イベントの数を取得
        int nEvent = GameData.events.Length;
        timerPoints = new TimerPointObject[nEvent];
        //イベントの数＋最初と最後の丸を生成
        for (int i = 0; i < nEvent; ++i)
        {
            timerPoints[i].percent = GameData.events[i].percent / 100.0f;
        }

        //スライダー更新
        timerSlider.value = GameData.timer / (playMinutes * 60.0f);
    }
    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
        if (!bStart || IsFinish()) return;
        //捕まったら時間減らす
        if (GameData.isCatchPenguin)
        {
            LossTime();
        }

        if (PauseManager.isPaused) return;
        GameData.timer += Time.deltaTime;
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
        GameData.timer += (playMinutes * 60.0f * (lossSecondsParcent / 100.0f));
        CountDown();
    }
    private void CountDown()
    {
        //スライダー更新
        timerSlider.value = GameData.timer / (playMinutes * 60.0f);

        //TimerPointの位置を経過していたら画像を変更
        for(int i = nCurrentPoint; i < timerPoints.Length; ++i)
        {
            if (timerSlider.value < timerPoints[i].percent) continue;
            nCurrentPoint = i + 1;
        }

        if (!bSound) return;
        if (GameData.timer < (playMinutes * 60.0f - soundSeconds)) return;
        
        //制限時間間近の場合は音鳴らす
        SoundManager.Play(audioSource, SoundManager.ESE.COUNTDOWN_001);
        bSound = false;

        //HURRY!!をを有効化
        hurryUI.SetActive(true);
    }
    public bool IsFinish()
    {
        return GameData.timer >= playMinutes * 60.0f;
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
