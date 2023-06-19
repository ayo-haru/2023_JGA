using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [Header("---使用する機能を選択---")]
    [Header("音")]
    [SerializeField]
    private ReactiveProperty<bool> useSound = new ReactiveProperty<bool>(false);

    [Header("フェード")]
    [SerializeField]
    private ReactiveProperty<bool> useFade = new ReactiveProperty<bool>(false);

    [Header("ポーズ")]
    [SerializeField]
    private ReactiveProperty<bool> usePause = new ReactiveProperty<bool>(false);

    [Header("UI")]
    [SerializeField]
    private ReactiveProperty<bool> useUI = new ReactiveProperty<bool>(false);

    private void Awake() {
        //----- イベント登録 -----
        // 音
        useSound.Subscribe(_ => { if (useSound.Value) { gameObject.AddComponent<SoundManager>(); } }).AddTo(this);
        // フェード
        useFade.Subscribe(_ => { if (useFade.Value) { /*gameObject.AddComponent<FadeManager>();*/ } }).AddTo(this);
        // ポーズ
        usePause.Subscribe(_ => { if (usePause.Value) { gameObject.AddComponent<PauseManager>(); } }).AddTo(this);
        // UI
        useUI.Subscribe(_ => { if (useUI.Value) { gameObject.AddComponent<UIManager>(); } }).AddTo(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (useSound.Value) {
            gameObject.AddComponent<SoundManager>();
        }
        if (useFade.Value) {
            //gameObject.AddComponent<FadeManager>();
        }
        if (usePause.Value) {
            gameObject.AddComponent<PauseManager>();
        }
        if (useUI.Value) {
            gameObject.AddComponent<UIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
