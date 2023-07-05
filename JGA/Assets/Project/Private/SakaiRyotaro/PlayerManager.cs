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
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	private GameObject _player;
	public Rigidbody _playerRb;

	[Header("PlayerManager")]
	[SerializeField, Tooltip("歩行速度")]
	private float _moveSpeed = 25;
	[SerializeField, Tooltip("歩行時最高速度")]
	private float _maxMoveSpeed = 5;
	[SerializeField, Tooltip("歩行速度に対する疾走速度の倍率")]
	private float _runMagnification = 1.5f;
	public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
	public float MaxMoveSpeed { get { return _maxMoveSpeed; } set { _maxMoveSpeed = value; } }
	public float RunMagnification { get { return _runMagnification; } set { _runMagnification = value; } }

	[SerializeField, Tooltip("リスタート座標")]
	private Transform respawnZone;

	[SerializeField] private List<PlayerAction> playerActions = new List<PlayerAction>();

	private Vector2 _moveInputValue;                         // 移動方向
	public Vector2 MoveInputValue { get { return _moveInputValue; } set { _moveInputValue = value; } }


	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		if (!_player)
			_player = GameObject.FindGameObjectWithTag("Player");
		_playerRb = _player.GetComponent<Rigidbody>();
		playerActions.AddRange(_player.GetComponents<PlayerAction>());

		foreach (var action in playerActions)
		{
			action.AwakeState(this);
		}
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		foreach (var action in playerActions)
		{
			action.StartState();
		}
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
