using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [Header("---�g�p����@�\��I��---")]
    [Header("��")]
    [SerializeField]
    private ReactiveProperty<bool> useSound = new ReactiveProperty<bool>(false);

    [Header("�t�F�[�h")]
    [SerializeField]
    private ReactiveProperty<bool> useFade = new ReactiveProperty<bool>(false);

    [Header("�|�[�Y")]
    [SerializeField]
    private ReactiveProperty<bool> usePause = new ReactiveProperty<bool>(false);

    [Header("UI")]
    [SerializeField]
    private ReactiveProperty<bool> useUI = new ReactiveProperty<bool>(false);

    private void Awake() {
        //----- �C�x���g�o�^ -----
        // ��
        useSound.Subscribe(_ => { if (useSound.Value) { gameObject.AddComponent<SoundManager>(); } }).AddTo(this);
        // �t�F�[�h
        useFade.Subscribe(_ => { if (useFade.Value) { /*gameObject.AddComponent<FadeManager>();*/ } }).AddTo(this);
        // �|�[�Y
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
