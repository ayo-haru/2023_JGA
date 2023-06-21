//=============================================================================
// @File	: [PlayerMove.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/05	スクリプト作成
//=============================================================================
using UnityEngine;

public class PlayerMove : PlayerAction
{
	[SerializeField] private Vector3 _vForce;               // 移動方向
	public Vector3 vForce { get { return _vForce; } }
	private Rigidbody rb;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	public override void AwakeState(PlayerManager pm)
	{
		base.AwakeState(pm);

		rb = _playerManager._playerRb;
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	public override void StartState()
	{

	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	public override void FixedUpdateState()
	{
		if (_playerManager.MoveInputValue != Vector2.zero)
		{
			Move();
		}
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	public override void UpdateState()
	{

	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
	{
		//// ゲームパッド接続時、傾きがjoyRunZone以上の時「走り」の判定にする
		//if (bGamePad)
		//	_IsRun = moveInputValue.magnitude >= joyRunZone;

		//--- 速度、制限速度を定義
		float force, max;
		//// アピール中の場合
		//if (_IsAppeal)
		//{
		//	force = appealForce;
		//	max = _maxAppealSpeed;
		//}
		//// 走り中の場合
		//else if (_IsRun)
		//{
		//	force = runForce;
		//	max = _maxRunSpeed;
		//}
		//// 歩いている場合
		//else
		{
			force = _playerManager.MoveSpeed;
			max = _playerManager.MaxMoveSpeed;
		}

		// 制限速度内の場合、移動方向の力を与える
		_vForce = new Vector3(_playerManager.MoveInputValue.x, 0, _playerManager.MoveInputValue.y) * force;
		Debug.Log($"_vForce:{_vForce}");

		////引きずっているとき
		//if (_IsDrag)
		//{
		//	//移動
		//	if (Vector3.Dot(vForce.normalized, transform.forward.normalized) <= -0.5f)
		//	{
		//		if (rb.velocity.magnitude < max && _vForce != Vector3.zero) rb.AddForce(_vForce);
		//	}
		//	//回転
		//	if (moveInputValue.normalized == Vector2.zero) return;
		//	float dragMoveAngle = (Vector3.SignedAngle(vForce.normalized, transform.forward.normalized, Vector3.up) <= 0.0f) ? -1.0f : 1.0f;
		//	transform.rotation *= Quaternion.AngleAxis(dragMoveAngle, Vector3.up);
		//}

		////引きずっていないとき
		//else
		{
			//移動
			if (rb.velocity.magnitude < max && _vForce != Vector3.zero)
				rb.AddForce(_vForce);
			//回転
			if (_playerManager.MoveInputValue.normalized == Vector2.zero)
				return;
			var fw = transform.forward - new Vector3(
				-_playerManager.MoveInputValue.x,
				transform.position.y,
				-_playerManager.MoveInputValue.y
				) / 2;
			fw.y = 0.0f;
			transform.LookAt(transform.position + fw);
		}
	}

}
