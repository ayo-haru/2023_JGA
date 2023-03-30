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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioObject : BaseObj, IObjectSound
{
    private bool fallFlg;
    private bool onOffFlg;
    private bool pickUpFlg;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        Init();
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
        if (audioSource.isPlaying)
        {
            isPlaySound = true;
        }
        else
        {
            isPlaySound = false;
        }
    }

    private void OnCollisionEnter(Collision collison)
    {
        if (collison.gameObject.tag == "Player" && !pickUpFlg)
        {
            SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
        }
        if (collison.gameObject.tag == "Ground" && fallFlg)
        {
            PlayRelease();
            fallFlg = false;
            pickUpFlg = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player.IsHold && !pickUpFlg)
            {
                PlayPickUp();
                pickUpFlg = true;
                fallFlg = true;
            }
            if (player.IsInteract)
            {
                if (onOffFlg)
                {
                    onOffFlg = false;
                    SoundManager.Play(audioSource, SoundManager.ESE.RADIO_CATCH);
                }
                if (!onOffFlg)
                {
                    onOffFlg = true;
                    SoundManager.Play(audioSource, SoundManager.ESE.RADIO_RELEASE);
                }
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
}
