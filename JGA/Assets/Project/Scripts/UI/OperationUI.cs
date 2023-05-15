//=============================================================================
// @File	: [OperationUI.cs]
// @Brief	: 操作方法のUI
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationUI : MonoBehaviour
{
	private GameObject player;
	private Player _Player;

	private bool isPlayerMove;

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");

		if (player) {
            _Player = player.GetComponent<Player>();
            isPlayerMove = _Player.IsMove;

        } else {
			Debug.LogError("プレイヤーが見つかりませんでした");
		}
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		// 移動中か取得
        isPlayerMove = _Player.IsMove;


    }
}
