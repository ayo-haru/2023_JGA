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
	[SerializeField] private Rigidbody rb;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip seCall;          // ＳＥ：鳴き声
	[SerializeField] private AudioClip seHit;           // ＳＥ：はたく
	[SerializeField] private AudioClip seHold;          // ＳＥ：つかむ
	[SerializeField] private AudioClip seWalk;          // ＳＥ：歩く
	[SerializeField] private Animator anim;             // Animatorへの参照

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
	[SerializeField] private float callMin = 0.5f;      // 鳴く間隔の最小値
	[SerializeField] private float callMax = 5.0f;      // 鳴く間隔の最大値
	[SerializeField] private float callInterval = 0;
	//----------------------------------------------------------------------------------------

	// フラグ --------------------------------------------------------------------------------
	private bool bRunButton;                    // [PC]シフトキー入力状態
	[SerializeField] private bool bGamePad;     // ゲームパッド接続確認フラグ

	[SerializeField] private bool _IsHit;       // インタラクトフラグ
	[SerializeField] private bool _IsHold;      // つかみフラグ
	[SerializeField] private bool _IsDrag;      // 引きずりフラグ
	[SerializeField] private bool _IsMove;      // 移動フラグ
	[SerializeField] private bool _IsRun;       // 走りフラグ
	[SerializeField] private bool _IsAppeal;    // アピールフラグ
	[SerializeField] private bool _IsRandom;    // 待機中のランダムな挙動
	[SerializeField] private bool _IsMegaphone; // メガホン用フラグ

	public bool IsHit		{ get { return _IsHit; } set { _IsHit = value; } }
	public bool IsHold		{ get { return _IsHold; } }
	public bool IsDrag		{ get { return _IsDrag; } }
	public bool IsMove		{ get { return _IsMove; } }
	public bool IsRun		{ get { return _IsRun; } }
	public bool IsAppeal	{ get { return _IsAppeal; } }
	public bool IsRandom	{ get { return _IsRandom; } }
	public bool IsMegaphone	{ get { return _IsMegaphone; } }

	private bool bHitMotion;                    // はたくモーション中は他のモーションさせないフラグ

	private bool DelayHit;
	private bool DelayMegaphone;
	//----------------------------------------------------------------------------------------

	// 移動 ----------------------------------------------------------------------------------
	[SerializeField] private Vector3 _vForce;               // 移動方向
	public Vector3 vForce { get { return _vForce; } }

	private Vector3 pauseVelocity;                          // ポーズ時の加速度保存
	private Vector3 pauseAngularVelocity;                   // ポーズ時の加速度保存

	private MyContorller gameInputs;                        // 方向キー入力取得
	private Vector2 moveInputValue;                         // 移動方向

	[SerializeField] private Collider InteractCollision;    // 掴んでいるオブジェクト：コリジョン
	[SerializeField] private Rigidbody HoldObjectRb;        // 掴んでいるオブジェクト：重力関連
	[SerializeField] private Outline InteractOutline;       // 掴んでいるオブジェクト：アウトライン

	[SerializeField] private List<Collider> WithinRange = new List<Collider>();  // インタラクト範囲内にあるオブジェクトリスト

	private Transform respawnZone;                          // リスポーン位置プレハブ設定用
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

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	private void FixedUpdate()
	{
		// はたき中でないとき移動
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
			Move();

	}

	private void Update()
	{
		// ポーズ中は移動しない
		if (PauseManager.isPaused)
			moveInputValue = Vector2.zero;
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
			if (!DelayHit)
			{
				DelayHit = true;
			}
			else
			{
				DelayHit = false;
				_IsHit = false;
			}
		}

		if (_IsMegaphone)
		{
			if (!DelayMegaphone)
			{
				DelayMegaphone = true;
			}
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
		else
		{
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
			{
				AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
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

		// 走っているか
		if (bRunButton && moveInputValue.normalized != Vector2.zero)
			_IsRun = true;
		else
			_IsRun = false;

		float length;
		if (InteractOutline != null)
		{
			length = Vector3.Distance(transform.position, InteractOutline.transform.position);
		}
		else
		{
			length = 10.0f;
		}

		// 移動中判定
		_IsMove = moveInputValue.normalized != Vector2.zero;

		// 待機中加算
		if (!_IsMove && !_IsAppeal && !_IsRandom)
		{
			IdolTime += Time.deltaTime;

			// もし２秒経過していたら
			if (IdolTime > 2.0f)
			{
				if (Random.Range(0, 2) == 1) // ※ 0～1の範囲でランダムな整数値が返る
				{
					IdolTime = 0.0f;
					_IsRandom = true;
					anim.SetBool("Random", true);
				}
			}
		}

		if (_IsRandom)
		{
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			// 再生中か？
			if (stateInfo.normalizedTime < 1.0f)
			{
				//Debug.Log($"再生中");
			}
			else
			{
				_IsRandom = false;
				anim.SetBool("Random", false);
			}
		}


		if (WithinRange.Count == 0 && InteractOutline != null)
		{
			InteractOutline.enabled = false;
			InteractOutline = null;
		}
		// プレイヤーに一番近いオブジェクトをインタラクト対象とする
		else
		{
			foreach (Collider obj in WithinRange)
			{
				if (InteractOutline != null && obj == InteractOutline.gameObject)
				{
					InteractOutline.enabled = true;
					continue;
				}

				float distance = Vector3.Distance(transform.position, obj.transform.position);

				if (length > distance)
				{
					length = distance;
					if (obj.TryGetComponent(out Outline outline))
					{
						if (InteractOutline != null)
							InteractOutline.enabled = false;
						InteractOutline = outline;
						InteractOutline.enabled = true;
					}
				}
			}
		}

		if (_IsHold)
		{
			//InteractCollision.transform.position = holdPos.transform.position;
			//InteractCollision.transform.rotation = holdPos.transform.rotation;
		}
	}

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
		HoldObjectRb = null;
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

		float force, max;

		if (_IsAppeal)
		{
			force = appealForce;
			max = _maxAppealSpeed;
		}
		else if (_IsRun)
		{
			force = runForce;
			max = _maxRunSpeed;
		}
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

		moveInputValue = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		if (bHitMotion || IsAppeal)
			return;

		bHitMotion = true;
		anim.SetBool("Hit", bHitMotion);
		//Debug.Log($"Hit");


		if (WithinRange.Count == 0 || PauseManager.isPaused || _IsHold)
			return;


		// 押された瞬間
		if (context.phase == InputActionPhase.Performed)
		{
			// 現在のインタラクト対象を登録
			if (InteractOutline != null)
			{
				InteractCollision = InteractOutline.GetComponent<Collider>();
			}
			else
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
		Vector3 vec = (InteractCollision.transform.position + new Vector3(0.0f, topvector, 0.0f) - transform.position).normalized;
		rigidbody.velocity = vec * blowpower;
		vec = (InteractCollision.transform.position - transform.position).normalized;
		rigidbody.AddTorque(vec * blowpower);

		_IsHit = true;

		//InteractObject.GetComponent<AudioSource>().Play();

		//SoundManager.Play(audioSource, SoundManager.ESE.);
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

		if (WithinRange.Count == 0 || PauseManager.isPaused)
			return;

		// 長押し開始
		if (context.phase == InputActionPhase.Performed)
		{
			// 現在のインタラクト対象を登録
			if (InteractOutline != null)
			{
				InteractCollision = InteractOutline.GetComponent<Collider>();
			}
			else
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

		// 長押し終了
		else if (context.phase == InputActionPhase.Canceled)
		{
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

	/// <summary>
	/// 咥える
	/// </summary>
	private void Hold(bool hold)
	{
		anim.SetBool("Hold", !_IsHold);

		// 掴む処理
		if (hold)
		{
			if (InteractCollision.TryGetComponent(out Rigidbody rigidbody))
			{
				HoldObjectRb = rigidbody;

				InteractCollision.transform.parent = transform;
				InteractCollision.transform.rotation = transform.rotation;

				if (InteractCollision.GetComponent<HingeJoint>() == null)
				{
					var joint = rigidbody.AddComponent<HingeJoint>();
					joint.connectedBody = rb;
					joint.anchor = new Vector3(0, 1.0f, 0);
				}
			}
		}
		// 離す処理
		else
		{
			anim.SetFloat("AnimSpeed", 1.0f);

			Destroy(InteractCollision.GetComponent<HingeJoint>());

			if (InteractObjectParent != null)
				InteractCollision.transform.parent = InteractObjectParent.transform;
			else
				InteractCollision.transform.parent = null;

			InteractCollision = null;
			HoldObjectRb = null;
		}
	}
	/// <summary>
	/// 引きずる処理
	/// </summary>
	/// <param name="bDrag"></param>
	private void Drag(bool bDrag)
	{
		//引きずり開始

		anim.SetBool("Hold", !_IsHold);
		if (bDrag)
		{
			//引きずり開始
			if (!InteractCollision.TryGetComponent(out Rigidbody rigidbody)) return;
			HoldObjectRb = rigidbody;
			//HingeJointの設定
			HingeJoint joint = InteractCollision.GetComponent<HingeJoint>();
			if (!joint) joint = rigidbody.AddComponent<HingeJoint>();
			joint.connectedBody = rb;
			joint.anchor = joint.transform.InverseTransformPoint(transform.TransformPoint(holdPos.transform.localPosition));//HoldPosを設定
			joint.axis = Vector3.up;
			joint.useLimits = true;
			JointLimits jointLimits = joint.limits;
			jointLimits.min = -45.0f;
			jointLimits.max = 45.0f;
			joint.limits = jointLimits;
			joint.enableCollision = true;
		}
		else
		{
			//離す処理
			anim.SetFloat("AnimSpeed", 1.0f);
			Destroy(InteractCollision.GetComponent<HingeJoint>());
			InteractCollision = null;
			HoldObjectRb = null;
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
		var obj = EffectManager.Create(transform.position + new Vector3(0, 4, 0), 0, transform.rotation);
		obj.transform.localScale = Vector3.one * 5;
		obj.transform.parent = transform;

		//Debug.Log($"InteractCollision:{InteractCollision}");
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

	//public void PlaySoundWalk()
	//{
	//	SoundManager.Play(audioSource, seWalk);
	//}

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
		HoldObjectRb = null;

		// インスペクターで設定したリスポーン位置に再配置する
		this.gameObject.transform.position = respawnZone.position;
	}
}
