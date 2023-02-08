//=============================================================================
// @File	: [FPSDisplay.cs]
// @Brief	: 
// @Author	: YoshiharaAsuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    //---- 変数宣言 ----
    [SerializeField] private float updateInterval = 0.02f;		// 計測時間間隔

	private float acuum;
	private float frames;
	private float timeLeft;
	private float FPS;

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		timeLeft -= Time.deltaTime;
		acuum += Time.timeScale / Time.deltaTime;
		frames++;

		if(0 < timeLeft){
			return;
        }

		FPS = acuum / frames;
		timeLeft = updateInterval;
		acuum = 0;
		frames = 0;
	}

    private void OnGUI()
    {
        GUILayout.Label("FPS:"+FPS.ToString("f2"));
    }
}
