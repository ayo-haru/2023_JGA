//=============================================================================
// @File	: [OnVisible.cs]
// @Brief	: 
// @Author	: YOUR_NAME
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/05	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnVisible : MonoBehaviour
{
    MainCamera mainCamera;
    Vector3 myPos;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        mainCamera = GameObject.Find("CameraParent").GetComponent<MainCamera>();
        myPos = this.gameObject.transform.position;

    }
    /// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
    {
        myPos = this.gameObject.transform.position;
    }
    public Vector3 GetPosition()
    {
        return myPos;
    }
}
