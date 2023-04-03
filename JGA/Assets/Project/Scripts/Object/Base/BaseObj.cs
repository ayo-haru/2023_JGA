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

//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class BaseObj : MonoBehaviour
{
	public enum ObjType
	{
		None = 0,   
		HIT,
		HOLD,
		HIT_HOLD,
		RETURN,

	}


	protected Rigidbody rb;
	protected AudioSource audioSource;
	protected Player player;

	public ObjType objType;

	protected bool isPlaySound; // 音が鳴っているか



	protected void Init()
	{
		rb  = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// 音が鳴ってるかフラグの取得
	/// </summary>
	/// <returns></returns>
	public bool GetisPlaySound() {
		return isPlaySound;
	}

}
