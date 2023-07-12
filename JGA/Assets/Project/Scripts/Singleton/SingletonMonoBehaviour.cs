//=============================================================================
// @File	: [SingletonMonoBehaviour.cs]
// @Brief	: シングルトンを作成するためにMonoBehaviourをシングルトン生成するクラス
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 参考URL:https://nobushiueshi.com/unitymonobehaviour%E3%82%92%E3%82%B7%E3%83%B3%E3%82%B0%E3%83%AB%E3%83%88%E3%83%B3singleton%E3%81%AB%E3%81%99%E3%82%8B%E6%96%B9%E6%B3%95/
//			  abstract(抽象化クラス)について : https://www.hanachiru-blog.com/entry/2019/03/29/213816
//			  whereについて : https://hiyotama.hatenablog.com/entry/2016/12/10/090000
// 
// [Date]
// 2023/02/08	スクリプト作成
// 2023/06/01	編集、dontDestroyOnLoadプロパティだったのを、直接指定するようにした
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	//protected abstract bool dontDestroyOnLoad { get;}

	private static T instance;

	public static T Instance
	{
		get 
		{
			if (instance == null)
			{
				//Type t = typeof(T);

				//instance = (T)FindObjectOfType(t);
				instance = FindObjectOfType<T>();
				if (instance == null)
				{
					Debug.LogError(typeof(T) + "is missing int the scene.");
				}
			}
			return instance;
		}
	}


	protected virtual void Awake()
	{
		//if(this != Instance){
		//    Destroy(this);
		//    return;
		//}
		//if (dontDestroyOnLoad){
		//    DontDestroyOnLoad(this.gameObject);
		//}

		// インスタンスしたものがnullの場合と、別のモノを参照した場合を判定
		if(instance != null && instance != this){
			Destroy(gameObject);
			return;
		}

		Debug.Log(this.name + "<color=#00EF41>が生成されました.</color>");
		instance = this as T;
		DontDestroyOnLoad(this.gameObject);
	}

	/// <summary>
	/// Singletonで生成しているオブジェクトを破壊する処理
	/// </summary>
	public void Release()
	{
		if(instance == null){
			Debug.Log(this.gameObject.name + "はありませんでした。" + "Release処理は行いませんでした。");
			return;
		}
		else{
			DestroyImmediate(this.gameObject);
			Debug.Log(this.gameObject.name + "の解放処理を行いました");

		}

	}
}
