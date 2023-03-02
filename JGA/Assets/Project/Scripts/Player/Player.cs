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


	[SerializeField] private bool isHold;       // つかみフラグ
	[SerializeField] private bool isRun;        // 走りフラグ

	private MyContorller gameInputs;            // キー入力
	private Vector2 moveInputValue;             // 移動方向

	private Collider HoldObject;                // 掴んでいるオブジェクト：コリジョン
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
		Move();

	}

	/// <summary>
	/// 移動処理
	/// </summary>
	private void Move()
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

	/// <summary>
	/// 移動方向取得
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
		if (WithinRange.Count == 0)
			return;

		// 押された時
		if (context.phase == InputActionPhase.Performed)
		{
			isHold = !isHold;

			// 掴む処理
			if (isHold)
			{
				float length = 10.0f;

				// キャッチできる範囲内にある複数のオブジェクトから
				// 一番近いオブジェクトをキャッチする
				foreach (Collider obj in WithinRange)
				{
					float distance = Vector3.Distance(transform.position, obj.transform.position);

					if (length > distance)
					{
						length = distance;
						HoldObject = obj;
					}
				}

				HoldObject.transform.parent = transform;
				HoldObject.transform.localPosition = new Vector3(0, 0, HoldObject.transform.localPosition.z);
				HoldObject.transform.localRotation = Quaternion.identity;
				if (HoldObject.TryGetComponent(out Rigidbody rigidbody))
				{
					HoldObjectRb = rigidbody;
					HoldObjectRb.useGravity = false;
					HoldObjectRb.isKinematic = true;
				}
			}

			// 離す処理
			else
			{
				HoldObject.transform.parent = null;
				HoldObjectRb.useGravity = true;
				HoldObjectRb.isKinematic = false;
				HoldObject = null;
				HoldObjectRb = null;
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



	private void OnCollisionStay(Collision collision)
	{
		// Playerと掴んでいるオブジェクトが接触していると、ぶっ飛ぶので離す
		if (HoldObject == collision.collider)
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






}
