//=============================================================================
// @File	: [ClockUI.cs]
// @Brief	: 時計のUI
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/19	スクリプト作成
// 2023/03/22	(小楠)ポーズ時の処理を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockUI : MonoBehaviour
{
    //表示用テキスト
    [SerializeField]private TextMeshProUGUI clock;
    //時間カウント用
    private float fTimer = 0.0f;
    private int nCount = 0;
    //時間計算用
    private int hours = 0;
    private int minutes = 0;
    //実プレイ時間
    [SerializeField,Range(1,10)] private int playMinute = 5;
    //ロス時間
    [SerializeField,Range(1,100),Tooltip("実プレイ時間の何％分減らしたいか指定してください")] private int lossSecondsParcent = 10;
    //営業時間
    private int openingHours = 0;
    //開始用フラグ
    private bool bStart = false;
    //開始時間
    [SerializeField,Range(0,22)] private int startHour = 9;
    //終了時間
    [SerializeField,Range(1, 23)] private int finishHour = 17;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        //営業時間の設定
        if(startHour > finishHour)
        {
            startHour = startHour ^= finishHour;
            finishHour = startHour ^= finishHour;
            startHour = startHour ^= finishHour;
        }
        if (startHour == finishHour) ++finishHour;
        openingHours = (finishHour - startHour) * 60;
        
        bStart = false;
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
        hours = startHour;
        minutes = 0;
        clock.text = string.Format("{0:00} : {1:00}",hours,minutes);

        fTimer = 0.0f;
        nCount = 0;

    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (!bStart || IsFinish() || PauseManager.isPaused) return;

        //時間を更新
        fTimer += Time.deltaTime;

        if (fTimer < ((playMinute * 60.0f) / openingHours)) return;

        CountUpClock();
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
        fTimer += (playMinute * 60.0f * (lossSecondsParcent / 100.0f));
        if (fTimer < ((playMinute * 60.0f) / openingHours)) return;
        CountUpClock();
    }

    private void CountUpClock()
    {
        while (fTimer >= (playMinute * 60.0f) / openingHours)
        {
            fTimer -= (playMinute * 60.0f) / openingHours;
            ++nCount;
            minutes = (minutes + 1) % 60;
            if (minutes == 0) ++hours;
        }

        //終了時間を超えていた場合は補正する
        if(hours >= finishHour)
        {
            hours = finishHour;
            minutes = 0;
        }

        clock.text = string.Format((minutes % 2 == 0) ? "{0:00} : {1:00}" : "{0:00}   {1:00}", hours, minutes / 5 * 5);
    }
    /// <summary>
    /// 終了時間に達したかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsFinish()
    {
        return nCount >= openingHours;
    }
    /// <summary>
    /// カウント開始
    /// </summary>
    public void CountStart()
    {
        bStart = true;
    }
    /// <summary>
    /// カウント停止
    /// </summary>
    public void CountStop()
    {
        bStart = false;
    }
}
