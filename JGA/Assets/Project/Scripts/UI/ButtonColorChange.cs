//=============================================================================
// @File	: [ButtonColorChange.cs]
// @Brief	: ３Dボタン色変更用
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/19	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    SpriteRenderer sr;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressColor;
    Selectable button;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        if(!sr)sr = GetComponent<SpriteRenderer>();
        button = GetComponent<Selectable>();
    }
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		if (!button.IsInteractable())
        {
            sr.color = Color.gray;
        }
	}

#if false
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
		
	}
#endif

    public void ChangeColorNormal()
    {
        if (!sr) return;
        if (!button.IsInteractable()) return;
        sr.color = normalColor;
    }
    public void ChangeColorPress()
    {
        if (!sr) return;
        if (!button.IsInteractable()) return;
        sr.color = pressColor;
    }
}
