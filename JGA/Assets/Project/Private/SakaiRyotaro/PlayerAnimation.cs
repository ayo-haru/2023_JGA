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

using UniRx;
using UnityEngine;

public class PlayerAnimation : PlayerAction
{
	public enum Animenum    // いい名前思いつかなかった
	{
		MOVE,
		RUN,
		APPEAL,
		HIT,
		CARRY,
		DRAG,
		RANDOM,
		ANIMSPEED,

		MAX_ANIM
	}
	private Animator anim;

	// Animatorパラメータ
	private int HashMove = 0;
	private int HashRun = 0;
	private int HashAppeal = 0;
	private int HashHit = 0;
	private int HashCarry = 0;
	private int HashDrag = 0;
	private int HashRandom = 0;
	private int HashAnimSpeed = 0;

	private bool _bHitMotion = false;
	public bool bHitMotion { get { return _bHitMotion; } }

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	public override void AwakeState(PlayerManager pm)
	{
		base.AwakeState(pm);

		if (anim == null) anim = GetComponent<Animator>();

		HashMove = Animator.StringToHash("move");
		HashRun = Animator.StringToHash("run");
		HashAppeal = Animator.StringToHash("Appeal");
		HashHit = Animator.StringToHash("Hit");
		HashCarry = Animator.StringToHash("Carry");
		HashDrag = Animator.StringToHash("Drag");
		HashRandom = Animator.StringToHash("Random");
		HashAnimSpeed = Animator.StringToHash("AnimSpeed");

		PlayerInputCallback.OnAppeal.Subscribe(x => { Appeal(); }).AddTo(this.gameObject);
		PlayerInputCallback.OnHit.Subscribe(x => { Hit(); }).AddTo(this.gameObject);
		PlayerInputCallback.OnHold.Subscribe(x => { Hold(); }).AddTo(this.gameObject);
		PlayerInputCallback.OnRun.Subscribe(x => { Run(); }).AddTo(this.gameObject);
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
		// アニメーション
		if (!_bHitMotion)
		{
			//anim.SetBool(HashMove, _IsMove);
			//anim.SetBool(HashRun, _IsRun);
		}
		// はたくモーション中は他のモーションに遷移させない
		else
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

			// はたくモーション中の時
			if (stateInfo.shortNameHash == HashHit)
			{
				// 再生終了時に他モーション再生を許可
				if (stateInfo.normalizedTime >= 1.0f)
				{
					_bHitMotion = false;
					anim.SetBool(HashHit, false);
				}
			}
		}
	}

	public void SetAnimation(Animenum num, bool? @bool = null)
	{
		// もし@boolに値が与えられていない場合
		// 対象のモーションをtrueにし、その他のモーションはfalseにする
		if (!@bool.HasValue)
		{
			anim.SetBool(HashMove,		Animenum.MOVE == num);
			anim.SetBool(HashRun,		Animenum.RUN == num);
			anim.SetBool(HashAppeal,	Animenum.APPEAL == num);
			anim.SetBool(HashHit,		Animenum.HIT == num);
			anim.SetBool(HashCarry,		Animenum.CARRY == num);
			anim.SetBool(HashDrag,		Animenum.DRAG == num);
			anim.SetBool(HashRandom,	Animenum.RANDOM == num);
		}
		// もし@boolに値が与えられていた場合
		// 対象のモーションのみtrueにする
		else
		{
			int hash = 0;
			switch (num)
			{
				case Animenum.MOVE:		hash = HashMove;	break;
				case Animenum.RUN:		hash = HashRun;		break;
				case Animenum.APPEAL:	hash = HashAppeal;	break;
				case Animenum.HIT:		hash = HashHit;		break;
				case Animenum.CARRY:	hash = HashCarry;	break;
				case Animenum.DRAG:		hash = HashDrag;	break;
				case Animenum.RANDOM:	hash = HashRandom;	break;
					//case Animenum.ANIM_ANIMSPEED:			break;
			}
			anim.SetBool(hash, @bool.Value);
		}
	}

	#region コールバック

	private void Appeal()
	{
		if (PlayerInputCallback.isAppeal)
			SetAnimation(Animenum.APPEAL, true);
		else
			SetAnimation(Animenum.APPEAL, false);
	}

	private void Hit()
	{
		//--- 以下の場合は実行しない
		//・既にはたいている
		//・アピール中
		//・ポーズ中
		//・掴んでいる

		if (_bHitMotion ||
			PlayerInputCallback.isAppeal ||
			PlayerInputCallback.isHold ||
			PauseManager.isPaused )
			return;

		PlayerInputCallback.isHit = _bHitMotion = true;
		SetAnimation(Animenum.HIT, true);
	}

	private void Hold()
	{
		//if (PlayerInputCallback.isHold)
		//	SetAnimation(Animenum.CARRY, true);
		//else
		//	SetAnimation(Animenum.CARRY, false);
	}

	private void Run()
	{
		if (PlayerInputCallback.isRun)
			SetAnimation(Animenum.RUN, true);
		else
			SetAnimation(Animenum.RUN, false);
	}

	#endregion


	#region アニメーションから呼び出す処理
	public void MoveSound()
	{
		//SoundManager.Play(audioSource, SoundManager.ESE.PENGUIN_WALK_002);
	}

	public void AnimStop()
	{
		anim.SetFloat(HashAnimSpeed, 0.0f);
	}

	/// <summary>
	/// 鳴く
	/// </summary>
	public void OnCall()
	{
		// エフェクトを生成
		var obj = EffectManager.Create(transform.position + new Vector3(0, 4, 0), 0, transform.rotation);
		obj.transform.localScale = Vector3.one * 5;
		obj.transform.parent = transform;
	}

	public void AnimCarryStop()
	{
		anim.SetBool(HashCarry, false);
	}

	public void AnimHold()
	{
		//if (InteractCollision == null || !InteractCollision.CompareTag("Interact"))
		//	return;

		//_IsHoldMotion = false;

		//if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		//{
		//	switch (baseObj.objType)
		//	{
		//		case BaseObj.ObjType.HOLD:
		//		case BaseObj.ObjType.HIT_HOLD:
		//			Hold(true);
		//			_IsHold = true;
		//			break;
		//		case BaseObj.ObjType.DRAG:
		//		case BaseObj.ObjType.HIT_DRAG:
		//			Drag(true);
		//			_IsHold = _IsDrag = true;
		//			break;
		//	}
		//}
	}

	public void AnimHit()
	{
		//// 範囲内に何もなければ、何もしない
		//if (WithinRange.Count == 0)
		//	return;

		//DecideNearestObject();

		//if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		//{
		//	if (baseObj.objType == BaseObj.ObjType.HIT ||
		//		baseObj.objType == BaseObj.ObjType.HIT_HOLD ||
		//		baseObj.objType == BaseObj.ObjType.HIT_DRAG)
		//	{
		//		Hit();
		//	}
		//}
	}
	#endregion

}
