//=============================================================================
// @File	: [BaseObj.cs]
// @Brief	: ギミックオブジェクトのベース
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/29	スクリプト作成
// 2023/04/04   OutLineスクリプトを追加するようにattributeつけました(吉原)
// 2023/04/18   ポーズ処理変更
// 2023/05/12   改良しようとしている。
// 2023/06/16   距離のステートを削除(使わなそう),初期値記載
// 2023/06/28   多分、破壊されたときにリストから削除しつつ破壊する処理できた…？
//              GimickObjectManagerが無い時のエラー処理を色々記載。
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]
public class BaseObj : MonoBehaviour, IPlayObjectSound
{
	//----- オブジェクトのステート -----
	public enum ObjType
	{
		NONE		= 0,    // 通常
		HIT			= 1,    // 叩く
		HOLD		= 2,    // 掴む
		DRAG		= 3,    // ひきずる 
		HIT_HOLD	= 4,    // 叩く&掴む
		HIT_DRAG	= 5,    // 叩く&ひきずる
		RETURN		= 6,    // 元に戻す(飼育員)
	}

	/* ========== 共通変数宣言 ===========*/
	protected GimickObjectManager gimickObjectManager = null;	// ギミックオブジェクトマネージャー
	protected Rigidbody rb = null;                              // リジッドボディ使用

	//protected AudioSource audioSource;						// オーディオソース

	// オーディオソースをリストで格納
	[SerializeField]
	protected List<AudioSource> audioSourcesList = new List<AudioSource>();

	protected Player player = null;								// プレイヤー取得
	[SerializeField] public ObjType objType;					// オブジェクトのタイプ
	protected bool isPlaySound = false;							// 音が鳴っているか

	public UnityEvent OnDestroyed = new UnityEvent();
	//===================================


	//--- ポーズ用変数 ---
	protected Vector3 pauseVelocity = Vector3.zero;
	protected Vector3 pauseAngleVelocity = Vector3.zero;
	//------------------



	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	protected virtual void Awake()
	{
		/*
		 このクラスを継承した物は必ず、ポーズの処理を登録を行いため、ここに記載
		*/
		PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
		PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);



	}

	/// <summary>
	/// オブジェクトが有効化された直後に呼び出される
	/// </summary>
	protected virtual void OnEnable()
	{
		// GimickObjectManagerのコンポーネントを検索
		gimickObjectManager = FindObjectOfType<GimickObjectManager>();

		if (gimickObjectManager == null)
		{
			Destroy(this.gameObject);
		}
		else
		{
			GimickObjectManager.Instance.AddGimickObjectsList(this);
			Debug.Log("<color=#00AEEF>" + this.gameObject.name + "をリストに追加しました.</color>");
		}
	}

	/// <summary>
	/// Behaviourが無効になった直後に呼び出される
	/// </summary>
	protected virtual void OnDisable()
	{
		Uninit();
	}

	protected virtual void OnDestroy()
	{
		OnDestroyed.Invoke();
	}

	#region 当たり判定処理===============================================

	protected virtual void OnCollisionEnter(Collision collision) 
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null){
			Debug.LogError("<color=#fd7e00>GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if(PauseManager.isPaused) return;
	}

	protected virtual void OnCollisionStay(Collision collision) 
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null)
		{
			Debug.LogError("<color=#fd7e00>GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if (PauseManager.isPaused) return;

	}

	protected virtual void OnCollisionExit(Collision collision)
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null)
		{
			Debug.LogError("<color=#fd7e00>" + this.gameObject.name + "の処理.GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if (PauseManager.isPaused) return;

	}

	protected virtual void OnTriggerEnter(Collider other) 
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null)
		{
			Debug.LogError("<color=#fd7e00>" + this.gameObject.name + "GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if (PauseManager.isPaused) return;

	}

	protected virtual void OnTriggerStay(Collider other) 
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null)
		{
			Debug.LogError("<color=#fd7e00>" + this.gameObject.name + "GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if (PauseManager.isPaused) return;

	}

	protected virtual void OnTriggerExit(Collider other) 
	{
		/* GimickObjectManagerがない場合は処理を行わない */
		if (gimickObjectManager == null)
		{
			Debug.LogError("<color=#fd7e00>" + this.gameObject.name + "GimickObjectManagerがありません</color>");
			return;
		}

		/* ポーズフラグ中は処理を行わない。 */
		if (PauseManager.isPaused) return;

	}
#endregion


	/// <summary>
	/// 初期化処理を行う
	/// </summary>
	protected void Init()
	{
		rb  = GetComponent<Rigidbody>();
		GetComponents<AudioSource>(audioSourcesList);
		objType = ObjType.NONE;
		isPlaySound = false;
		
	}

	/// <summary>
	/// オブジェクトのタイプを変更する初期化関数
	/// </summary>
	/// <param name="objType">オブジェクトのステート</param>
	protected void Init(ObjType Type)
	{
		rb = GetComponent<Rigidbody>();
		GetComponents<AudioSource>(audioSourcesList);
		objType = Type;
		isPlaySound = false;

	}

	
	/// <summary>
	/// 終了処理を行う(ここではオブジェクトが破壊された時等に行いたい処理を記載)
	/// </summary>
	protected void Uninit() 
	{
		this.OnDestroyed.AddListener(() => 
		{ 
			Debug.Log("<color=#00AEEF>" + this.gameObject.name + "破壊されました</color>");
			if (gimickObjectManager){
				GimickObjectManager.Instance.RemoveGimickObjectsList(this);
			}
		});
	}

	public virtual void OnStart() { }
	public virtual void OnUpdate() { }

	/// <summary>
	/// 引数無しの場合はリストの一番最初にあるオーディオソースを判定
	/// </summary>
	protected void PlaySoundChecker()
	{
		if (audioSourcesList[0].isPlaying) isPlaySound = true;
		else isPlaySound = false; 
	}

	/// <summary>
	/// 特定のオーディオソースを検索したいとき、リストの要素数を指定
	/// </summary>
	/// <param name="num"></param>
	protected void PlaySoundChecker(int num)
	{
		if (audioSourcesList[num].isPlaying)　isPlaySound = true;
		else　isPlaySound = false;
	}


	/// <summary>
	/// 音が鳴ってるかフラグの取得
	/// </summary>
	/// <returns></returns>
	public bool GetisPlaySound() { return isPlaySound; }


	/// <summary>
	/// ポーズの処理
	/// </summary>
	protected virtual void Pause()
	{
		/*
		  全てのオーディオソースを止める
		 */
		for(int i = 0; i < audioSourcesList.Count; i++){
			audioSourcesList[i].Pause();
		}

		pauseVelocity = rb.velocity;
		pauseAngleVelocity = rb.angularVelocity;
		//rb.isKinematic = true;
	}

	/// <summary>
	/// ポーズ解除の処理
	/// </summary>
	protected virtual void Resumed()
	{
		/*
		  全てのオーディオソースを対象
		 */
		for (int i = 0; i < audioSourcesList.Count; i++){
			audioSourcesList[i].UnPause();
		}

		// 物理挙動開始
		rb.velocity = pauseVelocity;
		rb.angularVelocity = pauseAngleVelocity;
		//rb.isKinematic = false;
	}


	// ===================== インターフェースメソッド =========================
	public void PlayHit()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.OBJECT_HIT);
	}

	public void PlayHit(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}

	public void PlayDrop()
	{
		SoundManager.Play(audioSourcesList[0], SoundManager.ESE.OBJECT_DROP);
	}

	public void PlayDrop(AudioSource audioSource, SoundManager.ESE soundNumber)
	{
		SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
	}
	//=====================================================================


}
