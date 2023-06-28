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

public class PlayerAnimation : PlayerAction
{
	public enum Animenum    // いい名前思いつかなかった
	{
		ANIM_MOVE,
		ANIM_RUN,
		ANIM_APPEAL,
		ANIM_HIT,
		ANIM_CARRY,
		ANIM_DRAG,
		ANIM_RANDOM,
		ANIM_ANIMSPEED,

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
					anim.SetBool(HashHit, _bHitMotion);
				}
			}
		}
	}

	public void SetAnimation(Animenum num, bool? @bool)
	{
		if (!@bool.HasValue)
		{
			anim.SetBool(HashMove, Animenum.ANIM_MOVE == num);
			anim.SetBool(HashRun, Animenum.ANIM_RUN == num);
			anim.SetBool(HashAppeal, Animenum.ANIM_APPEAL == num);
			anim.SetBool(HashHit, Animenum.ANIM_HIT == num);
			anim.SetBool(HashCarry, Animenum.ANIM_CARRY == num);
			anim.SetBool(HashDrag, Animenum.ANIM_DRAG == num);
			anim.SetBool(HashRandom, Animenum.ANIM_RANDOM == num);
		}
		else
		{
			int hash = 0;
			switch (num)
			{
				case Animenum.ANIM_MOVE:
					hash = HashMove;
					break;
				case Animenum.ANIM_RUN:
					hash = HashRun;
					break;
				case Animenum.ANIM_APPEAL:
					hash = HashAppeal;
					break;
				case Animenum.ANIM_HIT:
					hash = HashHit;
					break;
				case Animenum.ANIM_CARRY:
					hash = HashCarry;
					break;
				case Animenum.ANIM_DRAG:
					hash = HashDrag;
					break;
				case Animenum.ANIM_RANDOM:
					hash = HashRandom;
					break;
					//case Animenum.ANIM_ANIMSPEED:
					//	break;
			}
			anim.SetBool(hash, @bool.Value);
		}
	}

}
