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
using UnityEngine.AddressableAssets;


public class MySceneManager : SingletonMonoBehaviour<MySceneManager> {
    public static class Sound {
        public static SoundData BGMDatas;
        public static SoundData SEDatas;
    }

    public static class Effect {
        public static EffectData effectDatas;
    }

    public static class Volume {
        public static GameVolume GameVolumeDatas;
    }

    // このクラスを持つオブジェクトは消えない
    //protected override bool dontDestroyOnLoad { get { return true; } }

    //----- シーン遷移で使用 -----
    public enum SceneState {    // シーン定数
        SCENE_TITLE = 0,
        SCENE_GAME_001,
        SCENE_GAME_002,
    };

    public static string[] sceneName = {   // シーン名(実際に作ったシーンの名前入れてね)
		"Title",
        "Stage_001",
        "Stage_002"
    };

    private void Awake() {
        // FPSを60に固定
        Application.targetFrameRate = 60;

        //----- ScriptableObjectの登録したデータの読み込み -----
        //---オブジェクト
        GameData.characterDatas = AddressableLoader<PrefabContainer>.Load("CharacterData");
        GameData.UIDatas = AddressableLoader<PrefabContainer>.Load("UIData");
        GameData.animalDatas = AddressableLoader<PrefabContainer>.Load("AnimalData");
        //GameData.gimmickDatas = AddressableLoader<PrefabContainer>.Load("GimmickData");
        GameData.stageObjDatas = AddressableLoader<PrefabContainer>.Load("StageObjData");
        //---サウンド
        Sound.BGMDatas = AddressableLoader<SoundData>.Load("BGMData");
        Sound.SEDatas = AddressableLoader<SoundData>.Load("SEData");
        //---エフェクト
        Effect.effectDatas = AddressableLoader<EffectData>.Load("EffectData");
        //---音量
        Volume.GameVolumeDatas = AddressableLoader<GameVolume>.Load("GameVolume");
        //---データ
        GameData.zooKeeperData = new ZooKeeperData[2];
        GameData.zooKeeperData[(int)SceneState.SCENE_GAME_001 - 1] = AddressableLoader<ZooKeeperData>.Load("Stage01_ZooKeeperData");
        GameData.zooKeeperData[(int)SceneState.SCENE_GAME_002 - 1] = AddressableLoader<ZooKeeperData>.Load("Stage02_ZooKeeperData");
        GameData.guestData = new GuestData[2];
        GameData.guestData[(int)SceneState.SCENE_GAME_001 - 1] = AddressableLoader<GuestData>.Load("Stage01_GuestData");
        GameData.guestData[(int)SceneState.SCENE_GAME_002 - 1] = AddressableLoader<GuestData>.Load("Stage02_GuestData");

        //----- 変数初期化 -----
        GameData.isCatchPenguin = false;
    }

    private void OnApplicationQuit() {
        // ロードしたアセットを解放
        Addressables.Release(GameData.characterDatas);
        Addressables.Release(GameData.UIDatas);
        Addressables.Release(GameData.animalDatas);
        Addressables.Release(GameData.stageObjDatas);
        Addressables.Release(Sound.BGMDatas);
        Addressables.Release(Sound.SEDatas);
        Addressables.Release(Effect.effectDatas);
        Addressables.Release(Volume.GameVolumeDatas);
        for (int i = 0; i < GameData.zooKeeperData.Length; i++) {
            Addressables.Release(GameData.zooKeeperData);
        }
        for (int i = 0; i < GameData.guestData.Length; i++) {
            Addressables.Release(GameData.guestData[i]);
        }
    }

    /// <summary>
    /// 呼ばれたらシーン遷移する
    /// </summary>
    /// <param name="_nextScene">遷移先のシーンの定数</param>
    public static void SceneChange(SceneState _nextScene) {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(sceneName[(int)_nextScene]);
    }
    public static void SceneChange(int _nextScene) {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(sceneName[_nextScene]);
    }


    /// <summary>
    /// 呼ばれたらシーン読み込み直す
    /// </summary>
    public static void SceneReload() {
        // 現在のSceneを取得
        Scene loadScene = SceneManager.GetActiveScene();
        // 現在のシーンを再読み込みする
        SceneManager.LoadScene(loadScene.name);
    }
}
