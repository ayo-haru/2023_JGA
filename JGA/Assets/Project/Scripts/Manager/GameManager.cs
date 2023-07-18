//=============================================================================
// @File	: [TestGameManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager> 
{
	/* ===============インスペクター公開================ */
	[Header("開始するシーンを選択してください。")]
	[SerializeField]
	private bool TitleScene = true;

	[SerializeField]
	private bool Stage001 = false;

	[SerializeField]
	private bool Stage002 = false;



	[Header("生成したいオブジェクトにチェックを入れてください。")]

	[SerializeField]
	private bool isCreateStage = false;

	[SerializeField]
	private bool isCreatePlayer = false;

	[SerializeField]
	private bool isCreateEvent = false;

	[SerializeField]
	private bool isCreateGuest = false;

	[SerializeField]
	private bool isCreateGimickObject = false;

	//===============================================

	// ---各種マネージャー参照
	private GameObject stageManager;
	private GameObject eventManager;
	private GameObject guestManager;
	private GameObject gimickObjectManaager;

	//---時間UI
	private GameObject timerUI;
	private TimerSliderUI _TimerUI;

	//---客人数UI
	private GameObject guestNumUI;
	private GuestNumUI _GuestNumUI;

	//-----クリア画面
	private GameObject clearPanel;
	private ClearPanel _ClearPanel;
	private ReactiveProperty<bool> isClear = new ReactiveProperty<bool>(false);

	//-----ゲームオーバー画面
	private GameObject gameOverPanel;
	private GameOverPanel _GameOverPanel;
	private ReactiveProperty<bool> isGameOver = new ReactiveProperty<bool>(false);

	//------ プレイヤー
	private GameObject playerObj;
	private GameObject playerInstance;
	private GameObject playerRespawn;


	//---変数
	private bool isOnce; // 一度だけ処理をするときに使う
	AudioSource[] asList;
	public string[] LoadSceneName;



	protected override void Awake() 
	{
		base.Awake();

		//----- イベント登録 -----
		// クリア
		isClear.Subscribe(_ => { if (isClear.Value) OnClear(); }).AddTo(this);
		// ゲームオーバー
		isGameOver.Subscribe(_ => { if (isGameOver.Value) OnGameOver(); }).AddTo(this);


		//----- マネージャー読み込み -----
		// 以下各オブジェクト・マネージャーを取得
		gimickObjectManaager = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "GimickObjectManager");
		guestManager = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "GuestManager");
		eventManager = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "EventManager");
		stageManager = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "StageSceneManager");

		StartScene();

		//----- ゲーム初期化 -----
#if UNITY_EDITOR
		if (GameData.nowScene == 0) { // 本来ならnowSceneneには現在のシーン番号が入るがエディタ上で実行した場合は0が入っているので初期化
			for (int i = 0; i < System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length; i++) {
				if (SceneManager.GetActiveScene().name == MySceneManager.sceneName[i]) {
					GameData.nowScene = i;
				}
			}
			GameData.oldScene = GameData.nowScene;
		}
