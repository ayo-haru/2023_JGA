//=============================================================================
// @File	: [EmosionUI.cs]
// @Brief	: 感情表示用
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/03/03	スクリプト作成
// 2023/03/05	(小楠)列挙型の定義を変更 現在の感情を取得する関数を追加
// 2023/03/13	(小楠)ペンギンエアリア着いた時の感情を追加
// 2023/03/21	(小楠)エフェクト入れた
// 2023/03/21	(小楠)エラー直した
// 2023/05/24	(小楠)UIを客のモーションに沿って移動するように変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEmotion
{
    NONE,               //なし
    QUESTION,           //？
    ATTENSION_LOW,      //!
    ATTENSION_MIDDLE,   //!!
    ATTENSION_HIGH,     //!!!
    HIGH_TENSION,       // \|/
    MAX_EMOSION
}

public class EmosionUI : MonoBehaviour
{
    private EEmotion currentEmotion = EEmotion.NONE;    //現在の感情
    private GameObject effect = null;
    [SerializeField] private Transform headTransform;
    [SerializeField,Range(0.1f,1.0f)] private float effectPosOffset = 0.5f;
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
        if (!effect) return;
        effect.transform.position = (headTransform) ? headTransform.position : transform.position + Vector3.up * effectPosOffset;
    }
#if false
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
	{
        
    }
#endif
    public void SetEmotion(EEmotion emotion)
    {
        if (currentEmotion == emotion) return;

        currentEmotion = emotion;
        if(effect)Destroy(effect);

        switch (currentEmotion)
        {
            case EEmotion.QUESTION:
                effect = EffectManager.Create(transform.position, 4);
                break;
            case EEmotion.ATTENSION_LOW:
                effect = EffectManager.Create(transform.position, 1);
                break;
            case EEmotion.ATTENSION_MIDDLE:
                effect = EffectManager.Create(transform.position, 2);
                break;
            case EEmotion.ATTENSION_HIGH:
                effect = EffectManager.Create(transform.position, 3);
                break;
            case EEmotion.HIGH_TENSION:
                effect = EffectManager.Create(transform.position, 5);
                break;
            default:
                effect = null;
                break;
        }
    }

    public EEmotion GetEmotion()
    {
        return currentEmotion;
    }
}
