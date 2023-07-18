//=============================================================================
// @File	: [PlayerInteract.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/07/14	スクリプト作成
//=============================================================================

using UniRx;
using UnityEngine.InputSystem;

public class PlayerInteract : PlayerAction
{

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	public override void AwakeState(PlayerManager pm)
	{
		base.AwakeState(pm);

		PlayerInputCallback.OnHold.Subscribe(x => { }).AddTo(this.gameObject);
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
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	public override void UpdateState()
	{

	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		//bHitMotion = true;
		//_IsHitMotion = true;
		//if (HashHit == 0)
		//	HashHit = Animator.StringToHash("Hit");
		//anim.SetBool(HashHit, bHitMotion);
	}

	/// <summary>
	/// 咥える
	/// </summary>
	private void OnHold(InputAction.CallbackContext context)
	{
		//// ポーズ中は実行しない
		//if (PauseManager.isPaused)
		//	return;

		////--- 長押し開始（つかむ処理）
		//if (context.phase == InputActionPhase.Performed)
		//{
		//	// 範囲内のオブジェクトが無い場合は実行しない
		//	if (WithinRange.Count == 0)
		//		return;

		//	DecideNearestObject();

		//	// オブジェクトの種類によって持つアニメーションを変える
		//	if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		//	{
		//		switch (baseObj.objType)
		//		{
		//			case BaseObj.ObjType.HOLD:
		//			case BaseObj.ObjType.HIT_HOLD:
		//				anim.SetBool(HashCarry, !_IsHold);
		//				break;
		//			case BaseObj.ObjType.DRAG:
		//			case BaseObj.ObjType.HIT_DRAG:
		//				anim.SetBool(HashDrag, !_IsHold);
		//				break;
		//		}
		//		_IsHoldMotion = true;
		//	}
		//	// 移動中判定
		//	moveInputValue = Vector2.zero;
		//	_IsMove = false;
		//}

		////--- 長押し終了（離す処理）
		//else if (context.phase == InputActionPhase.Canceled)
		//{
		//	if (InteractCollision == null)
		//		return;

		//	if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		//	{
		//		switch (baseObj.objType)
		//		{
		//			case BaseObj.ObjType.HOLD:
		//			case BaseObj.ObjType.HIT_HOLD:
		//				Hold(false);
		//				_IsHold = false;
		//				break;
		//			case BaseObj.ObjType.DRAG:
		//			case BaseObj.ObjType.HIT_DRAG:
		//				Drag(false);
		//				_IsHold = _IsDrag = false;
		//				break;
		//		}
		//	}
		//	_IsHoldMotion = false;
		//}

	}

}
