//=============================================================================
// @File	: [GroundCheck.cs]
// @Brief	: 地面との接触判定を行う処理
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 参考URL:https://ingentity.hatenablog.com/entry/2018/04/30/182217
// 
// [Date]
// 2023/04/28	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class GroundCheck : MonoBehaviour
{
	// 地面と接触した際のイベント
	public UnityEvent OnEnterGround;

	// 地面から離れた際のイベント
	public UnityEvent OnExitGround;

	// 接触回数
	[SerializeField] private int enterNum = 0;

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("isGround");
		enterNum++;
		OnEnterGround.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.Log("ExitGround");
		enterNum--;
		if (enterNum <= 0) OnExitGround.Invoke();
	}
}
