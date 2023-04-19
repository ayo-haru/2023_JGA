//=============================================================================
// @File	: [DebugClick.cs]
// @Brief	: 
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
public class DebugClick : MonoBehaviour
{
    Selectable selectable;
    SpriteRenderer sr;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        Image image = gameObject.GetComponent<Image>();
        image.sprite = sr.sprite;
        selectable.targetGraphic = image;
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        sr = GetComponent<SpriteRenderer>();
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
		
	}

    public void Click()
    {
        Debug.Log("あああああ");
        sr.color = Color.red;
    }
    public void EndClick()
    {
        sr.color = Color.white;
    }
}
