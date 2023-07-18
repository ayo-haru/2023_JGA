using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SystemManager : MonoBehaviour
{
    [Header("---使用する機能を選択---")]
    [Header("音")]
    [SerializeField]
    private ReactiveProperty<bool> useSound = new ReactiveProperty<bool>(false);
    private GameObject soundManagerObj;

    [Header("エフェクト")]
    [SerializeField]
    private ReactiveProperty<bool> useEffect = new ReactiveProperty<bool>(false);
    private GameObject effectManagerObj;

    [Header("フェード")]
    [SerializeField]
    private ReactiveProperty<bool> useFade = new ReactiveProperty<bool>(false);
    private GameObject fadeManagerObj;

    [Header("ポーズ")]
    [SerializeField]
    private ReactiveProperty<bool> usePause = new ReactiveProperty<bool>(false);
    private GameObject pauseManagerObj;

    [Header("UI")]
    [SerializeField]
    private ReactiveProperty<bool> useUI = new ReactiveProperty<bool>(false);
    private GameObject uiManagerObj;

    private void Awake() {
        soundManagerObj = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "SoundManager");
        effectManagerObj = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "EffectManager");
        fadeManagerObj = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "FadeManager");
        pauseManagerObj = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "PauseManager");
        uiManagerObj = PrefabContainerFinder.Find(ref GameData.managerObjDatas, "UIManager");

        //----- イベント登録 -----
        // 音
        useSound.Subscribe(_ => { if (useSound.Value) { InstatiateManager(soundManagerObj); } }).AddTo(this);
        // エフェクト
        useEffect.Subscribe(_ => { if (useEffect.Value){ InstatiateManager(effectManagerObj);}}).AddTo(this);        
        // フェード
        useFade.Subscribe(_ => { if (useFade.Value) { InstatiateManager(fadeManagerObj); } }).AddTo(this);
        // ポーズ
        usePause.Subscribe(_ => { if (usePause.Value) { InstatiateManager(pauseManagerObj); } }).AddTo(this);
        // UI
        useUI.Subscribe(_ => { if (useUI.Value) { InstatiateManager(uiManagerObj); } }).AddTo(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (useSound.Value) {
        //    gameObject.AddComponent<SoundManager>();
        //}
        //if (useFade.Value) {
        //    //gameObject.AddComponent<FadeManager>();
        //}
        //if (usePause.Value) {
        //    gameObject.AddComponent<PauseManager>();
        //}
        //if (useUI.Value) {
        //    gameObject.AddComponent<UIManager>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstatiateManager(GameObject _managerObj) {
        GameObject _work;

        _work = Instantiate(_managerObj, gameObject.transform);
        _work.name = _managerObj.name;
    }
}
