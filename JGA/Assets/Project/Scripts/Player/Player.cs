//=============================================================================
// @File	: [Player.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: Ichida Mai    Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/02/25	スクリプト作成
// 2023/03/08	リスポーンするよ
// 2023/04/04	インタラクトオブジェクトの所を手直ししました～(吉原)
//				OnHit(コールバック関数)とOnHit(メインの処理)で名前がまったく同じだからメインの処理はHitにさせていただきましたわ！！！
// 2023/04/28   引きずる処理追加しました(小楠)
// 2023/05/04   引きずり終了する時の処理を少し変更しました(小楠)
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
	[SerializeField] private Rigidbody		rb;
	[SerializeField] private AudioSource	audioSource;
	[SerializeField] private AudioClip		seCall;			// ＳＥ：鳴き声
	[SerializeField] private AudioClip		seWalk;			// ＳＥ：歩く
	[SerializeField] private Animator		anim;			// Animatorへの参照

	[Header("ステータス")] //-----------------------------------------------------------------
	[SerializeField] private float moveForce = 7;           // 歩行時速度
	[SerializeField] private float _maxMoveSpeed = 5;       // 歩行時最高速度
	[SerializeField] private float runMagnification = 1.5f; // 疾走速度倍率
	private float runForce;                                 // 疾走時速度
	private float _maxRunSpeed;                             // 疾走時最高速度
	private float appealForce;                              // アピール時速度
	private float _maxAppealSpeed;                          // アピール時最高速度

	public float MaxMoveSpeed	{ get { return _maxMoveSpeed; } }
	public float MaxRunSpeed	{ get { return _maxRunSpeed; } }
	public float MaxAppealSpeed { get { return _maxAppealSpeed; } }

	[SerializeField] private float joyRunZone = 0.8f;   // ジョイスティックで走り始めるゾーン
	//----------------------------------------------------------------------------------------

	// フラグ --------------------------------------------------------------------------------
	private bool bRunButton;                    // [PC]シフトキー入力状態
	[SerializeField] private bool bGamePad;     // ゲームパッド接続確認フラグ

	[SerializeField] private bool _IsHit;       // はたきフラグ
	[SerializeField] private bool _IsHitMotion; // はたき開始フラグ
	[SerializeField] private bool _IsHold;      // つかみフラグ
	[SerializeField] private bool _IsDrag;      // 引きずりフラグ
	[SerializeField] private bool _IsMove;      // 移動フラグ
	[SerializeField] private bool _IsRun;       // 走りフラグ
	[SerializeField] private bool _IsAppeal;    // アピールフラグ
	[SerializeField] private bool _IsRandom;    // 待機中のランダムな挙動
	[SerializeField] private bool _IsMegaphone; // メガホン用フラグ

	public bool IsHit		{ get { return _IsHit; } set { _IsHit = value; } }
	public bool IsHitMotion	{ get { return _IsHitMotion; } }
	public bool IsHold		{ get { return _IsHold; } }
	public bool IsDrag		{ get { return _IsDrag; } }
	public bool IsMove		{ get { return _IsMove; } }
	public bool IsRun		{ get { return _IsRun; } }
	public bool IsAppeal	{ get { return _IsAppeal; } }
	public bool IsRandom	{ get { return _IsRandom; } }
	public bool IsMegaphone	{ get { return _IsMegaphone; } }

	private bool bHitMotion;                    // はたくモーション中は他のモーションさせないフラグ

	private bool DelayHit;
	private bool DelayHitMotion;
	private bool DelayMegaphone;
	//----------------------------------------------------------------------------------------

	// 移動 ----------------------------------------------------------------------------------
	[SerializeField] private Vector3 _vForce;               // 移動方向
	public Vector3 vForce { get { return _vForce; } }

	[SerializeField] private Collider	InteractCollision;		// 掴んでいるオブジェクト：コリジョン
	[SerializeField] private Outline	InteractOutline;		// 掴んでいるオブジェクト：アウトライン
	[SerializeField] private HingeJoint	InteractJoint;			// 掴んでいるオブジェクト：HingeJoint
	[SerializeField] private Transform	InteractPoint;			// 掴んでいるオブジェクト：HoldPoint
	[SerializeField] private float		InteractBoundsSizeY;     // 掴んでいるオブジェクト：BoundsSize.y

	[SerializeField] private HashSet<Collider> WithinRange = new HashSet<Collider>();  // インタラクト範囲内にあるオブジェクトリスト

	private MyContorller	gameInputs;						// 方向キー入力取得
	private Vector2			moveInputValue;					// 移動方向
	private Vector3			pauseVelocity;					// ポーズ時の加速度保存
	private Vector3			pauseAngularVelocity;			// ポーズ時の加速度保存
	private Transform		respawnZone;					// リスポーン位置プレハブ設定用
	//----------------------------------------------------------------------------------------

	[SerializeField] private GameObject holdPos;    // 持つときの位置

	private GameObject InteractObjectParent;        // シーン上の「InteractObject」

	private float IdolTime = 0;                     // 待機中のランダムな挙動用カウンタ

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		//--- ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		//--- NULLリファレンス回避
		if (rb == null) rb = GetComponent<Rigidbody>();
		if (audioSource == null) audioSource = GetComponent<AudioSource>();
		if (anim == null) anim = GetComponent<Animator>();

		//--- 回転固定
		rb.constraints = RigidbodyConstraints.FreezeRotation;

		//--- 持つときの場所を子オブジェクトから検索
		if (holdPos == null)
			holdPos = transform.Find("HoldPos").gameObject;

		//--- プレイヤー初期位置
		var respawn = GameObject.Find("PlayerSpawn");
		if (respawn == null)
			Debug.LogError("<color=red>プレイヤーのリスポーン位置が見つかりません[Player.cs]</color>");
		else
			respawnZone = respawn.transform;

		//--- 物を離した際に元の階層に戻すためのやつ
		InteractObjectParent = GameObject.Find("InteractObject");


		// Input Actionインスタンス生成
		gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Player.Move.performed += OnMove;
		gameInputs.Player.Move.canceled += OnMove;
		gameInputs.Player.Appeal.performed += OnAppeal;
		gameInputs.Player.Appeal.canceled += OnAppeal;
		gameInputs.Player.Hit.performed += OnHit;
		gameInputs.Player.Hold.performed += OnHold;
		gameInputs.Player.Hold.canceled += OnHold;
		gameInputs.Player.Run.performed += OnRun;
		gameInputs.Player.Run.canceled += OnRun;

		// Input Actionを有効化
		gameInputs.Enable();

		// アピール速度設定
		runForce = moveForce * runMagnification;
		_maxRunSpeed = _maxMoveSpeed * runMagnification;
		appealForce = (moveForce + runForce) / 2;
		_maxAppealSpeed = (_maxMoveSpeed + _maxRunSpeed) / 2;
	}

	private void OnDisable()
	{
		// inout actionを無効化
		gameInputs.Disable();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	private void FixedUpdate()
	{
		// はたき中でないとき移動
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
			Move();

		if (InteractJoint != null && InteractPoint != null)
		{
			if (_IsHold)
				InteractJoint.anchor = new Vector3(0, InteractPoint.localPosition.y / InteractBoundsSizeY, 0);

			//if (_IsDrag)
			//	InteractJoint.anchor = InteractJoint.transform.InverseTransformPoint(transform.TransformPoint(holdPos.transform.localPosition));//HoldPosを設定
		}
	}

	private void Update()
	{
		// ポーズ中は移動しない
		if (PauseManager.isPaused)
			moveInputValue = Vector2.zero;
		// ポーズでない時にisKinematicをfalseにする
		else if (rb.isKinematic == true)
			rb.isKinematic = false;

		// ゲームパッドが接続されているか
		bGamePad = Gamepad.current != null;

		// リスタート
		if (MySceneManager.GameData.isCatchPenguin)
		{
			ReStart();
			return;
		}

		// インタラクトして１フレーム経過後
		if (_IsHit)
		{
			// インタラクトして１フレーム経過後
			if (!DelayHit)
			{
				DelayHit = true;
			}
			// インタラクトして２フレーム経過後
			else
			{
				DelayHit = false;
				_IsHit = false;
			}
		}

		// インタラクトして１フレーム経過後
		if (_IsHitMotion)
		{
			// インタラクトして１フレーム経過後
			if (!DelayHitMotion)
			{
				DelayHitMotion = true;
			}
			// インタラクトして２フレーム経過後
			else
			{
				DelayHitMotion = false;
				_IsHitMotion = false;
			}
		}

		// インタラクトしたオブジェクトがメガホンの場合
		if (_IsMegaphone)
		{
			// １フレーム経過後
			if (!DelayMegaphone)
			{
				DelayMegaphone = true;
			}
			// ２フレーム経過後
			else
			{
				_IsMegaphone = false;
				DelayMegaphone = false;
			}
		}

		// アニメーション
		if (!bHitMotion)
		{
			anim.SetBool("move", _IsMove);
			anim.SetBool("run", _IsRun);
		}
		// はたくモーション中は他のモーションさせない
		else
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			if (stateInfo.IsName("Hit"))
			{
				// 再生中か？
				if (stateInfo.normalizedTime < 1.0f)
				{
					//Debug.Log($"Hit再生中");
				}
				else
				{
					bHitMotion = false;
					anim.SetBool("Hit", bHitMotion);
				}

			}
		}

		// 走っているか判定
		_IsRun = bRunButton && moveInputValue.normalized != Vector2.zero;

		float length;       // プレイヤーと一番近いオブジェクトの距離
		if (InteractOutline != null)
		{
			length = Vector3.Distance(transform.position, InteractOutline.transform.position);
		}
		else
		{
			length = 10.0f; // とりあえず10.0f
		}

		// 移動中判定
		_IsMove = moveInputValue.normalized != Vector2.zero;

		// 待機中のランダムな挙動のための処理
		if (!_IsMove && !_IsAppeal && !_IsRandom)
		{
			IdolTime += Time.deltaTime;

			// もし２秒経過していたら50％の確立で再生
			if (IdolTime > 2.0f && Random.Range(0, 2) == 1) // ※ 0～1の範囲でランダムな整数値が返る
			{
				IdolTime = 0.0f;
				_IsRandom = true;
				anim.SetBool("Random", true);
			}
		}

		if (_IsRandom)
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			// ランダムな挙動の再生が終了した時
			if (stateInfo.normalizedTime >= 1.0f)
			{
				_IsRandom = false;
				anim.SetBool("Random", false);
			}
		}

		// 範囲内にオブジェクトがない && 一番近くのオブジェクト情報を保持している　の場合
		if (WithinRange.Count == 0 && InteractOutline != null)
		{
			// アウトラインを非表示にする
			InteractOutline.enabled = false;
			InteractOutline = null;
		}

		// 範囲内にオブジェクトがある || 一番近くのオブジェクト情報を保持していない　の場合
		else
		{
			// プレイヤーに一番近いオブジェクトをインタラクト対象とする
			foreach (Collider obj in WithinRange)
			{
				// 一番近くのオブジェクト情報を保持していた場合
				if (InteractOutline != null && obj == InteractOutline.gameObject)
				{
					// アウトラインを有効化
					InteractOutline.enabled = true;
					continue;
				}

				// オブジェクトとの距離を計算
				float distance = Vector3.Distance(transform.position, obj.transform.position);

				// もし一番近くのオブジェクト情報と比較して、より距離が短かった場合
				if (length > distance)
				{
					// 距離を更新
					length = distance;
					if (obj.TryGetComponent(out Outline outline))
					{
						// 有効になっているアウトラインを無効化
						if (InteractOutline != null)
							InteractOutline.enabled = false;

						// より短い方のオブジェクトのアウトラインを登録、有効化
						InteractOutline = outline;
						InteractOutline.enabled = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// ポーズ開始時の
	/// </summary>
	private void Pause()
	{
		// 物理
		pauseVelocity = rb.velocity;
		pauseAngularVelocity = rb.angularVelocity;
		rb.isKinematic = true;
		// アニメーション
		anim.speed = 0.0f;

		_IsAppeal = false;

		// 持ったままポーズに入ると挙動おかしくなるので救済措置「捨てる」
		_IsHold = _IsDrag = false;
		InteractCollision = null;
	}

	private void Resumed()
	{
		// 物理
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngularVelocity;
		rb.isKinematic = false;
		// アニメーション
		anim.speed = 1.0f;
	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
	{
		if (bGamePad)
			_IsRun = moveInputValue.magnitude >= joyRunZone;

		//--- 速度、制限速度を定義
		float force, max;
		// アピール中の場合
		if (_IsAppeal)
		{
			force = appealForce;
			max = _maxAppealSpeed;
		}
		// 走り中の場合
		else if (_IsRun)
		{
			force = runForce;
			max = _maxRunSpeed;
		}
		// 歩いている場合
		else
		{
			force = moveForce;
			max = _maxMoveSpeed;
		}
		// 制限速度内の場合、移動方向の力を与える
		_vForce = new Vector3(moveInputValue.x, 0, moveInputValue.y) * force;

		//引きずっているとき
		if (_IsDrag)
		{
			//移動
			if (Vector3.Dot(vForce.normalized, transform.forward.normalized) <= -0.5f)
			{
				if (rb.velocity.magnitude < max && _vForce != Vector3.zero) rb.AddForce(_vForce);
			}
			//回転
			if (moveInputValue.normalized == Vector2.zero) return;
			float dragMoveAngle = (Vector3.SignedAngle(vForce.normalized, transform.forward.normalized, Vector3.up) <= 0.0f) ? -1.0f : 1.0f;
			transform.rotation *= Quaternion.AngleAxis(dragMoveAngle, Vector3.up);
		}

		//引きずっていないとき
		if (!_IsDrag)
		{
			//移動
			if (rb.velocity.magnitude < max && _vForce != Vector3.zero) rb.AddForce(_vForce);
			//回転
			if (moveInputValue.normalized == Vector2.zero) return;
			var fw = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 2;
			fw.y = 0.0f;
			transform.LookAt(transform.position + fw);
		}
#if false
		// 制限速度内の場合、移動方向の力を与える
		_vForce = new Vector3(moveInputValue.x, 0, moveInputValue.y) * force;
		if (rb.velocity.magnitude < max && _vForce != Vector3.zero)
			rb.AddForce(_vForce);

		// 進行方向に向かって回転する
		if (moveInputValue.normalized != Vector2.zero)
		{
			var fw = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 2;
			fw.y = 0;
			transform.LookAt(transform.position + fw);
		}
#endif
	}

	/// <summary>
	/// 走る
	/// </summary>
	private void OnRun(InputAction.CallbackContext context)
	{
		if (PauseManager.isPaused)
			return;

		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				bRunButton = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
				bRunButton = false;
				_IsRun = false;
				break;
		}
	}

	public void MoveSound()
	{
		SoundManager.Play(audioSource, seWalk);
	}

	/// <summary>
	/// 移動方向取得
	/// </summary>
	private void OnMove(InputAction.CallbackContext context)
	{
		if (PauseManager.isPaused)
			return;

		if (moveInputValue != context.ReadValue<Vector2>())
		{
			moveInputValue = context.ReadValue<Vector2>();
			rb.velocity = rb.velocity/2;
			rb.angularVelocity = rb.angularVelocity/2;
		}
		if (context.phase == InputActionPhase.Canceled)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		// 既にはたいている、アピール中、ポーズ中、掴んでいる場合は実行しない
		if (bHitMotion || IsAppeal || PauseManager.isPaused || _IsHold)
			return;

		bHitMotion = true;
		_IsHitMotion = true;
		anim.SetBool("Hit", bHitMotion);
		//Debug.Log($"Hit");
	}

	/// <summary>
	/// アニメーションから呼び出す用
	/// </summary>
	public void AnimHit()
	{
		Debug.Log($"Hit");

		// 範囲内に何もなければ、はたくモーションのみ再生
		if (WithinRange.Count == 0)
			return;

		//--- 現在のインタラクト対象を登録
		if (InteractOutline != null)
		{
			InteractCollision = InteractOutline.GetComponent<Collider>();
		}
		else    // もし一番近くのオブジェクト情報を保持していない場合
		{
			float length = 10.0f;

			// プレイヤーに一番近いオブジェクトをインタラクト対象とする
			foreach (Collider obj in WithinRange)
			{
				float distance = Vector3.Distance(transform.position, obj.transform.position);

				if (length > distance)
				{
					length = distance;
					InteractCollision = obj;
					break;
				}
			}
		}

		// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
		if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		{
			if (baseObj.objType == BaseObj.ObjType.HIT ||
				baseObj.objType == BaseObj.ObjType.HIT_HOLD)
			{
				Hit();
			}
		}
		else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject))
		{
			if (baseObject.objState == BaseObject.OBJState.HIT ||
				baseObject.objState == BaseObject.OBJState.HITANDHOLD)
			{
				Hit();
			}
		}
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void Hit()
	{
		var rigidbody = InteractCollision.GetComponent<Rigidbody>();
		float blowpower = 10.0f;    // 吹っ飛ぶ強さ
		float topvector = 0.1f;     // 吹っ飛ぶ強さ

		// プレイヤーが範囲内にいる時にインタラクトフラグがTrueになったらふき飛ぶよ
		rigidbody.isKinematic = false;

		// 吹っ飛ばし処理
		Vector3 vec = (InteractCollision.transform.position + new Vector3(0.0f, topvector, 0.0f) - transform.position).normalized;
		rigidbody.velocity = vec * blowpower;
		vec = (InteractCollision.transform.position - transform.position).normalized;
		rigidbody.AddTorque(vec * blowpower);

		_IsHit = true;
	}

	/// <summary>
	/// 咥える
	/// </summary>
	private void OnHold(InputAction.CallbackContext context)
	{
		//if (context.phase == InputActionPhase.Performed)
		//	Debug.Log($"InputActionPhase.Performed");
		//if (context.phase == InputActionPhase.Canceled)
		//	Debug.Log($"InputActionPhase.Canceled");

		if (PauseManager.isPaused)
			return;

		//--- 長押し開始
		if (context.phase == InputActionPhase.Performed)
		{
			// 範囲内のオブジェクトが無い場合は実行しない
			if (WithinRange.Count == 0)
				return;

			// 現在のインタラクト対象を登録
			if (InteractOutline != null)
			{
				InteractCollision = InteractOutline.GetComponent<Collider>();
			}
			else    // もし一番近くのオブジェクト情報を保持していない場合
			{
				float length = 10.0f;

				if (WithinRange.Count == 0)
					Debug.LogError($"WithinRange.Countが0です");

				// プレイヤーに一番近いオブジェクトをインタラクト対象とする
				foreach (Collider obj in WithinRange)
				{
					float distance = Vector3.Distance(transform.position, obj.transform.position);

					if (length > distance)
					{
						length = distance;
						InteractCollision = obj;
					}
				}
			}

			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
			{
				switch (baseObj.objType)
				{
					case BaseObj.ObjType.HOLD:
					case BaseObj.ObjType.HIT_HOLD:
						anim.SetBool("Carry", !_IsHold);
						break;
					case BaseObj.ObjType.DRAG:
					case BaseObj.ObjType.HIT_DRAG:
						anim.SetBool("Drag", !_IsHold);
						break;
				}
			}
			else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject))
			{

				if (baseObject.objState == BaseObject.OBJState.HOLD ||
					baseObject.objState == BaseObject.OBJState.HITANDHOLD)
				{
					anim.SetBool("Carry", !_IsHold);
				}
			}
		}

		// 長押し終了
		else if (context.phase == InputActionPhase.Canceled)
		{
			if (InteractCollision == null)
				return;

			// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
			{
				switch (baseObj.objType)
				{
					case BaseObj.ObjType.HOLD:
					case BaseObj.ObjType.HIT_HOLD:
						Hold(false);
						_IsHold = false;
						break;
					case BaseObj.ObjType.DRAG:
					case BaseObj.ObjType.HIT_DRAG:
						Drag(false);
						_IsHold = _IsDrag = false;
						break;
				}
#if false
				if (baseObj.objType == BaseObj.ObjType.HOLD ||
					baseObj.objType == BaseObj.ObjType.HIT_HOLD)
				{
					Hold(false);
					_IsHold = false;
				}
#endif
			}
			else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject))
			{

				if (baseObject.objState == BaseObject.OBJState.HOLD ||
					baseObject.objState == BaseObject.OBJState.HITANDHOLD)
				{
					Hold(false);
					_IsHold = false;
				}
			}
		}

	}

	public void AnimHold()
	{
		// TODO
		if (InteractCollision.tag == "Interact")
		{
			// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
			{
				switch (baseObj.objType)
				{
					case BaseObj.ObjType.HOLD:
					case BaseObj.ObjType.HIT_HOLD:
						Hold(true);
						_IsHold = true;
						break;
					case BaseObj.ObjType.DRAG:
					case BaseObj.ObjType.HIT_DRAG:
						Drag(true);
						_IsHold = _IsDrag = true;
						break;
				}
#if false
					if (baseObj.objType == BaseObj.ObjType.HOLD ||
						baseObj.objType == BaseObj.ObjType.HIT_HOLD)
					{
						Hold(true);
						_IsHold = true;
					}
#endif
			}
			else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject))
			{

				if (baseObject.objState == BaseObject.OBJState.HOLD ||
					baseObject.objState == BaseObject.OBJState.HITANDHOLD)
				{
					Hold(true);
					_IsHold = true;
				}
			}

		}
	}

	/// <summary>
	/// 咥える
	/// </summary>
	private void Hold(bool hold)
	{
		// 掴む処理
		if (hold)
		{
			if (InteractCollision.TryGetComponent(out Rigidbody rigidbody))
			{
				InteractCollision.transform.parent = transform;
				
				// メッシュのサイズを取得
				Bounds maxBounds = new Bounds(Vector3.zero, Vector3.zero);
				maxBounds.Encapsulate(InteractCollision.GetComponentInChildren<MeshFilter>().mesh.bounds);
				InteractBoundsSizeY = maxBounds.size.y / 2;

				//--- 掴む座標を取得
				InteractPoint = null;
				float distance = 10.0f;	// とりあえず10.0f

				for (int i = 0; i < InteractCollision.transform.childCount; i++)
				{
					var children = InteractCollision.transform.GetChild(i); // GetChild()で子オブジェクトを取得
					if (children.name == "HoldPoint")
					{
						// pointが空 || 現状のpointより距離が近い場合
						if (InteractPoint == null ||
							(InteractPoint != null && distance > Vector3.Distance(transform.position, children.transform.position)))
						{
							InteractPoint = children.transform;
							distance = Vector3.Distance(transform.position, InteractPoint.position);
						}
					}

				}

				// オブジェクトをくちばし辺りに移動
				//var pos = holdPos.transform.localPosition - point.localPosition;
				//InteractCollision.transform.localPosition = pos;
				InteractCollision.transform.parent = holdPos.transform;
				InteractCollision.transform.localPosition = Vector3.zero;
				//InteractCollision.transform.rotation = transform.rotation;
				InteractCollision.transform.rotation = Quaternion.identity;

				if (InteractCollision.GetComponent<HingeJoint>() == null)
				{
					// HingeJonitの角度制限を有効化
					InteractJoint = rigidbody.AddComponent<HingeJoint>();
					InteractJoint.connectedBody = rb;
					InteractJoint.useLimits = true;
					InteractJoint.anchor = new Vector3(0, InteractPoint.localPosition.y / InteractBoundsSizeY, 0);
					//InteractJoint.anchor = new Vector3(0, 1, 0);
					JointLimits jointLimits = InteractJoint.limits;
					jointLimits.min = -90.0f;
					jointLimits.max = 20.0f;
					jointLimits.bounciness = 0;
					InteractJoint.limits = jointLimits;
				}
			}
		}
		// 離す処理
		else
		{
			anim.SetFloat("AnimSpeed", 1.0f);

			InteractPoint = null;
			InteractJoint = null;
			Destroy(InteractCollision.GetComponent<HingeJoint>());

			if (InteractObjectParent != null)
				InteractCollision.transform.parent = InteractObjectParent.transform;
			else
				InteractCollision.transform.parent = null;

			InteractCollision = null;
		}
	}

	/// <summary>
	/// 引きずる処理
	/// </summary>
	/// <param name="bDrag"></param>
	private void Drag(bool bDrag)
	{
		//引きずり開始

		anim.SetBool("Drag", !_IsHold);
		if (bDrag)
		{
			//引きずり開始
			if (!InteractCollision.TryGetComponent(out Rigidbody rigidbody)) return;
			//HingeJointの設定
			InteractJoint = InteractCollision.GetComponent<HingeJoint>();
			if (!InteractJoint) InteractJoint = rigidbody.AddComponent<HingeJoint>();
			InteractJoint.connectedBody = rb;
			InteractJoint.anchor = InteractJoint.transform.InverseTransformPoint(transform.TransformPoint(holdPos.transform.localPosition));//HoldPosを設定
			InteractJoint.axis = Vector3.up;
			InteractJoint.useLimits = true;
			JointLimits jointLimits = InteractJoint.limits;
			jointLimits.min = -45.0f;
			jointLimits.max = 45.0f;
			InteractJoint.limits = jointLimits;
			InteractJoint.enableCollision = true;
		}
		else
		{
			//離す処理
			anim.SetFloat("AnimSpeed", 1.0f);
			Destroy(InteractCollision.GetComponent<HingeJoint>());
			InteractCollision = null;
		}
	}


	/// <summary>
	/// アピール
	/// </summary>
	private void OnAppeal(InputAction.CallbackContext context)
	{
		if (PauseManager.isPaused)
			return;

		switch (context.phase)
		{
			case InputActionPhase.Performed:
				//Debug.Log("アピール");
				_IsAppeal = true;
				break;
			case InputActionPhase.Canceled:
				//Debug.Log("アピール終了");
				_IsAppeal = false;
				break;
		}
		anim.SetBool("Appeal", _IsAppeal);
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

		// もしメガホンを保持している場合は、再生しない
		if (InteractCollision != null && InteractCollision.name.Contains("Megaphone"))
		{
			_IsMegaphone = true;
		}

		if (!_IsMegaphone)
			SoundManager.Play(audioSource, seCall);
	}

	public void AnimStop()
	{
		anim.SetFloat("AnimSpeed", 0.0f);
	}

	public void AnimCarryStop()
	{
		anim.SetBool("Carry", false);
	}

	#region 衝突判定
	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		if (InteractCollision != null && InteractCollision == collision.collider)
		{
			collision.transform.localPosition += Vector3.forward / 10;
			Debug.Log("<color=blue>離す</color>");
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Interact")
			return;

			WithinRange.Add(other);

		if (WithinRange.Count == 1 && other.TryGetComponent(out Outline outline))
			outline.enabled = true;
	}

	private void OnTriggerExit(Collider other)
	{
		//if (_IsHold)
			WithinRange.Remove(other);

		if (other.TryGetComponent(out Outline outline))
			outline.enabled = false;
	}
	#endregion

	public void ReStart()
	{
		// 捕まったら持ってるもの捨てる
		_IsHold = _IsDrag = false;
		InteractCollision = null;

		// インスペクターで設定したリスポーン位置に再配置する
		this.gameObject.transform.position = respawnZone.position;
	}
}
