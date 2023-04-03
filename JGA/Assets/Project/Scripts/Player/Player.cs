//=============================================================================
// @File	: [Player.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/02/25	スクリプト作成
// 2023/03/08	リスポーンするよ
// 2023/04/04	インタラクトオブジェクトの所を手直ししました～(吉原)
//				OnHit(コールバック関数)とOnHit(メインの処理)で名前がまったく同じだからメインの処理はHitにさせていただきましたわ！！！
//=============================================================================
using System.Collections.Generic;
using UniRx;
using UnityEngine;
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
	[SerializeField, Tooltip("歩行時速度")]
	private float moveForce = 7;
	[SerializeField, Tooltip("歩行時最高速度")]
	private float _maxMoveSpeed = 5;
	public float MaxMoveSpeed { get { return _maxMoveSpeed; } }
	[SerializeField, Tooltip("疾走速度倍率")]
	private float runMagnification = 1.5f;

	private float runForce;         //疾走時速度
	private float _maxRunSpeed;     //疾走時最高速度
	public float MaxRunSpeed { get { return _maxRunSpeed; } }
	private float appealForce;      //アピール時速度
	private float _maxAppealSpeed;  //アピール時最高速度
	public float MaxAppealSpeed { get { return _maxAppealSpeed; } }

	[SerializeField, Tooltip("ジョイスティックで走り始めるゾーン")]
	private float joyRunZone = 0.8f;
	[SerializeField, Tooltip("鳴く間隔の最小値"), Range(0.0f, 30.0f)]
	private float callMin = 0.5f;
	[SerializeField, Tooltip("鳴く間隔の最大値"), Range(0.0f, 30.0f)]
	private float callMax = 5.0f;
	[SerializeField]
	private float callInterval = 0;
	//----------------------------------------------------------------------------------------

	// フラグ --------------------------------------------------------------------------------
	//[SerializeField]
	//private bool _IsInteract;  // インタラクトフラグ
	//public bool IsInteract { get { return _IsInteract; } set { _IsInteract = value; } }
	//private bool DelayInteract;
	[SerializeField]
	private bool _IsHit;  // インタラクトフラグ
	public bool IsHit { get { return _IsHit; } set { _IsHit = value; } }
	private bool DelayHit;

	[SerializeField]
	private bool _IsHold;       // つかみフラグ
	public bool IsHold { get { return _IsHold; } }
	[SerializeField]
	private bool _IsMove;
	public bool IsMove { get { return _IsMove; } }
	//[SerializeField]
	//private bool _IsWalk;
	//public bool IsWalk { get { return _IsWalk; } }
	[SerializeField]
	private bool _IsRun;        // 走りフラグ
	public bool IsRun { get { return _IsRun; } }
	[SerializeField]
	private bool _IsAppeal;    // アピールフラグ
	public bool IsAppeal { get { return _IsAppeal; } }

	[SerializeField] private bool bGamePad;     // ゲームパッド接続確認フラグ

	private bool bAnimHit;

	private bool bHitMotion;
	//----------------------------------------------------------------------------------------

	[SerializeField] private Vector3 _vForce;
	public Vector3 vForce { get { return _vForce; } }

	private Vector3 pauseVelocity;
	private Vector3 pauseAngularVelocity;


	private MyContorller gameInputs;            // 方向キー入力取得
	private Vector2 moveInputValue;             // 移動方向

	private Collider InteractCollision;            // 掴んでいるオブジェクト：コリジョン
	private Rigidbody HoldObjectRb;             // 掴んでいるオブジェクト：重力関連
	private Outline InteractOutline;            // 掴んでいるオブジェクト：アウトライン

	[SerializeField]
	private List<Collider> WithinRange = new List<Collider>();  // インタラクト範囲内にあるオブジェクトリスト

	private Transform respawnZone;              // リスポーン位置プレハブ設定用

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		// ポーズ時の動作を登録
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);

		if (rb == null)
			rb = GetComponent<Rigidbody>();

		// 回転固定
		rb.constraints = RigidbodyConstraints.FreezeRotation;

		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();


		if (anim == null)
			anim = GetComponent<Animator>();

		var respawn = GameObject.Find("PlayerSpawn");
		if (respawn == null)
			Debug.LogError("<color=red>プレイヤーのリスポーン位置が見つかりません[Player.cs]</color>");
		else
			respawnZone = respawn.transform;


		// seCallの音量クソでかいので小さくする
		audioSource.volume = 0.2f;

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
		if (PauseManager.isPaused)
		{
			moveInputValue = Vector2.zero;
		}

		// ゲームパッドが接続されていないとnullになる。
		if (Gamepad.current == null)
			bGamePad = false;
		else
			bGamePad = true;

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

		// アピール中のランダム鳴き
		if (_IsAppeal)
		{
			if (callInterval > 0)
			{
				callInterval -= Time.deltaTime;
			}
			else
			{
				callInterval = Random.Range(callMin, callMax);
				OnCall();
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
		_IsMove = moveInputValue.normalized != Vector2.zero ? true : false;


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
		if (!bGamePad)
		{
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
			if (rb.velocity.magnitude < max)
				rb.AddForce(_vForce);

			// 進行方向に向かって回転する
			if (moveInputValue.normalized != Vector2.zero)
			{
				var fw = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 2;
				fw.y = 0;
				transform.LookAt(transform.position + fw);
			}
		}
		else
		{
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
			if (rb.velocity.magnitude < max)
				rb.AddForce(_vForce);

			// 進行方向に向かって回転する
			if (moveInputValue.normalized != Vector2.zero)
			{
				var fw = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 2;
				fw.y = 0;
				transform.LookAt(transform.position + fw);
			}
		}
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
				if (moveInputValue.normalized != Vector2.zero)
					_IsRun = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
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
		bHitMotion = true;
		anim.SetBool("Hit", bHitMotion);
		Debug.Log($"Hit");

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

			//if (InteractCollision.GetComponent<BaseObj>().objType == BaseObj.ObjType.HIT ||
			//	InteractCollision.GetComponent<BaseObj>().objType == BaseObj.ObjType.HIT_HOLD){
			//	OnHit();
			//}

			// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj)){

				if(baseObj.objType == BaseObj.ObjType.HIT ||
					baseObj.objType == BaseObj.ObjType.HIT_HOLD){
					Hit();
				}
			}
			else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject)){

				if(baseObject.objState == BaseObject.OBJState.HIT ||
					baseObject.objState == BaseObject.OBJState.HITANDHOLD){
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
		if (WithinRange.Count == 0 || PauseManager.isPaused)
			return;

		// 長押し
		if (context.phase == InputActionPhase.Performed)
		{
			// もし掴んでいたら離す
			if (HoldObjectRb != null)
			{
				_IsHold = false;
				
				// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
				if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj)){

					if (baseObj.objType == BaseObj.ObjType.HOLD ||
						baseObj.objType == BaseObj.ObjType.HIT_HOLD){
						Hold();
					}
				}
				else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject)){

					if (baseObject.objState == BaseObject.OBJState.HOLD ||
						baseObject.objState == BaseObject.OBJState.HITANDHOLD){
						Hold();
					}
				}
			}
			else
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

				switch (InteractCollision.tag)
				{
					case "Interact":
						_IsHold = true;
						// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
						if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj)){

							if (baseObj.objType == BaseObj.ObjType.HOLD ||
								baseObj.objType == BaseObj.ObjType.HIT_HOLD){
								Hold();
							}
						}
						else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject)){

							if (baseObject.objState == BaseObject.OBJState.HOLD ||
								baseObject.objState == BaseObject.OBJState.HITANDHOLD){
								Hold();
							}
						}
						break;
				}
			}

		}
		else if (HoldObjectRb != null)
		{
			_IsHold = false;
			// BaseObjとBaseObject二つあるため、それぞれ出来るように書きました(吉原 04/04 4:25)
			if (InteractCollision.TryGetComponent<BaseObj>(out var baseObj)){

				if (baseObj.objType == BaseObj.ObjType.HOLD ||
					baseObj.objType == BaseObj.ObjType.HIT_HOLD){
					Hold();
				}
			}
			else if (InteractCollision.TryGetComponent<BaseObject>(out var baseObject)){

				if (baseObject.objState == BaseObject.OBJState.HOLD ||
					baseObject.objState == BaseObject.OBJState.HITANDHOLD){
					Hold();
				}
			}
		}

	}

	/// <summary>
	/// 咥える
	/// </summary>
	private void Hold()
	{
		// 掴む処理
		if (_IsHold)
		{
			InteractCollision.transform.parent = transform;
			InteractCollision.transform.localPosition = new Vector3(0, 0.5f, InteractCollision.transform.localPosition.z);
			//InteractObject.transform.localRotation = Quaternion.identity;

			if (InteractCollision.TryGetComponent(out Rigidbody rigidbody))
			{
				HoldObjectRb = rigidbody;
				HoldObjectRb.useGravity = false;
				HoldObjectRb.isKinematic = true;
			}
			//SoundManager.Play(audioSource, SoundManager.ESE.);
		}

		// 離す処理
		else
		{
			InteractCollision.transform.parent = null;
			HoldObjectRb.useGravity = true;
			HoldObjectRb.isKinematic = false;
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
	private void OnCall()
	{
		var obj = EffectManager.Create(transform.position + new Vector3(0, 2, 0), 0, transform.rotation);
		obj.transform.parent = transform;
		SoundManager.Play(audioSource, seCall);
	}

	#region 衝突判定
	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		//if (InteractObject == collision.collider)
		//	collision.transform.localPosition += Vector3.forward / 10;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (/*other.tag != "holdObject" && */other.tag != "Interact")
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
		// インスペクターで設定したリスポーン位置に再配置する
		this.gameObject.transform.position = respawnZone.position;
	}
}
