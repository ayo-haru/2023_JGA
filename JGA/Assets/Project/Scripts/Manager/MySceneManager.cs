//=============================================================================
// @File	: [MySceneManager]
// @Brief	: オリジナルのシーンマネージャー
// @Author	: Yoshihara Asuka
// @Editer	: Ichida Mai
// @Detail  : 
// 
// [Date]
// 2023/02/02 スクリプト作成,フレームレート数を指定の処理を記載(吉原)
// 2023/05/25 飼育員巡回ルートを列挙に追加(吉原)
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
		public static ZooKeeperData[] zooKeeperData;
		public static GuestData[] guestData;

		//---フラグ
		public static bool isCatchPenguin;

		//---データ
		public static int randomGuestCnt;   // ランダム生成させる客の今の数
		public static int oldScene;			// ひとつ前のシーン番号
		public static int nowScene;         // 現在のシーン番号
		public static EventParam[] events;  // 各シーンのイベント
		public static int guestCnt;         // 客のカウント
		public static float timer;			// ゲームのタイマー 
		public static Vector3 playerPos;    // プレイヤーの座標
		public static bool isContinueGame;	// ゲーム続きからやってる？セーブデータがあるならture
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

	public static class Volume
	{
		public static GameVolume GameVolumeDatas;
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
		// 客用
		PENGUIN_N = 0,		// ペンギンブース北
		PENGUIN_S,			// ペンギンブース南
		PENGUIN_W,			// ペンギンブース西
		PENGUIN_E,			// ペンギンブース東
		RESTSPOT_01,		// 休憩スペース1
		RESTSPOT_02,		// 休憩スペース2
		HORSE_01,			// ウマ1
		HORSE_02,			// ウマ2
		HORSE_03,			// ウマ3
		ZEBRA_01,			// シマウマ1
		ZEBRA_02,			// シマウマ2
		ZEBRA_03,			// シマウマ3
		POLARBEAR,			// シロクマ
		BEAR_01,			// クマ1
		BEAR_02,			// クマ2
		PANDA,				// パンダ


		ENTRANCE,

		// 飼育員巡回用
		BELL_AREA,			// 鐘周辺
		POLAR_AREA,			// シロクマ周辺
		BEAR_AREA,			// クマ周辺
		PANDA_AREA_01,		// パンダ周辺
		PANDA_AREA_02,		// パンダ周辺
		HOURSE_AREA,		// ウマ
		ZEBRA_AREA_01,		// シマウマ周辺1
		ZEBRA_AREA_02,		// シマウマ周辺2
		FOUNTAIN_AREA,		// 噴水周辺
		LAKE_AREA,			// 池周辺


		
		


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
		//---音量
		Volume.GameVolumeDatas = AddressableLoader<GameVolume>.Load("GameVolume");
		//---データ
		GameData.zooKeeperData = new ZooKeeperData[2];
		GameData.zooKeeperData[(int)SceneState.SCENE_GAME_001-1] = AddressableLoader<ZooKeeperData>.Load("Stage01_ZooKeeperData");
		GameData.zooKeeperData[(int)SceneState.SCENE_GAME_002-1] = AddressableLoader<ZooKeeperData>.Load("Stage02_ZooKeeperData");
		GameData.guestData = new GuestData[2];
		GameData.guestData[(int)SceneState.SCENE_GAME_001-1] = AddressableLoader<GuestData>.Load("Stage01_GuestData");
		GameData.guestData[(int)SceneState.SCENE_GAME_002-1] = AddressableLoader<GuestData>.Load("Stage02_GuestData");

		//----- 変数初期化 -----
		GameData.isCatchPenguin = false;
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
		// 現在のSceneを取得
		Scene loadScene = SceneManager.GetActiveScene();
		// 現在のシーンを再読み込みする
		SceneManager.LoadScene(loadScene.name);
	}
}
