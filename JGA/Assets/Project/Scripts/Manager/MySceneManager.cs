//=============================================================================
// @File	: [MySceneManager]
// @Brief	: オリジナルのシーンマネージャー
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail  : 
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
//=============================================================================
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : SingletonMonoBehaviour<MySceneManager>
{
	// ゲーム内で使うデータを持つクラス
	public static class GameData
	{
		//---プレハブの登録
		public static PrefabContainer characterDatas;
		public static PrefabContainer UIDatas;
		public static PrefabContainer animalDatas;
		public static PrefabContainer gimmickDatas;

		//---データの登録
		public static ZooKeeperData zooKeeperData;
		public static GuestData guestData;

		//---フラグ
		public static bool isCatchPenguin;

		//---データ
		public static int randomGuestCnt;   // ランダム生成させる客の今の数
		public static int nowScene;         // 現在のシーン番号
		public static EventParam[] events;		// 各シーンのイベント
	}

	public static class Sound
	{
		public static SoundData BGMDatas;
		public static SoundData SEDatas;
	}

	public static class Effect
	{
		public static EffectData effectDatas;
	}

	// このクラスを持つオブジェクトは消えない
	protected override bool dontDestroyOnLoad { get { return true; } }

	//----- シーン遷移で使用 -----
	public enum SceneState
	{    // シーン定数
		SCENE_TITLE = 0,
		SCENE_GAME_001,
		SCENE_GAME_002,
	};

	public static string[] sceneName = {   // シーン名(実際に作ったシーンの名前入れてね)
		"Title",
		"Stage_001",
		"Stage_002"
	};

	//----- 飼育員、客のルートに使用 -----
	public enum eRoot
	{
		PENGUIN_N = 0,
		PENGUIN_S,
		PENGUIN_W,
		PENGUIN_E,
		HORSE,
		ELEPHANT,
		LION,
		POLARBEAR,
		BIRD,

		ENTRANCE
	}

	//----- イベント -----
	public enum eEvent {
		GUEST_ENTER,	// 客入場
		SOUND_BELL,		// 鐘
		GO_POLARBEAR,	// しろくま
		GO_HOURSE,		// うま
		OBJ_MEGAPHON,	// メガホン使え

		TIMEOUT_ALERT	// HURRYのUI 
	}

	private void Awake()
	{
		// FPSを60に固定
		Application.targetFrameRate = 60;

		//----- ScriptableObjectの登録したデータの読み込み -----
		//---オブジェクト
		GameData.characterDatas = AddressableLoader<PrefabContainer>.Load("CharacterData");
		GameData.UIDatas = AddressableLoader<PrefabContainer>.Load("UIData");
		GameData.animalDatas = AddressableLoader<PrefabContainer>.Load("AnimalData");
		GameData.gimmickDatas = AddressableLoader<PrefabContainer>.Load("GimmickData");
		//---サウンド
		Sound.BGMDatas = AddressableLoader<SoundData>.Load("BGMData");
		Sound.SEDatas = AddressableLoader<SoundData>.Load("SEData");
		//---エフェクト
		Effect.effectDatas = AddressableLoader<EffectData>.Load("EffectData");
		//---データ
		GameData.zooKeeperData = AddressableLoader<ZooKeeperData>.Load("ZooKeeperData");
		GameData.guestData = AddressableLoader<GuestData>.Load("GuestData");

		//----- 変数初期化 -----
		GameData.isCatchPenguin = false;
	}

	private void Start()
	{

	}

	/// <summary>
	/// 呼ばれたらシーン遷移する
	/// </summary>
	/// <param name="_nextScene">遷移先のシーンの定数</param>
	public static void SceneChange(SceneState _nextScene)
	{
		SceneManager.LoadScene(sceneName[(int)_nextScene]);
	}
    public static void SceneChange(int _nextScene) {
        SceneManager.LoadScene(sceneName[_nextScene]);
    }


    /// <summary>
    /// 呼ばれたらシーン読み込み直す
    /// </summary>
    public static void SceneReload()
	{
		/*
		 * 一応作ってみたんだけどInitializeSceneは再読み込みされないのでちょっとどうしようかなって感じ
		 * 本格的に使うならつくろかなーなんて
		 */


		// 現在のSceneを取得
		Scene loadScene = SceneManager.GetActiveScene();
		// 現在のシーンを再読み込みする
		SceneManager.LoadScene(loadScene.name);
	}
}