#endif
		if (GameData.isContinueGame && GameData.oldScene == (int)MySceneManager.SceneState.SCENE_TITLE) {
			/*
			 * セーブデータが存在または初期化済みでタイトルシーンを通ってきた場合つづきから
			 */
		} else {
			BeginGame();
		}

		//----- 変数初期化 -----
		isOnce = false;
		// BGM再生用にオーディオソース取得
		asList = GetComponents<AudioSource>();
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	private void Start() 
	{

		/* ※呼び出す順番に注意！*/
		//CreateStageManager();
		//CreatPlayer();
		//CreateEventManager();
		//CreateGuestManager();
		//CreateGimickObjectManager();


		//----- タイマーUIの取得 -----
		//timerUI = UIManager.UIManagerInstance.TimerUIObj;
		timerUI = GameObject.Find("TimerSlider");
		if (timerUI) {
			_TimerUI = timerUI.GetComponent<TimerSliderUI>();

			_TimerUI.CountStart();
		} else {
			Debug.LogWarning("TimerUIがシーン上にありません");
		}

		//----- 客人数カウントUIの取得 -----
		//guestNumUI = UIManager.UIManagerInstance.TimerUIObj;
		guestNumUI = GameObject.Find("GuestNumText");
		if (guestNumUI) {
			_GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
		} else {
			Debug.LogWarning("GuestNumUIがシーン上にありません");
		}

		//----- 音の処理 -----
		// BGM再生
		SoundManager.Play(asList[0], SoundManager.EBGM.GAME001);
		SoundManager.Play(asList[1], SoundManager.EBGM.INZOO);
		// フェード中だったら待機して音を止める
		StartCoroutine(WaitFade());

		//----- セーブ -----
		SaveManager.SaveAll();  // 一旦全セーブ

		//----- プレイヤー生成 -----
		CreatPlayer();
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	private void Update() 
	{

		if(!TestMySceneManager.CheckLoadScene("Title")){
			/*
			 *TODO
			 *プレイヤーのマネージャー？からプレイヤーの座標を取得して保存するか
			 *プレイヤーが保存するようにするかとかのいずれかでプレイヤーの座標をセーブする
			 */
			//-----プレイヤーの座標保存---- -
			GameData.playerPos = playerInstance.transform.position;
			SaveSystem.SaveLastPlayerPos(GameData.playerPos);
			Debug.Log("<color=#00AEEF>プレイヤー生成をしました。</color>");
		}


		//----- 時間系の処理 -----
		if (timerUI) {
			isGameOver.Value = _TimerUI.IsFinish();
		}

		//----- ゲームクリア -----
		if (guestNumUI) {
			isClear.Value = _GuestNumUI.isClear();
		}

		if (isClear.Value) {
			if (!isOnce) {   // 一度だけ処理
							 //　クリア画面取得
				if (!clearPanel) clearPanel = GameObject.Find("ClearPanel");
				if (clearPanel && !_ClearPanel) _ClearPanel = clearPanel.GetComponent<ClearPanel>();

				int next = -1;
				if (_ClearPanel) next = _ClearPanel.GetNextScene();
				if (next != -1) {
					GameData.oldScene = GameData.nowScene;  // 今のシーンをひとつ前のシーンとして保存
					GameData.nowScene = next;
					//SceneChange(next);  // シーン遷移
					isOnce = true;

					SaveManager.SaveAll();
				}
			}
		}

		//----- 制限時間のゲームオーバー -----
		if (isGameOver.Value) {
			if (!isOnce) {
				// ゲームオーバー画面取得
				if (!gameOverPanel) gameOverPanel = GameObject.Find("GameOverPanel");
				if (gameOverPanel && !_GameOverPanel) _GameOverPanel = gameOverPanel.GetComponent<GameOverPanel>();

				int next = -1;
				if (_GameOverPanel) next = _GameOverPanel.GetNextScene();
				if (next != -1) {
					GameData.oldScene = GameData.nowScene;  // 今のシーンをひとつ前のシーンとして保存
					GameData.nowScene = next;
					//SceneChange(next);  // シーン遷移
					isOnce = true;
				}
			}
		}
	}

	private void LateUpdate() 
	{
		//----- 飼育員に捕まったフラグを下す -----
		/*
		 * Updateで下ろすと各処理と同フレーム中にフラグが降りてしまうためLateに書いた
		 */
		if (GameData.isCatchPenguin) {
			GameData.isCatchPenguin = false;
		}
	}


	/// <summary>
	/// ギミックマネージャー生成処理
	/// </summary>
	private void CreateGimickObjectManager() 
	{
		if (!isCreateGimickObject) {
			return;
		} else if (!gimickObjectManaager) {
			Debug.Log(gimickObjectManaager.name + "は見つかりませんでした.");
			return;
		} else {
			InstantiateWithoutClone(gimickObjectManaager);
		}

	}

	/// <summary>
	/// プレイヤー生成
	/// </summary>
	private void CreatPlayer()
	{
		if (isCreatePlayer)
		{
			if (!TestMySceneManager.CheckLoadScene("Title")) {
				playerRespawn = GameObject.Find("PlayerSpawn");
				GameData.playerPos = playerRespawn.transform.position;

				playerObj = PrefabContainerFinder.Find(ref GameData.characterDatas, "Player.prefab");
				playerInstance = Instantiate(playerObj, GameData.playerPos, Quaternion.Euler(0.0f, 180.0f, 0.0f));
				playerInstance.name = "Player";
			}
		}
	}

	/// <summary>
	/// 客マネージャー生成
	/// </summary>
	private void CreateGuestManager() 
	{
		if (!isCreateGuest) {
			return;
		} 
		else if (!guestManager) {
			Debug.Log(guestManager.name + "は見つかりませんでした.");
			return;
		} 
		else InstantiateWithoutClone(gimickObjectManaager);
	}

	/// <summary>
	/// イベントマネージャ-生成
	/// </summary>
	private void CreateEventManager() 
	{
		if (!isCreateEvent) {
			return;
		} 
		else InstantiateWithoutClone(eventManager);
	}

	private void CreateStageManager()
	{
		if (!isCreateStage){
			return;
		}
		else if (!stageManager){
			Debug.Log(stageManager.name + "は見つかりませんでした.");
			return;
		}
		else{
			InstantiateWithoutClone(stageManager);
		}
	}
	/// <summary>
	/// 生成する際にCloneがつくのを回避する処理
	/// </summary>
	/// <param name="original"></param>
	/// <returns></returns>
	public static GameObject InstantiateWithoutClone(UnityEngine.Object original) {
		var result = Instantiate(original);
		result.name = result.name.Replace("(Clone)", "");
		return (GameObject)result;
	}

	/// <summary>
	/// ステージを初めの状態からやる
	/// </summary>
	private void BeginGame() {
		GameData.guestCnt = 0;
		GameData.timer = 0.0f;
		//playerRespawn = GameObject.Find("PlayerSpawn");
		//GameData.playerPos = playerRespawn.transform.position;

		// 一個前のシーンがタイトルかつ今のシーンがステージ１
		if (GameData.oldScene == (int)MySceneManager.SceneState.SCENE_TITLE &&
			GameData.nowScene == (int)MySceneManager.SceneState.SCENE_GAME_001) {
			TutorialManager.StartTutorial();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IEnumerator WaitFade() {
		asList[0].Pause();
		asList[1].Pause();
		yield return new WaitUntil(() => FadeManager.fadeMode == FadeManager.eFade.Default);
		asList[0].UnPause();
		asList[1].UnPause();
	}

	/// <summary>
	/// クリアになった瞬間にやる処理
	/// </summary>
	private void OnClear() {
		// ポーズ
		if (!PauseManager.isPaused) {
			PauseManager.isPaused = true;
			PauseManager.NoMenu = true;
			PauseManager.Pause();
		}

		//----- 音の再生 -----
		// ポーズ後にやらないとポーズに消される
		SoundManager.Play(asList[2], SoundManager.ESE.GAMECLEAR);

		// セーブ
		SaveManager.SaveGuestCnt(0);
		SaveManager.SaveTimer(0.0f);
		if (System.Enum.GetNames(typeof(MySceneManager.SceneState)).Length > GameData.nowScene) {  // 最大シーンではないとき
			SaveManager.SaveLastStageNum(GameData.nowScene + 1);
		} else {
			SaveManager.SaveLastStageNum(GameData.nowScene);
		}
		SaveManager.SaveLastPlayerPos(GameData.playerPos);
	}

	/// <summary>
	/// ゲームオーバーになったらやる処理
	/// </summary>
	private void OnGameOver() {
		// ポーズ
		if (!PauseManager.isPaused) {
			PauseManager.isPaused = true;
			PauseManager.NoMenu = true;
			PauseManager.Pause();
		}

		//----- 音の再生 -----
		// ポーズ後にやらないとポーズに消される
		SoundManager.Play(asList[2], SoundManager.ESE.GAMEOVER);

		//----- セーブ -----
		// 初期値の値で保存。シーン番号は現在のシーン。
		SaveManager.SaveInitDataAll();
	}

	private void StartScene()
	{
		if (TitleScene)
		{
			StartTitle();
		}

		if(Stage001)
		{
			StartStage001();
		}

		if (Stage002)
		{
			StartStage002();
		}

	}
	private void StartTitle()
	{
		TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TITLE);
	}

	private void StartStage001()
	{
		TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_GAME_001);

		CreateEventManager();
		CreateGuestManager();
		CreateGimickObjectManager();
	}

	private void StartStage002()
	{
		TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_GAME_002);

		CreateEventManager();
		CreateGuestManager();
		CreateGimickObjectManager();
	}


}


