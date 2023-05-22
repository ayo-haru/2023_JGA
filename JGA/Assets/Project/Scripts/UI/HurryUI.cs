//=============================================================================
// @File	: [HurryUI.cs]
// @Brief	: 
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HurryUI : MonoBehaviour
{
    //背景画像
    [SerializeField] private GameObject backGround;
    private RawImage riBackGround;
    //文字画像
    [SerializeField] private GameObject hurry;
    private RectTransform rtHurry;

    //文字移動速度
    [SerializeField, Range(1.0f, 100.0f)] private float moveSpeed = 1.0f;
    //文字中央待機時間
    [SerializeField, Min(1)] private int waitTime = 1;
    private float fWaitTimer = 0;

    //処理ステップ
    private enum EStep {STEP_FADEIN,STEP_WAIT,STEP_FADEOUT,MAX_STEP};
    private EStep step = EStep.STEP_FADEIN;

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
        //コンポーネント取得
        if (backGround　&& !riBackGround) riBackGround = backGround.GetComponent<RawImage>();
        if (hurry && !rtHurry)rtHurry = hurry.GetComponent<RectTransform>();

        if (!rtHurry) return;

        step = EStep.STEP_FADEIN;
        //初期位置設定
        Vector3 pos = rtHurry.localPosition;
        pos.x = rtHurry.rect.width * 2;
        rtHurry.localPosition = pos;
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if(!riBackGround || !rtHurry)
        {
            Debug.LogError("コンポーネントが取得できていません");
            return;
        }
#if false
        //テクスチャスクロール
        var uv_rect = riBackGround.uvRect;
        uv_rect.x = Mathf.Repeat(Time.time * scrollSpeed, 1.0f);
        riBackGround.uvRect = uv_rect;
#endif
        switch (step)
        {
            case EStep.STEP_FADEIN:
                {
                    Vector3 pos = rtHurry.localPosition;
                    pos.x -= moveSpeed;
                    if(pos.x < 0.0f)
                    {
                        pos.x = 0.0f;
                        fWaitTimer = 0.0f;
                        step = EStep.STEP_WAIT;
                    }
                    rtHurry.localPosition = pos;
                }
                break;
            case EStep.STEP_WAIT:
                fWaitTimer += Time.deltaTime;
                if (fWaitTimer >= waitTime)
                {
                    step = EStep.STEP_FADEOUT;
                }
                break;
            case EStep.STEP_FADEOUT:
                {
                    Vector3 pos = rtHurry.localPosition;
                    pos.x -= moveSpeed;
                    if (pos.x < -(rtHurry.rect.width * 2))
                    {
                        step = EStep.MAX_STEP;
                    }
                    rtHurry.localPosition = pos;
                }
                break;
            case EStep.MAX_STEP:
                Destroy(gameObject);
                break;
        }

    }
#if false
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
		
	}
#endif
}
