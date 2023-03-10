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
//=============================================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip seCall;          // ＳＥ：鳴き声
	[SerializeField] private Animator anim;             // Animatorへの参照

	[Header("ステータス")]
	[SerializeField, Tooltip("歩行時速度")]
	private float moveForce = 7;
	[SerializeField, Tooltip("アピール時速度")]
	private float appealForce = 8.75f;
	[SerializeField, Tooltip("疾走時速度")]
	private float runForce = 10.5f;
	[SerializeField, Tooltip("歩行時最高速度")]
	private float maxMoveSpeed = 5;
	[SerializeField, Tooltip("アピール時最高速度")]
	private float maxAppealSpeed = 6.25f;
	[SerializeField, Tooltip("疾走時最高速度")]
	private float maxRunSpeed = 7.5f;
	[SerializeField, Tooltip("ジョイスティックで走り始めるゾーン")]
	private float joyRunZone = 0.8f;
	[SerializeField, Tooltip("鳴く間隔の最小値"), Range(0.0f, 30.0f)]
	private float callMin = 0.5f;
	[SerializeField, Tooltip("鳴く間隔の最大値"), Range(0.0f, 30.0f)]
	private float callMax = 5.0f;
	[SerializeField]
	private float callInterval = 0;


	[SerializeField] private Vector3 _vForce;
	public Vector3 vForce { get { return _vForce; } }


	[SerializeField] private bool _IsInteract;   // インタラクトフラグ
	public bool IsInteract { get { return _IsInteract; } set { _IsInteract = value; } }        // インタラクトプロパティ
	private bool delay;

	[SerializeField] private bool isHold;       // つかみフラグ
	[SerializeField] private bool isRun;        // 走りフラグ
	[SerializeField] private bool isAppeal;     // アピールフラグ


	[SerializeField] private bool bGamePad;     // ゲームパッド接続確認フラグ
	private MyContorller gameInputs;            // 方向キー入力取得
	private Vector2 moveInputValue;             // 移動方向

	private Collider InteractObject;            // 掴んでいるオブジェクト：コリジョン
	private Rigidbody HoldObjectRb;             // 掴んでいるオブジェクト：重力関連
	[SerializeField]
	private List<Collider> WithinRange = new List<Collider>();  // インタラクト範囲内にあるオブジェクトリスト

	private Transform respawnZone;              // リスポーン位置プレハブ設定用

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
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
		gameInputs.Player.Interact.performed += OnInteract;
		gameInputs.Player.Interact.canceled += OnInteract;
		gameInputs.Player.Run.performed += OnRun;
		gameInputs.Player.Run.canceled += OnRun;

		// Input Actionを有効化
		gameInputs.Enable();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	private void FixedUpdate()
	{
		Move();

	}

	private void Update()
	{
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

		if (_IsInteract)
		{
			if (!delay)
				delay = true;
			else
			{
				delay = false;
				_IsInteract = false;
			}
		}

		if (isAppeal)
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


		anim.SetBool("move", moveInputValue.normalized != Vector2.zero);
		anim.SetBool("run", isRun);
	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
	{
		if (!bGamePad)
		{
			float force;

			if (isAppeal)
				force = appealForce;
			else if (isRun)
				force = runForce;
			else
				force = moveForce;

			// 制限速度内の場合、移動方向の力を与える
			_vForce = new Vector3(moveInputValue.x, 0, moveInputValue.y) * force;
			if (rb.velocity.magnitude < (isRun ? maxRunSpeed : maxMoveSpeed))
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
			isRun = moveInputValue.magnitude >= joyRunZone;

			float force;

			if (isAppeal)
				force = appealForce;
			else if (isRun)
				force = runForce;
			else
				force = moveForce;

			// 制限速度内の場合、移動方向の力を与える
			_vForce = new Vector3(moveInputValue.x, 0, moveInputValue.y) * force;
			if (rb.velocity.magnitude < (isRun ? maxRunSpeed : maxMoveSpeed))
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
		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				isRun = true;
				break;
			// 離された時
			case InputActionPhase.Canceled:
				isRun = false;
				break;
		}
	}


	/// <summary>
	/// 移動方向取得
	/// </summary>
	private void OnMove(InputAction.CallbackContext context)
	{
		moveInputValue = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// インタラクト
	/// </summary>
	private void OnInteract(InputAction.CallbackContext context)
	{
		if (WithinRange.Count == 0)
			return;


		// 押された時
		if (context.phase == InputActionPhase.Performed)
		{
			_IsInteract = true;
			float length = 10.0f;

			// プレイヤーに一番近いオブジェクトをインタラクト対象とする
			foreach (Collider obj in WithinRange)
			{
				float distance = Vector3.Distance(transform.position, obj.transform.position);

				if (length > distance)
				{
					length = distance;
					InteractObject = obj;
				}
			}

			switch (InteractObject.tag)
			{
				case "holdObject":
					OnHold(true);
					break;

				case "HitObject":
					OnHit();
					break;

				default:
					break;
			}

		}
		else if (HoldObjectRb != null)
		{
			switch (HoldObjectRb.tag)
			{
				case "holdObject":
					OnHold(false);
					break;

				default:
					break;
			}
		}

	}

	/// <summary>
	/// つかむ
	/// </summary>
	private void OnHold(bool isHold)
	{
		// 掴む処理
		if (isHold)
		{
			InteractObject.transform.parent = transform;
			InteractObject.transform.localPosition = new Vector3(0, 0.5f, InteractObject.transform.localPosition.z);
			InteractObject.transform.localRotation = Quaternion.identity;
			if (InteractObject.TryGetComponent(out Rigidbody rigidbody))
			{
				HoldObjectRb = rigidbody;
				HoldObjectRb.useGravity = false;
				HoldObjectRb.isKinematic = true;
			}
		}

		// 離す処理
		else
		{
			InteractObject.transform.parent = null;
			HoldObjectRb.useGravity = true;
			HoldObjectRb.isKinematic = false;
			InteractObject = null;
			HoldObjectRb = null;
		}
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit()
	{
		//Debug.Log($"はたく");
	}

	/// <summary>
	/// アピール
	/// </summary>
	private void OnAppeal(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				Debug.Log("アピール");
				isAppeal = true;
				break;
			case InputActionPhase.Canceled:
				Debug.Log("アピール終了");
				isAppeal = false;
				break;
		}
		anim.SetBool("Appeal", isAppeal);
	}

	/// <summary>
	/// 鳴く
	/// </summary>
	private void OnCall()
	{
		if (seCall != null)
			audioSource.PlayOneShot(seCall);
		else
			Debug.LogError($"seCallが定義されていません。");

	}

	#region 衝突判定
	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		if (InteractObject == collision.collider)
			collision.transform.localPosition += Vector3.forward / 10;
	}

	private void OnTriggerEnter(Collider other)
	{
		WithinRange.Add(other);
	}

	private void OnTriggerExit(Collider other)
	{
		WithinRange.Remove(other);
	}
	#endregion

	public void ReStart()
	{
		// インスペクターで設定したリスポーン位置に再配置する
		this.gameObject.transform.position = respawnZone.position;
	}
}
