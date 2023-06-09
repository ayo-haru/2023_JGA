//=============================================================================
// @File	: [PlayerManager.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/05	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	private GameObject _player;

	[Header("PlayerManager")]
	[SerializeField, Tooltip("歩行速度")]
	private float moveSpeed = 7;
	[SerializeField, Tooltip("歩行時最高速度")]
	private float _maxMoveSpeed = 5;
	[SerializeField, Tooltip("歩行速度に対する疾走速度の倍率")]
	private float runMagnification = 1.5f;

	[SerializeField, Tooltip("リスタート座標")]
	private Transform respawnZone;

	[SerializeField] private List<PlayerAction> playerActions = new List<PlayerAction>();

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
		if (!_player)
			_player = GameObject.FindGameObjectWithTag("Player");
		playerActions.AddRange(_player.GetComponents<PlayerAction>());
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		foreach (var action in playerActions)
		{
			action.FixedUpdateState();
		}
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		foreach (var action in playerActions)
		{
			action.UpdateState();
		}
	}
}
