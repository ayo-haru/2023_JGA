//=============================================================================
// @File	: [GimickObjectManager.cs]
// @Brief	: ギミックで使用するオブジェクトの管理を行うスクリプト
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/14	スクリプト作成
// 2023/06/14	オブジェクトをリストに登録するように変更
// 2023/07/12	GetGImickObjectAll()をBaseObj型で返すように変更
// 2023/07/12	BaseObjのOnStartをAddListに追加された際に行うように変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GimickObjectManager : SingletonMonoBehaviour<GimickObjectManager>
{
	[SerializeField]
	private List<BaseObj> GimickObjectsList = new List<BaseObj>();
	protected override void Awake()
	{
		base.Awake();

		// このオブジェクトが有効化された時にシーンを生成
		TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TESTGIMICK);

		Debug.Log("<color=#00AEEF>ギミックオブジェクトシーンを加算します.</color>");

	}

	/// <summary>
	/// オブジェクトが有効化された時
	/// </summary>
	private void OnEnable()
	{
	}

	/// <summary>
	/// オブジェクトが破壊されたとき	/// </summary>
	private void OnDestroy()
	{
		// このオブジェクトが破壊されたときにシーンを削除
		//TestMySceneManager.SubtractScene(TestMySceneManager.SCENE.SCENE_TESTGIMICK);
	}


	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	private void Start()
	{
		//TestMySceneManager.AddScene(TestMySceneManager.SCENE.SCENE_TESTGIMICK);

		
		/*  AddGimickObjectListにOnStartを移動(吉原:07/12)
		 *  
		if (GimickObjectsList.Count == 0) { return; }

		foreach (BaseObj Gimickobjects in GimickObjectsList)
		{
			Gimickobjects.OnStart();
		}
		*/

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	private void Update()
	{
		if (GimickObjectsList.Count == 0) { return; }

		foreach (BaseObj Gimickobjects in GimickObjectsList)
		{
			Gimickobjects.OnUpdate();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Release();
		}

	}

	/// <summary>
	/// リストにギミックオブジェクトを追加
	/// </summary>
	/// <param name="obj"></param>
	public void AddGimickObjectsList(BaseObj obj)
	{
		/* 重複チェックを入れようと思ったが、
		 * 同じオブジェクト複数個使用する可能性がある
		 * と思ったので記載なし。(06/16時点 吉原) */
		GimickObjectsList.Add(obj);
		obj.OnStart();

	}

	/// <summary>
	/// リストにあるギミックオブジェクトを削除
	/// </summary>
	/// <param name="obj"> </param>
	public void RemoveGimickObjectsList(BaseObj obj)
	{
		/* 重複チェックを入れようと思ったが、
		 * 同じオブジェクト複数個使用する可能性がある
		 * と思ったので記載なし。(06/16時点 吉原) */
		GimickObjectsList.Remove(obj);
	}

	/// <summary>
	/// GimickObjectManagerで登録ているリストにあるオブジェクトの型があるかチェック
	/// </summary>
	/// <typeparam name="T">検索したい型</typeparam>
	/// <returns>リストにある型 → true / リストに無い型 → false </returns>
	public bool CheckGimickObjectType<T>() where T : BaseObj
	{
		System.Type targetType = typeof(T);

		foreach(BaseObj Gimickobjects in GimickObjectsList){

			//System.Type GimickObjectstype = GimickObjectsList.GetType();
			//if(GimickObjectstype == targetType){
			//	return true;
			//}
			if(Gimickobjects is T){
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// GimickObjectManagerで登録しているリストのオブジェクトの型を取得
	/// </summary>
	/// <typeparam name="T">検索したい型</typeparam>
	/// <returns>取得した型を返す</returns>
	public System.Type GetGimickObjectType<T>() where T : BaseObj
	{
		System.Type targetType = typeof(T);
		foreach (BaseObj Gimickobjects in GimickObjectsList)
		{

			//System.Type GimickObjectstype = GimickObjectsList.GetType();
			//if(GimickObjectstype == targetType){
			//	return true;
			//}
			if (Gimickobjects is T)
			{
				return targetType;
			}
		}
		return targetType;
	}

	/// <summary>
	/// GimickObjectManagerで登録しているリストのオブジェクトを取得
	/// </summary>
	/// <typeparam name="T">検索したい型</typeparam>
	/// <returns>検索した型のオブジェクトを返す</returns>
	public GameObject GetGimickObject<T>() where T : BaseObj
	{

		foreach(BaseObj gimickObjects in GimickObjectsList){
			if(gimickObjects is T){
				return gimickObjects.gameObject; 
			}
		}

		Debug.LogError("<color=#fd7e00>指定されたオブジェクトは見つかりませんでした。</color>");
		return null;
	}

	/// <summary>
	/// GimickObjectManagerで登録しているリストにあるオブジェクトをすべて取得
	/// </summary>
	/// <returns></returns>
	public List<BaseObj> GetGimickObjectAll()
	{
		List<BaseObj> objectsList = new List<BaseObj>();

		foreach(BaseObj gimickObjects in GimickObjectsList){
			objectsList.Add(gimickObjects);
		}

		return objectsList;
	}


}

