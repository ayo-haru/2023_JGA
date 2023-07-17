using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SystemManager : MonoBehaviour
{
    [Header("---�g�p����@�\��I��---")]
    [Header("��")]
    [SerializeField]
    private ReactiveProperty<bool> useSound = new ReactiveProperty<bool>(false);
    private GameObject soundManagerObj;

    [Header("�t�F�[�h")]
    [SerializeField]
    private ReactiveProperty<bool> useFade = new ReactiveProperty<bool>(false);
    private GameObject fadeManagerObj;

    [Header("�|�[�Y")]
    [SerializeField]
    private ReactiveProperty<bool> usePause = new ReactiveProperty<bool>(false);
    private GameObject pauseManagerObj;

    [Header("UI")]
    [SerializeField]
    private ReactiveProperty<bool> useUI = new ReactiveProperty<bool>(false);
    private GameObject uiManagerObj;

    PrefabContainer managerDatas;

    private void Awake() {
        managerDatas = AddressableLoader<PrefabContainer>.Load("ManagerObjData");

        soundManagerObj = PrefabContainerFinder.Find(ref managerDatas, "SoundManager");
        fadeManagerObj = PrefabContainerFinder.Find(ref managerDatas, "FadeManager");
        pauseManagerObj = PrefabContainerFinder.Find(ref managerDatas, "PauseManager");
        uiManagerObj = PrefabContainerFinder.Find(ref managerDatas, "UIManager");

        //----- �C�x���g�o�^ -----
        // ��
        useSound.Subscribe(_ => { if (useSound.Value) { InstatiateManager(soundManagerObj); } }).AddTo(this);
        // �t�F�[�h
        useFade.Subscribe(_ => { if (useFade.Value) { /*InstatiateManager(FadeManager);*/ } }).AddTo(this);
        // �|�[�Y
        usePause.Subscribe(_ => { if (usePause.Value) { InstatiateManager(pauseManagerObj); } }).AddTo(this);
        // UI
        useUI.Subscribe(_ => { if (useUI.Value) { InstatiateManager(uiManagerObj); } }).AddTo(this);
    }

    private void OnApplicationQuit() {
        Addressables.Release(managerDatas);
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
