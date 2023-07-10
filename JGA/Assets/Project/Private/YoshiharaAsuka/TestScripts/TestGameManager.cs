//=============================================================================
// @File	: [TestGameManager.cs]
// @Brief	: 
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/07	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestGameManager : SingletonMonoBehaviour<TestGameManager>
{
	// マネージャーオブジェクトのプレハブを取得
	public static PrefabContainer managerObj;

	/* ----インスペクター公開----*/
	[Header("生成したいオブジェクトにチェックを入れてください。")]

	[SerializeField]
	private bool isCreateGimickObject = false;

	[SerializeField]
	private bool isCreateGuest = false;

	private GameObject gimickObjectManaager;
	private GameObject guestManager;

	protected override void Awake()
	{
		base.Awake();

		// マネージャーデータのアセットを取得
		managerObj = AddressableLoader<PrefabContainer>.Load("ManagerObjData");

		// 以下各オブジェクト・マネージャーを取得
		gimickObjectManaager = PrefabContainerFinder.Find(managerObj, "GimickObjectManager");
		guestManager = PrefabContainerFinder.Find(managerObj, "GuestManager");

	}
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	private void Start()
	{
		CreateGimickObjectManager();
		CreateGuestManager();
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	private void Update()
	{

	}


	private void RegistManager()
	{

	}

	private void CreateGimickObjectManager()
	{
		if (!isCreateGimickObject){
			return;
		}
		else if (!gimickObjectManaager){
			Debug.Log(gimickObjectManaager.name + "は見つかりませんでした.");
		}
		else Instantiate(gimickObjectManaager);
	}

	private void CreateGuestManager()
	{
		if (!isCreateGuest){
			return;
		}
		else if (!guestManager)
		{
			Debug.Log(guestManager.name + "は見つかりませんでした.");
		}
		else Instantiate(gimickObjectManaager);
	}
}

