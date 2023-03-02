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
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
	[SerializeField] private Rigidbody rb;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private AudioClip seCall;

	[Header("ステータス")]
	[SerializeField, Tooltip("追加速度")]
	private float moveForce = 7;
	[SerializeField, Tooltip("走る際の倍率")]
	private float runMagnification = 1.5f;
	[SerializeField, Tooltip("最高速度")]
	private float maxSpeed = 5;


	[SerializeField] private bool isHold;
	[SerializeField] private bool isRun;

	private MyContorller gameInputs;
	private Vector2 moveInputValue;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		if (rb == null)
			rb = GetComponent<Rigidbody>();

		// 回転固定
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();

		// seCallの音量クソでかいので小さくする
		audioSource.volume = 0.2f;

		// Input Actionインスタンス生成
		gameInputs = new MyContorller();

		// Actionイベント登録
		gameInputs.Player.Move.performed += OnMove;
		gameInputs.Player.Move.canceled += OnMove;
		gameInputs.Player.Hit.performed += OnHit;
		gameInputs.Player.Hold.performed += OnHold;
		gameInputs.Player.Hold.canceled += OnHold;
		gameInputs.Player.Run.performed += OnRun;
		gameInputs.Player.Run.canceled += OnRun;
		gameInputs.Player.Call.performed += OnCall;

		// Input Actionを有効化
		gameInputs.Enable();
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	private void FixedUpdate()
	{
		// 制限速度内の場合、移動方向の力を与える
		if (rb.velocity.magnitude < maxSpeed * (isRun ? runMagnification : 1))
			//rb.AddForce(new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1));
			rb.velocity = new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1);

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

	private void Update()
	{
		//// 制限速度内の場合、移動方向の力を与える
		//if (rb.velocity.magnitude < maxSpeed * (isRun ? runMagnification : 1))
		//	rb.velocity = new Vector3(moveInputValue.x, 0, moveInputValue.y) * moveForce * (isRun ? runMagnification : 1);
	}

	/// <summary>
	/// 移動
	/// </summary>
	private void OnMove(InputAction.CallbackContext context)
	{
		moveInputValue = context.ReadValue<Vector2>();
	}

	/// <summary>
	/// はたく
	/// </summary>
	private void OnHit(InputAction.CallbackContext context)
	{
		Debug.Log($"はたく");
	}

	/// <summary>
	/// つかむ
	/// </summary>
	private void OnHold(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			// 押された時
			case InputActionPhase.Performed:
				isHold = !isHold;
				break;
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
	private void OnCall(InputAction.CallbackContext context)
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



}
