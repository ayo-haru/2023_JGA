//=============================================================================
// @File	: [UIManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	GameObject canvas;
	GameObject fadePanel;



	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        canvas = GameObject.Find("Canvas");

       // GameObject _fadePanel = PrefabContainerFinder.Find(MySceneManager.GameData.UIDatas, "FadePanel.prefab");
       // fadePanel = Instantiate(_fadePanel);

       //fadePanel.transform.parent =  canvas.transform;
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
}
