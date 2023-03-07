//=============================================================================
// @File	: [FadeInOut.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
	enum eFade {
		Default,
		FadeIn,
		FadeOut
    }

	[Header("α値に加算減算する値")]
	[SerializeField]
	private float speed = 0.01f;

	private Image image;
	private float alpha;

	private eFade fadeMode;

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		fadeMode = eFade.Default;
		alpha = 0.0f;
		image = GetComponent<Image>();
		image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (MySceneManager.GameData.isCatchPenguin) {
			fadeMode = eFade.FadeOut;
		}

        if (fadeMode != eFade.Default) {
			if (fadeMode == eFade.FadeOut) {
				if (alpha < 1.0f) {
					alpha += speed*2;
				} else {
					fadeMode = eFade.FadeIn;
				}
			} else if (fadeMode == eFade.FadeIn) {
				if (alpha > 0.0f) {
					alpha -= speed;
				} else {
					fadeMode = eFade.Default;
				}
			}
			image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
	}
}
