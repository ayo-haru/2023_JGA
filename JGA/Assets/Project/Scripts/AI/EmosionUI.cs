//=============================================================================
// @File	: [EmosionUI.cs]
// @Brief	: 感情表示用
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/03	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEmotion
{
    NONE,           //なし
    ATTENSION,      //！
    ENJOY,          //!!!
    QUESTION,       //？
    MAX_EMOSION
}

public class EmosionUI : MonoBehaviour
{
    private EEmotion currentEmotion = EEmotion.NONE;    //現在の感情
    private TextMesh ui;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        ui = GetComponent<TextMesh>();
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (!ui) return;
        if (ui.text == "") return;

        //カメラの方に向ける
        Vector3 dir = Camera.main.transform.position;
        dir.y = transform.position.y;
        transform.LookAt(dir);
    }

    public void SetEmotion(EEmotion emotion)
    {
        if (!ui) return;

        currentEmotion = emotion;

        switch (currentEmotion)
        {
            case EEmotion.ATTENSION:
                ui.text = "!";
                break;
            case EEmotion.QUESTION:
                ui.text = "???";
                break;
            case EEmotion.ENJOY:
                ui.text = "!!!";
                break;
            default:
                ui.text = "";
                break;
        }
    }
}
