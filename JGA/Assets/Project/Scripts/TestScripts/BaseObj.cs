//=============================================================================
// @File	: [BaseObj.cs]
// @Brief	: ギミックオブジェクトのベース
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/29	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObj : MonoBehaviour
{
	protected Rigidbody rb;
	protected AudioSource audioSource;


	
	protected bool isPlaySound;	// 音が鳴っているか


	/// <summary>
	/// 音が鳴ってるかフラグの取得あ
	/// </summary>
	/// <returns></returns>
	public bool GetisPlaySound() {
		return isPlaySound;
	}
}
