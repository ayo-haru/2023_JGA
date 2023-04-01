//=============================================================================
// @File	: [BaseObject.cs]
// @Brief	: オブジェクトベースの改訂版を作成してみる
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/01	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
	//---- 変数宣言 ----
	protected Rigidbody rb;						// リジッドボディ
	protected AudioSource _audioSource;			// オーディオソース

	protected bool isPlaySound;					// サウンド再生中フラグ

	/// <summary>
	/// 初期化関数
	/// </summary>
	/// <returns></returns>
	protected void Init()
    {
		rb = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
    }



	/// <summary>
	/// 音の再生中フラグの取得
	/// </summary>
	/// <returns></returns>
	public bool GetIsPlaySound()
    {
		return isPlaySound;
    }


}
