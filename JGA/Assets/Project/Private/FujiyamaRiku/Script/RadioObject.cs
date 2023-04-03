//=============================================================================
// @File	: [RadioObject.cs]
// @Brief	: ラジオの挙動
// @Author	: FujiyamaRiku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/31	スクリプト作成
// 2023/03/31	音実装
// 2023/03/31   ラジオが鳴ってるときの音実装
// 2023/04/02   音が多数でなってしまっていたのの解消、ポーズの処理の書き込み
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;

public class RadioObject : BaseObj, IObjectSound
{
    //ペンギンが持った後に落とすときまでのフラグ
    private bool fallFlg;
    //ラジオをオンオフするフラグ
    private bool onOffFlg;
    //ラジオが鳴ってる時用のAudioSource
    private AudioSource[] playAudio;

    const int RadioAudio = 1;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        Init();
        objType = ObjType.HIT_HOLD;
        //生成＆初期化
        playAudio = this.GetComponents<AudioSource>();

        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        //SEが鳴ってるときになっているフラグを返す
        if (audioSource.isPlaying)
        {
            isPlaySound = true;
        }
        else
        {
            isPlaySound = false;
        }
        //ラジオが鳴っているときの処理
        if (playAudio[RadioAudio].isPlaying)
        {
            isPlaySound = true;
        }
        else
        {
            //ラジオがオンになったときにSEが終わったらラジオを流す
            if (onOffFlg && !audioSource.isPlaying)
            {
                SoundManager.Play(playAudio[RadioAudio], SoundManager.ESE.RADIO_PLAY);
            }
            isPlaySound = false;
        }
    }

    private void OnCollisionEnter(Collision collison)
    {
        //プレイヤーと当たっていてプレイヤーが持っていなかったら
        if (collison.gameObject.tag == "Player" && !fallFlg)
        {
            SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
        }
        //地面に当たったときにプレイヤーが持っている状態から落としたら
        if (collison.gameObject.tag == "Ground" && fallFlg)
        {
            PlayRelease();
            fallFlg = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーの判定に触れているときに
        if (other.tag == "Player")
        {
            //持っている判定だったら
            if (player.IsHold && !fallFlg)
            {
                PlayPickUp();
                fallFlg = true;
            }
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //プレイヤーがはたいたときにOnOffする
        if (player.IsHit && other.tag == "Player")
        {
            if (onOffFlg)
            {
                onOffFlg = false;

                playAudio[RadioAudio].Stop();

                SoundManager.Play(audioSource, SoundManager.ESE.RADIO_RELEASE);

            }
            else if (!onOffFlg)
            {
                onOffFlg = true;
                SoundManager.Play(audioSource, SoundManager.ESE.RADIO_CATCH);

            }
        }
    }

    public void PlayPickUp()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
    }

    public void PlayHold()
    {

    }

    public void PlayRelease()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_DROP);
    }

    private void Pause()
    {
        audioSource.Pause();
        playAudio[RadioAudio].Pause();
    }

    private void Resumed()
    {
        audioSource.Play();
        playAudio[RadioAudio].Play();
    }
}
