//=============================================================================
// @File	: [UIManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UIManager : MonoBehaviour,IManagerCommon
{
	private static UIManager instance;

	public static UIManager UIManagerInstance { get { return instance; } }

	//---キャンバス
	private GameObject canvasObj;
	private Canvas canvas;
	public Canvas CanvasObj { get { return canvas; } }

	//---カメラ
	private Camera resultCamera;
	private ResultCamera _ResultCamera;

	//---フェード
	private GameObject fade;
	public GameObject FadeObj { get { return fade; } }

	//---ポーズ
	private GameObject pause;
	public GameObject PauseObj { get { return pause; } }

	//---クリア
	private GameObject clearUI;
	public GameObject ClearUIObj { get { return clearUI; } }

	//---ゲームオーバー
	private GameObject failedUI;
	public GameObject FailedUIObj { get { return failedUI; } }

	//---操作方法
	private GameObject operationUI;
	public GameObject OperationUIObj { get { return operationUI; } }

	//---時間UI
	private GameObject timerUI;
	public GameObject TimerUIObj { get{ return timerUI; } }
	private TimerSliderUI _TimerUI;
	[Header("----- 時間UI設定項目 -----")]
	[Header("プレイ時間")]
	[Range(1, 10)] public int playMinutes;
	[Header("ロス時間")]
	[Tooltip("実プレイ時間の何％分減らしたいか指定してください")]
	[Range(1, 100)] 
	public int lossSecondsParcent = 10;
	[Header("残り何秒で音を鳴らすか")]
	[Range(1, 60)]
	public int soundSeconds = 10;

	//---客人数UI
	private static GameObject guestNumUI;
	private GuestNumUI _GuestNumUI;
	public GameObject GuestNumUI { get { return guestNumUI; } }
	[Header("----- 客カウントUI設定項目 -----")]
	//目標人数
	[Range(1, 99)] public int clearNum = 10;
	//アニメーション時間
	[Range(0, 1)] public float animTime = 0.5f;
	//拡大率
	[Range(1, 2)] public float scaleValue = 1.5f;

	//---チュートリアル
	private TutorialManager _TutorialManager;

	private void Awake() {
		AsyncOperation asyncOperation = TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TESTUI);

		StartCoroutine(WaitAddScene(asyncOperation));
	}

	void Start()
	{

		if(!TestMySceneManager.CheckLoadScene("Title")){
			InitGameUI();
		}
	}

	void Update() {
		//if (GameData.nowScene != (int)MySceneManager.SceneState.SCENE_TITLE) {


		if (!TestMySceneManager.CheckLoadScene("Title"))
		{
		UpdateGameUI();
		}

		//}
	}

	/// <summary>
	/// シーン加算完了待ち
	/// </summary>
	/// <param name="_asyncOperation"></param>
	/// <returns></returns>
	public IEnumerator WaitAddScene(AsyncOperation _asyncOperation) {
		yield return new WaitUntil(() => _asyncOperation.isDone == true);
	}

	/// <summary>
	/// ゲームUIの初期化
	/// </summary>
	private void InitGameUI() {
		#region
		//----- ポーズの取得 -----
		pause = GameObject.Find("Pause");

		//----- 操作方法のUIの取得 -----
		operationUI = GameObject.Find("OperationUI");

		//----- クリアのUIの取得 ------
		clearUI = GameObject.Find("ClearPanel");

		//----- ゲームオーバーのUI読み込み -----
		failedUI = GameObject.Find("GameOverPanel");


		//----- タイマーUIの取得 -----
		timerUI = GameObject.Find("TimerSlider");
		if (timerUI) {
			_TimerUI = timerUI.GetComponent<TimerSliderUI>();

			_TimerUI.CountStart();
		} else {
			Debug.LogWarning("TimerUIがシーン上にありません");
		}

		//----- 客人数カウントUIの取得 -----
		guestNumUI = GameObject.Find("GuestNumText");
		if (guestNumUI) {
			_GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
		} else {
			Debug.LogWarning("GuestNumUIがシーン上にありません");
		}

		//----- リザルトカメラの取得 -----
		//GameObject _cameraManagerObj;
		//_cameraManagerObj = GameObject.Find("CameraManager");
		//if (_cameraManagerObj) {
		//    _ResultCamera = _cameraManagerObj.GetComponent<ResultCamera>();
		//} else {
		//    Debug.LogWarning("CameraManagerがシーン上にありません");
		//}
		//resultCamera = GameObject.Find("ResultCamera").GetComponent<Camera>();

		//----- チュートリアルマネージャーの取得 -----
		if (GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
			GameObject _tutotialManagerObj;
			_tutotialManagerObj = GameObject.Find("TutorialManager");
			if (_tutotialManagerObj) {
				_TutorialManager = _tutotialManagerObj.GetComponent<TutorialManager>();
			}
		}
	#endregion
	}

	private void UpdateGameUI() {
		//----- ゲームクリア -----
		if (guestNumUI) {
			if (_GuestNumUI.isClear()) {
				canvas.worldCamera = resultCamera;
				if (_ResultCamera.rotateFlg) {
					clearUI.SetActive(true);
				}
			} else {
				clearUI.SetActive(false);
			}
		}

		//----- ゲームオーバー -----
		if (timerUI) {
			if (_TimerUI.IsFinish()) {
				failedUI.SetActive(true);
			} else {
				failedUI.SetActive(false);
			}
		}

		//-----  チュートリアル中かで変わるUI -----
		if (GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
			// タイマーの表示非表示
			if (timerUI) {
				var currentTutorialTask = _TutorialManager.GetCurrentTask();
				switch (currentTutorialTask) {
					case TutorialTask001:           // チュートリアル開始時
						_TimerUI.CountStop();
						timerUI.SetActive(false);
						break;
					case TutorialTask028:           // タイマーチュートリアル開始
						timerUI.SetActive(true);
						break;
					case TutorialTask032:           // チュートリアル終了
						_TimerUI.CountStart();
						break;
					default:
						break;
				}
			}
		}
	}
}
