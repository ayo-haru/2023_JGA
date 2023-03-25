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

		//---データの登録
		public static ZooKeeperData zooKeeperData;

		//---フラグ
		public static bool isCatchPenguin;
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
		SCENE_GAME,
	};

	private static string[] sceneName = {   // シーン名(実際に作ったシーンの名前入れてね)
		"Title",
		"ProtoType",
		"Stage_001"	// 海川君のステージ出来たらこっちにいろいろうつして、ProtoTypeのシーンとはさよなら
	};

	//----- 飼育員、客のルートに使用 -----
	public enum eRoot
	{
		NONE = 0,
		PENGUIN,
		BEAR,
		ELEPHANT,
		LION,
		POLARBEAR,
		BIRD,

		OTHER
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
		//---サウンド
		Sound.BGMDatas = AddressableLoader<SoundData>.Load("BGMData");
		Sound.SEDatas = AddressableLoader<SoundData>.Load("SEData");
		//---エフェクト
		Effect.effectDatas = AddressableLoader<EffectData>.Load("EffectData");
		//---データ
		GameData.zooKeeperData = AddressableLoader<ZooKeeperData>.Load("ZooKeeperData");

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
