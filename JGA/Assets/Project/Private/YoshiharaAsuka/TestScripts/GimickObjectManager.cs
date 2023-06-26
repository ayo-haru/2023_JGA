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
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GimickObjectManager : SingletonMonoBehaviour<GimickObjectManager>
{
	[SerializeField]
	private List<BaseObj> GimickObjectsList = new List<BaseObj>();

	/// <summary>
	/// オブジェクトが有効化された時
	/// </summary>
	private void OnEnable()
	{
		// このオブジェクトが有効化された時にシーンを生成
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

		if(GimickObjectsList.Count == 0) { return; }

		foreach (BaseObj Gimickobjects in GimickObjectsList){
			Gimickobjects.OnStart();
		}

	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		if (GimickObjectsList.Count == 0) { return; }

		foreach (BaseObj Gimickobjects in GimickObjectsList)
		{
			Gimickobjects.OnUpdate();
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

		Debug.LogError("指定されたオブジェクトは見つかりませんでした。");
		return null;
	}

	/// <summary>
	/// GimickObjectManagerで登録しているリストにあるオブジェクトをすべて取得
	/// </summary>
	/// <returns></returns>
	public List<GameObject> GetGimickObjectAll()
	{
		List<GameObject> objectsList = new List<GameObject>();

		foreach(BaseObj gimickObjects in GimickObjectsList){
			objectsList.Add(gimickObjects.gameObject);
		}

		return objectsList;
	}


}

