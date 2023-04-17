//=============================================================================
// @File	: [BaseObj.cs]
// @Brief	: ギミックオブジェクトのベース
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/29	スクリプト作成
// 2023/04/04   OutLineスクリプトを追加するようにattributeつけました(吉原)
// 2023/04/18   ポーズ処理変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Outline))]
public abstract class BaseObj : MonoBehaviour, IPlayObjectSound
{
	public enum ObjType
	{
		None = 0,   
		HIT,
		HOLD,
		HIT_HOLD,
		RETURN,

	}

	//---変数宣言---
	protected Rigidbody rb;
	protected AudioSource audioSource;
	protected Player player;

	public ObjType objType;

	protected bool isPlaySound; // 音が鳴っているか

    //--- ポーズ用変数 ---
    protected Vector3 pauseVelocity = Vector3.zero;
    protected Vector3 pauseAngleVelocity = Vector3.zero;

    protected virtual void Awake()
    {
        Init();
    }


    protected void Init()
	{
		rb  = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(this.gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(this.gameObject);
    }

    protected void PlaySoundChecker()
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

    /// <summary>
	/// ポーズの処理
	/// </summary>
	protected virtual void Pause()
    {
        // 物理挙動停止
        audioSource.Pause();
        pauseVelocity = rb.velocity;
        pauseAngleVelocity = rb.angularVelocity;
        rb.isKinematic = true;
    }

    protected virtual void Resumed()
    {
        audioSource.Play();
        // 物理挙動開始
        rb.velocity = pauseVelocity;
        rb.angularVelocity = pauseAngleVelocity;
        rb.isKinematic = false;
    }

    /// <summary>
    /// 音が鳴ってるかフラグの取得
    /// </summary>
    /// <returns></returns>
    public bool GetisPlaySound() {
		return isPlaySound;
	}

    public void PlayHit()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_HIT);
    }

    public void PlayHit(AudioSource audioSource, SoundManager.ESE soundNumber)
    {
        SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
    }

    public void PlayDrop()
    {
        SoundManager.Play(audioSource, SoundManager.ESE.OBJECT_DROP);
    }

    public void PlayDrop(AudioSource audioSource, SoundManager.ESE soundNumber)
    {
        SoundManager.Play(audioSource, (SoundManager.ESE)soundNumber);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {

        // 地面と当たった時の音
        if (collision.gameObject.tag == "Ground")
        {
            PlayHit();
        }

        // それ以外のオブジェクトと当たった時
        else
        {
            PlayHit();
        }
    }
    protected virtual void OnCollisionStay(Collision collision) { }
    protected virtual void OnCollisionExit(Collision collision) { }


    protected virtual void OnTriggerEnter(Collider other)
    {

    }
    protected virtual void OnTriggerStay(Collider other) { }
    protected virtual void OnTriggerExit(Collider other) { }
}
