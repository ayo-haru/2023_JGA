//=============================================================================
// @File	: [Player.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/25	スクリプト作成
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

	[Header("ステータス")]
	[SerializeField, Tooltip("追加速度")]
	private float moveForce = 7;
	[SerializeField, Tooltip("走る際の倍率")]
	private float runMagnification = 1.5f;
	[SerializeField, Tooltip("最高速度")]
	private float maxSpeed = 5;
	[SerializeField, Tooltip("ジョイスティックで走り始めるゾーン")]
	private float joyRunZone = 0.8f;


	[SerializeField] private bool isInteract;   // インタラクトフラグ
	public bool IsInteract { get; set; }        // インタラクトプロパティ

	[SerializeField] private bool isHold;       // つかみフラグ
	[SerializeField] private bool isRun;        // 走りフラグ


	[SerializeField] private bool bGamePad;                      // ゲームパッド接続確認
	private MyContorller gameInputs;            // キー入力
	private Vector2 moveInputValue;             // 移動方向

	private Collider InteractObject;            // 掴んでいるオブジェクト：コリジョン
	private Rigidbody HoldObjectRb;             // 掴んでいるオブジェクト：重力関連
	[SerializeField]
	private List<Collider> WithinRange = new List<Collider>();


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

		if (isInteract == true)
			isInteract = false;
	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
	{
		if (!bGamePad)
		{
			// 制限速度内の場合、移動方向の力を与える
			if (rb.velocity.magnitude < maxSpeed * (isRun ? runMagnification : 1))
				rb.AddForce(new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1));
			//rb.velocity = new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1);

			// 進行方向に向かって回転する
			if (moveInputValue.normalized != Vector2.zero)
			{
				// 候補1
				var awd = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 10;
				awd.y = 0;
				transform.LookAt(transform.position + awd);

				//// 候補2
				//var vael = new Vector3(moveInputValue.x, transform.position.y, moveInputValue.y) - transform.forward;
				//transform.Rotate(Vector3.up, vael.magnitude, Space.World);
			}
		}
		else
		{
			isRun = moveInputValue.magnitude >= joyRunZone;

			// 制限速度内の場合、移動方向の力を与える
			if (rb.velocity.magnitude < maxSpeed * (isRun ? runMagnification : 1))
				rb.AddForce(new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1));

			// 進行方向に向かって回転する
			if (moveInputValue.normalized != Vector2.zero)
			{
				var awd = transform.forward - new Vector3(-moveInputValue.x, transform.position.y, -moveInputValue.y) / 10;
				awd.y = 0;
				transform.LookAt(transform.position + awd);
			}
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
	/// アピール
	/// </summary>
	private void OnAppeal(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Performed:
				Debug.Log("アピール");
				break;
			case InputActionPhase.Canceled:
				Debug.Log("アピール終了");
				break;
		}
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		Debug.Log($"はたく");
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
			Debug.Log($"インタラクト");
			isInteract = true;
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

			switch (InteractObject/*.tag*/)
			{
				//case:holdObject
				//	OnHold(true);
				//break;

				// case:HitObject
				//	OnHit();
				// break;

				default:
					break;
			}

		}
		else
		{
			switch (InteractObject/*.tag*/)
			{
				//case:holdObject
				//	OnHold(false);
				//break;

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
			InteractObject.transform.localPosition = new Vector3(0, 0, InteractObject.transform.localPosition.z);
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
	/// 鳴く
	/// </summary>
	private void OnCall()
	{
		if (seCall != null)
			audioSource.PlayOneShot(seCall);
		else
			Debug.LogError($"seCallが定義されていません。");

	}



	private void OnGUI()
	{
		GUILayout.Label($"");   // imgui回避
		GUILayout.Label($"isRun:{isRun}");
		GUILayout.Label($"isHold:{isHold}");
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






}
