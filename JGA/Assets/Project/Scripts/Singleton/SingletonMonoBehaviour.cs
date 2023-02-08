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
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected abstract bool dontDestroyOnLoad { get;}

    private static T instance;

    public static T Instance
    {
        get 
        {
            if (!instance)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (!instance)
                {
                    Debug.LogError(t + "is nothing.");
                }
            }
            return instance;
        }
    }


    protected virtual void Awake()
    {
        if(this != Instance){
            Destroy(this);
            return;
        }
        if (dontDestroyOnLoad){
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
