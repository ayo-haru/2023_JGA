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
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
	new private Transform transform;
	private Rigidbody rb;
	private AudioSource audioSource;
	private Animator anim;           // Animatorへの参照

	// Animatorパラメータ
	private int HashMove = 0;
	private int HashRun = 0;
	private int HashAppeal = 0;
	private int HashHit = 0;
	private int HashCarry = 0;
	private int HashDrag = 0;
	private int HashRandom = 0;
	private int HashAnimSpeed = 0;

	[Header("ステータス")] //-----------------------------------------------------------------
	[SerializeField] private float moveForce = 7;           // 歩行時速度
	[SerializeField] private float _maxMoveSpeed = 5;       // 歩行時最高速度
	[SerializeField] private float runMagnification = 1.5f; // 疾走速度倍率
	private float runForce;                                 // 疾走時速度
	private float _maxRunSpeed;                             // 疾走時最高速度
	private float appealForce;                              // アピール時速度
	private float _maxAppealSpeed;                          // アピール時最高速度

	public float MaxMoveSpeed { get { return _maxMoveSpeed; } }
	public float MaxRunSpeed { get { return _maxRunSpeed; } }
	public float MaxAppealSpeed { get { return _maxAppealSpeed; } }

	[SerializeField] private float joyRunZone = 0.8f;   // ジョイスティックで走り始めるゾーン

	//----------------------------------------------------------------------------------------

	[Header("フラグ")] //---------------------------------------------------------------------
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

	public bool IsHit { get { return _IsHit; } set { _IsHit = value; } }
	public bool IsHitMotion { get { return _IsHitMotion; } }
	public bool IsHold { get { return _IsHold; } }
	public bool IsDrag { get { return _IsDrag; } }
	public bool IsMove { get { return _IsMove; } }
	public bool IsRun { get { return _IsRun; } }
	public bool IsAppeal { get { return _IsAppeal; } }
	public bool IsRandom { get { return _IsRandom; } }
	public bool IsMegaphone { get { return _IsMegaphone; } }

	private bool bHitMotion;                    // はたくモーション中は他のモーションさせないフラグ

	private bool DelayHit;
	private bool DelayHitMotion;
	private bool DelayMegaphone;
	//----------------------------------------------------------------------------------------

	// 移動 ----------------------------------------------------------------------------------
	[SerializeField] private Vector3 _vForce;               // 移動方向
	public Vector3 vForce { get { return _vForce; } }

	[SerializeField] private Collider InteractCollision;        // 掴んでいるオブジェクト：コリジョン
	[SerializeField] private Outline InteractOutline;       // 掴んでいるオブジェクト：アウトライン
	[SerializeField] private HingeJoint InteractJoint;          // 掴んでいるオブジェクト：HingeJoint
	[SerializeField] private Vector3 InteractJointAnchor;    // 掴んでいるオブジェクト：InteractJoint.anchor

	[SerializeField] private HashSet<Collider> WithinRange = new HashSet<Collider>();  // インタラクト範囲内にあるオブジェクトリスト

#if UseMyContorller
	private MyContorller gameInputs;	// 方向キー入力取得
#else
	[SerializeField] private InputActionReference actionMove;
	[SerializeField] private InputActionReference actionAppeal;
	[SerializeField] private InputActionReference actionHit;
	[SerializeField] private InputActionReference actionHold;
	[SerializeField] private InputActionReference actionRun; //PC only
