//=============================================================================
// @File	: [TestFade.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/09	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFade : MonoBehaviour
{
	private GameObject fadePanel = null;
	private Image fadeImagePanel = null;

	[SerializeField]
	private float alpha = 0.0f;

	private bool fade = false;



	private void Awake()
	{
		fade = true;
	}


	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		fadePanel = GameObject.Find("FadePanel");
		fadeImagePanel = fadePanel.GetComponent<Image>();
		alpha = fadeImagePanel.color.a;
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		if (fade == true) Fade();
	}

	private void Fade()
	{
		FadeOut();

		fade = false;
	}

	private void FadeOut()
	{
		alpha += 0.01f;
		fadeImagePanel.color = new Color(0, 0, 0, alpha);
		if(alpha < 0.0f){
			return;
		}
	}

	private void FadeIn()
	{
		alpha -= 0.01f;
		fadeImagePanel.color = new Color(0, 0, 0, alpha);
		if(alpha > 1.0f){
			return;
		}
	}
}
