//=============================================================================
// @File	: [GimickObjectManager.cs]
// @Brief	: ギミックで使用するオブジェクトの管理を行うスクリプト
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/14	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimickObjectManager : SingletonMonoBehaviour<GimickObjectManager>
{
	BaseObj GimickObjects = null; 

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{

		GimickObjects.OnStart();
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		GimickObjects.OnUpdate();
	}
}

