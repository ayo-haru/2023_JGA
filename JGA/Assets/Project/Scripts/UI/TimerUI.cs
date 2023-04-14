//=============================================================================
// @File	: [TimerUI.cs]
// @Brief	: 円形の時計UI
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/02	スクリプト作成
// 2023/04/07	残り時間少なくなった時にHURRY!!を表示
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

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
#if false
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
		
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		
	}
#endif
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
        fillImage.fillAmount = fTimer / (playMinutes * 60.0f);
        if (!bSound) return;
        if (fTimer < (playMinutes * 60.0f - soundSeconds)) return;
        
        SoundManager.Play(audioSource, SoundManager.ESE.COUNTDOWN_001);
        //HURRY!!を生成
        GameObject ui = Instantiate(hurryUI);
        ui.transform.SetParent(gameObject.transform.parent);
        RectTransform rt = gameObject.GetComponent<RectTransform>();
        ui.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, rt.localPosition.y - hurryUIPosY, 0.0f);
        bSound = false;
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
}
