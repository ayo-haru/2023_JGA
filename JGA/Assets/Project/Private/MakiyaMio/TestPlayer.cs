//=============================================================================
// @File	: [TestPlayer.cs]
// @Brief	: プレイヤーの仮の処理
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float speed = 3.0f;
    //[SerializeField] private GameObject obj;
    //private GameObject gimmickObj;
    //public static List<GameObject> gimmickList = new List<GameObject>();

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{

    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (Input.GetKeyDown(KeyCode.E))
        {
            //gimmickObj = Instantiate(obj, transform.position, Quaternion.identity);
            //gimmickList.Add(gimmickObj);
            //int i = 0;
            //gimmickObj.name = "kanban" + i;
            //i++;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }
}
