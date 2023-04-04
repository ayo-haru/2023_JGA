//=============================================================================
// @File	: [FadeManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Sakai Ryotaro
// @Detail	: 
// 
// [Date]
// 2023/03/08	スクリプト作成
// 2023/04/04	フェード時にポーズ画面を表示させない処理を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
	public enum eFade
	{
		Default,
		FadeIn,
		FadeOut
	}

	[Header("α値に加算減算する値")]
	[SerializeField]
	private float speed = 0.01f;

	private Image image;
	private static float alpha;

	[System.NonSerialized]
	public static eFade fadeMode;

	private void Awake()
	{
		fadeMode = eFade.Default;
		alpha = 0.0f;
		image = GetComponent<Image>();
		image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		// シーンが切り替わった瞬間フェードイン開始するイベント
		SceneManager.activeSceneChanged += StartFadeIn;
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		if (MySceneManager.GameData.isCatchPenguin)
		{   // ペンギンを捕まえたらフェードアウト開始
			StartFadeIn();
		}

		if (fadeMode != eFade.Default)
		{
			if (fadeMode == eFade.FadeOut)
			{
				if (alpha < 1.0f)
				{
					alpha += speed * 2;
				}
				else
				{
					fadeMode = eFade.FadeIn;
				}
			}
			else if (fadeMode == eFade.FadeIn)
			{
				if (alpha > 0.1f)
				{
					alpha -= speed;
				}
				else
				{
					fadeMode = eFade.Default;
					alpha = 0.0f;
					PauseManager.isPaused = false;
					PauseManager.NoMenu = false;
					PauseManager.Resume();
				}
			}
			image.color = new Color(0.0f, 0.0f, 0.0f, alpha);
		}
	}

	/// <summary>
	/// フェードアウト開始。フェードアウトが終わったらフェードインする。
	/// </summary>
	public static void StartFadeOut()
	{
		alpha = 0.0f;

		fadeMode = eFade.FadeOut;

		if (!PauseManager.isPaused)
		{
			PauseManager.isPaused = true;
			PauseManager.NoMenu = true;
			PauseManager.Pause();
		}
	}

	/// <summary>
	/// フェードイン開始。フェードインさせたいときに書く
	/// </summary>
	public static void StartFadeIn()
	{
		alpha = 1.0f;

		fadeMode = eFade.FadeIn;

		if (!PauseManager.isPaused)
		{
			PauseManager.isPaused = true;
			PauseManager.NoMenu = true;
			PauseManager.Pause();
		}
	}
	/// <summary>
	/// フェードイン開始。シーン切り替わった瞬間のイベント用関数。
	/// </summary>
	/// <param name="thisScene"></param>
	/// <param name="nextScene"></param>
	public static void StartFadeIn(Scene thisScene, Scene nextScene)
	{
		alpha = 1.0f;

		fadeMode = eFade.FadeIn;

		if (!PauseManager.isPaused)
		{
			PauseManager.isPaused = true;
			PauseManager.NoMenu = true;
			PauseManager.Pause();
		}
	}

	/// <summary>
	/// 現在のフェードのステートを取る
	/// </summary>
	/// <returns></returns>
	public static eFade GetState()
	{
		return fadeMode;
	}
}