#endif

	private KeyConfigPanel keyConfigPanel;                 // キーコンフィグ変更検知用
	private Vector2 moveInputValue;                 // 移動方向
	private Vector3 pauseVelocity;                  // ポーズ時の加速度保存
	private Vector3 pauseAngularVelocity;           // ポーズ時の加速度保存
	[SerializeField] private Transform respawnZone;                 // リスポーン位置プレハブ設定用

	//----------------------------------------------------------------------------------------

	[SerializeField] private Rigidbody holdPos;    // 持つときの位置

	private GameObject InteractObjectParent;        // シーン上の「InteractObject」

	private float IdolTime = 0;                     // 待機中のランダムな挙動用カウンタ


	void Awake()
	{
		//--- ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		//--- NULLリファレンス回避
		if (transform == null) transform = this.gameObject.transform;
		if (rb == null) rb = GetComponent<Rigidbody>();
		if (audioSource == null) audioSource = GetComponent<AudioSource>();
		if (anim == null) anim = GetComponent<Animator>();

		HashMove = Animator.StringToHash("move");
		HashRun = Animator.StringToHash("run");
		HashAppeal = Animator.StringToHash("Appeal");
		HashHit = Animator.StringToHash("Hit");
		HashCarry = Animator.StringToHash("Carry");
		HashDrag = Animator.StringToHash("Drag");
		HashRandom = Animator.StringToHash("Random");
		HashAnimSpeed = Animator.StringToHash("AnimSpeed");

		//--- 回転固定
		rb.constraints = RigidbodyConstraints.FreezeRotation;

		//--- 持つときの場所を子オブジェクトから検索
		if (holdPos == null)
		{
			var obj = transform.Find("HoldPos").gameObject;
			if (obj != null)
			{
				if (obj.TryGetComponent(out Rigidbody rigidbody))
					holdPos = rigidbody;
				else
					holdPos = obj.AddComponent<Rigidbody>();
				holdPos.useGravity = false;
				holdPos.isKinematic = false;
			}
		}

		//--- プレイヤー初期位置
		if (respawnZone == null)
		{
			var respawn = GameObject.Find("PlayerSpawn");
			if (respawn == null)
				Debug.LogError("<color=red>プレイヤーのリスポーン位置が見つかりません[Player.cs]</color>");
			else
				respawnZone = respawn.transform;
		}

		//--- 物を離した際に元の階層に戻すためのやつ
		InteractObjectParent = GameObject.Find("InteractObject");


		//--- Input Actionイベント登録
#if UseMyContorller
		// インスタンス生成
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
#else
		actionMove.action.performed += OnMove;
		actionMove.action.canceled += OnMove;
		actionAppeal.action.performed += OnAppeal;
		actionAppeal.action.canceled += OnAppeal;
		actionHit.action.performed += OnHit;
		actionHold.action.performed += OnHold;
		actionHold.action.canceled += OnHold;
		actionRun.action.performed += OnRun;
		actionRun.action.canceled += OnRun;

		// Input Actionを有効化
		actionMove.ToInputAction().Enable();
		actionAppeal.ToInputAction().Enable();
		actionHit.ToInputAction().Enable();
		actionHold.ToInputAction().Enable();
		actionRun.ToInputAction().Enable();
#endif

		if (keyConfigPanel == null)
		{
			var obj = GameObject.Find("KeyConfigPanel");
			if (obj)
				keyConfigPanel = obj.GetComponent<KeyConfigPanel>();
		}

		//--- 速度設定
		runForce = moveForce * runMagnification;
		_maxRunSpeed = _maxMoveSpeed * runMagnification;
		appealForce = (moveForce + runForce) / 2;
		_maxAppealSpeed = (_maxMoveSpeed + _maxRunSpeed) / 2;
	}

	private void OnDisable()
	{
		// Input Actionを無効化
#if UseMyContorller
		gameInputs.Disable();
#else
		actionMove.ToInputAction().Enable();
		actionAppeal.ToInputAction().Enable();
		actionHit.ToInputAction().Enable();
		actionHold.ToInputAction().Enable();
		actionRun.ToInputAction().Enable();
#endif
	}

	private void FixedUpdate()
	{
		// はたき中でないとき移動
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
			Move();

		if (_IsHold && _IsDrag)
		{
			if (InteractJoint != null)
				InteractJoint.anchor = InteractJointAnchor;
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

		// リスタート処理
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

		//--- 走っているか判定
		if (!bGamePad)
			_IsRun = !bGamePad && bRunButton && moveInputValue.normalized != Vector2.zero;

		// アニメーション
		if (!bHitMotion)
		{
			anim.SetBool(HashMove, _IsMove);
			anim.SetBool(HashRun, _IsRun);
		}
		// はたくモーション中は他のモーションさせない
		else
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			if (stateInfo.IsName("Hit"))
			{
				// 再生終了時に他モーション再生を許可
				if (stateInfo.normalizedTime >= 1.0f)
				{
					bHitMotion = false;
					anim.SetBool(HashHit, bHitMotion);
				}
			}
		}


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
				anim.SetBool(HashRandom, true);
			}
		}

		// ランダム挙動
		if (_IsRandom)
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			// ランダムな挙動の再生が終了した時
			if (stateInfo.normalizedTime >= 1.0f)
			{
				_IsRandom = false;
				anim.SetBool(HashRandom, false);
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

	#region ポーズ処理
	/// <summary>
	/// ポーズ開始時処理
	/// </summary>
	private void Pause()
	{
		// Input Actionを無効化
#if UseMyContorller
		gameInputs.Disable();
#else
		actionMove.ToInputAction().Disable();
		actionAppeal.ToInputAction().Disable();
		actionHit.ToInputAction().Disable();
		actionHold.ToInputAction().Disable();
		actionRun.ToInputAction().Disable();
#endif

		// 物理停止し値を保存しておく
		pauseVelocity = rb.velocity;
		pauseAngularVelocity = rb.angularVelocity;
		rb.isKinematic = true;

		// アニメーションは停止
		anim.speed = 0.0f;

		_IsAppeal = false;
		anim.SetBool(HashAppeal, _IsAppeal);

		_IsHold = _IsDrag = false;
		InteractCollision = null;
	}

	/// <summary>
	/// ポーズ終了時処理
	/// </summary>
	private void Resumed()
	{
		// Input Actionを有効化
#if UseMyContorller
		gameInputs.Enable();
#else
		actionMove.ToInputAction().Enable();
		actionAppeal.ToInputAction().Enable();
		actionHit.ToInputAction().Enable();
		actionHold.ToInputAction().Enable();
		actionRun.ToInputAction().Enable();
#endif

		// 物理を再開させポーズ前の値を代入
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngularVelocity;
		rb.isKinematic = false;

		// アニメーションを再開
		anim.speed = 1.0f;
	}
	#endregion

	#region 入力時処理
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
			if (!bGamePad)
			{
				rb.velocity = rb.velocity / 2;
				rb.angularVelocity = rb.angularVelocity / 2;
			}
		}
		if (context.phase == InputActionPhase.Canceled)
		{
			moveInputValue = Vector2.zero;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		// 以下の場合は実行しない
		// ・既にはたいている
		// ・アピール中
		// ・ポーズ中
		// ・掴んでいる
		if (bHitMotion || IsAppeal || PauseManager.isPaused || _IsHold)
			return;

		bHitMotion = true;
		_IsHitMotion = true;
		if (HashHit == 0)
			HashHit = Animator.StringToHash("Hit");
		anim.SetBool(HashHit, bHitMotion);
	}

	/// <summary>
	/// 咥える
	/// </summary>
	private void OnHold(InputAction.CallbackContext context)
	{
		// ポーズ中は実行しない
		if (PauseManager.isPaused)
			return;

		//--- 長押し開始（つかむ処理）
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

			// オブジェクトの種類によって持つアニメーションを変える
			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
			{
				switch (baseObj.objType)
				{
					case BaseObj.ObjType.HOLD:
					case BaseObj.ObjType.HIT_HOLD:
						anim.SetBool(HashCarry, !_IsHold);
						break;
					case BaseObj.ObjType.DRAG:
					case BaseObj.ObjType.HIT_DRAG:
						anim.SetBool(HashDrag, !_IsHold);
						break;
				}
			}
		}

		//--- 長押し終了（離す処理）
		else if (context.phase == InputActionPhase.Canceled)
		{
			if (InteractCollision == null)
				return;

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
			}
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
			// アピール開始
			case InputActionPhase.Performed:
				_IsAppeal = true;
				break;

			// アピール終了
			case InputActionPhase.Canceled:
				_IsAppeal = false;
				break;
		}
		anim.SetBool(HashAppeal, _IsAppeal);
	}
	#endregion

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
	{
		// ゲームパッド接続時、傾きがjoyRunZone以上の時「走り」の判定にする
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
		else
		{
			//移動
			if (rb.velocity.magnitude < max && _vForce != Vector3.zero) rb.AddForce(_vForce);
			//回転
			if (moveInputValue.normalized == Vector2.zero) return;
			var fw = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 2;
			fw.y = 0.0f;
			transform.LookAt(transform.position + fw);
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
	private void Hold(bool hold)
	{
		// 掴む処理
		if (hold)
		{
			if (InteractCollision.TryGetComponent(out Rigidbody rigidbody))
			{
				// メッシュのサイズを取得
				Bounds maxBounds = new Bounds(Vector3.zero, Vector3.zero);
				maxBounds.Encapsulate(InteractCollision.GetComponentInChildren<MeshFilter>().mesh.bounds);
				float InteractBoundsSizeY = maxBounds.size.y / 2;

				//--- 掴む座標を取得
				Transform InteractPoint = null;
				float distance = 10.0f; // とりあえず10.0f

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
							distance = Vector3.Distance(holdPos.transform.localPosition, InteractPoint.position);
						}
					}
				}

				// オブジェクトをくちばし辺りに移動
				//var pos = holdPos.transform.localPosition - point.localPosition;
				//InteractCollision.transform.localPosition = pos;
				//InteractCollision.transform.parent = transform;
				InteractCollision.transform.parent = holdPos.transform;
				InteractCollision.transform.localPosition = Vector3.zero;
				InteractCollision.transform.rotation = transform.rotation;
				float jointLimitsMin = -90.0f;
				float jointLimitsMax = 45.0f;

				bool bChainsawMan = false;
				if (InteractCollision.TryGetComponent(out FishObject fish))
				{
					bChainsawMan = true;

					// Ｙ軸を中心に回転
					InteractCollision.transform.rotation = Quaternion.AngleAxis(90, Vector3.up) * InteractCollision.transform.rotation;
				}
				else if (InteractCollision.TryGetComponent(out MegaPhone mega))
				{
					// Ｙ軸を中心に回転
					holdPos.transform.rotation = Quaternion.AngleAxis(90, Vector3.up) * transform.rotation;
					// Ｚ軸を中心に回転
					//InteractCollision.transform.rotation = Quaternion.AngleAxis(-45, Vector3.forward) * InteractCollision.transform.rotation;
				}
				else if (InteractCollision.TryGetComponent(out Radio rad))
				{
					// Ｙ軸を中心に回転
					InteractCollision.transform.rotation = Quaternion.AngleAxis(180, Vector3.up) * InteractCollision.transform.rotation;
					jointLimitsMin = 10.0f;
					jointLimitsMax = -90.0f;
				}
				
				if (InteractCollision.GetComponent<HingeJoint>() == null)
				{
					// HingeJonitの角度制限を有効化
					InteractJoint = rigidbody.AddComponent<HingeJoint>();
					InteractJoint.autoConfigureConnectedAnchor = false;
					InteractJoint.connectedBody = holdPos;
					InteractJoint.useLimits = true;
					InteractJoint.anchor = InteractJointAnchor = new Vector3(0, InteractPoint.localPosition.y / InteractBoundsSizeY, 0);
					if (bChainsawMan)
						InteractJoint.axis = new Vector3(0, 0, 1);
					JointLimits jointLimits = InteractJoint.limits;
					jointLimits.min = jointLimitsMin;
					jointLimits.max = jointLimitsMax;
					jointLimits.bounciness = 0;
					InteractJoint.limits = jointLimits;
				}
			}
		}
		// 離す処理
		else
		{
			anim.SetFloat(HashAnimSpeed, 1.0f);

			Destroy(InteractJoint);
			InteractJoint = null;

			if (InteractObjectParent != null)
				InteractCollision.transform.parent = InteractObjectParent.transform;
			else
				InteractCollision.transform.parent = null;

			InteractCollision = null;
			InteractJointAnchor = Vector3.zero;
		}
	}

	/// <summary>
	/// 引きずる処理
	/// </summary>
	/// <param name="bDrag"></param>
	private void Drag(bool bDrag)
	{
		//引きずり開始

		anim.SetBool(HashDrag, bDrag);
		if (bDrag)
		{
			InteractCollision.transform.parent = transform;
			//InteractCollision.transform.parent = holdPos.transform;

			if (InteractCollision.name.Contains("Corn"))
			{
				InteractCollision.GetComponent<Rigidbody>().mass = 1.0f;
			}

			//引きずり開始
			if (!InteractCollision.TryGetComponent(out Rigidbody rigidbody)) return;
			//HingeJointの設定
			InteractJoint = InteractCollision.GetComponent<HingeJoint>();
			if (!InteractJoint) InteractJoint = rigidbody.AddComponent<HingeJoint>();
			InteractJoint.connectedBody = rb;
			//InteractJoint.anchor = InteractJoint.transform.InverseTransformPoint(transform.TransformPoint(holdPos.transform.localPosition));//HoldPosを設定
			InteractJoint.anchor = transform.position - InteractJoint.transform.position;
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
			if (InteractObjectParent != null)
				InteractCollision.transform.parent = InteractObjectParent.transform;
			else
				InteractCollision.transform.parent = null;

			if (InteractCollision.name.Contains("Corn"))
				InteractCollision.GetComponent<Rigidbody>().mass = 5.0f;

			//離す処理
			anim.SetFloat(HashAnimSpeed, 1.0f);
			Destroy(InteractJoint);
			InteractJoint = null;
			InteractCollision = null;
			InteractJointAnchor = Vector3.zero;
		}
	}

	#region アニメーションから呼び出す処理
	public void MoveSound()
	{
		SoundManager.Play(audioSource, SoundManager.ESE.PENGUIN_WALK_002);
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

		// もしメガホンを保持している場合は、再生しない
		if (InteractCollision != null && InteractCollision.name.Contains("Megaphone"))
			_IsMegaphone = true;
		else
			SoundManager.Play(audioSource, SoundManager.ESE.PENGUIN_VOICE);
	}

	public void AnimCarryStop()
	{
		anim.SetBool(HashCarry, false);
	}

	public void AnimHold()
	{
		if (InteractCollision == null)
			return;

		if (InteractCollision.CompareTag("Interact"))
		{
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
			}
		}
	}

	public void AnimHit()
	{
		// 範囲内に何もなければ、何もしない
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

		if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj))
		{
			if (baseObj.objType == BaseObj.ObjType.HIT ||
				baseObj.objType == BaseObj.ObjType.HIT_HOLD)
			{
				Hit();
			}
		}
	}
	#endregion

	#region 衝突判定
	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		if (InteractCollision != null && InteractCollision == collision.collider)
			collision.transform.localPosition += Vector3.forward / 10;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Interact"))
			return;

		WithinRange.Add(other);

		if (WithinRange.Count == 1 && other.TryGetComponent(out Outline outline))
			outline.enabled = true;
	}

	private void OnTriggerExit(Collider other)
	{
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
