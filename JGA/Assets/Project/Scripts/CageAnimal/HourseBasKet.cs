//=============================================================================
// @File	: [HourseBasKet.cs]
// @Brief	: 馬の箱ににんじんがあるかどうか
// @Author	: MAKIYA MIO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/24	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourseBasKet : MonoBehaviour
{
    public List<GameObject> carrot = new List<GameObject>();
    public List<bool> bBasket = new List<bool>();

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        List<string> name = new List<string>();
        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            objects.Add(this.gameObject.transform.GetChild(i).gameObject);
        }
        for(int i = 1; i < 4; i++)
        {
            name.Add("Carrot_00" + i);
        }
        for (int i = 0; i < objects.Count; i++)
        {
            for(int j = 0; j < name.Count; j++)
            {
                if(objects[i].gameObject.name == name[j])
                {
                    carrot.Add(objects[i]);
                    bBasket.Add(true);
                }
            }
        }
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
	}

    /// <summary>
    /// バスケットににんじんが入ってるか
    /// </summary>
    private void CarrotIn()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //for(int i = 0; i < carrot.Count; i++)
        //{
        //    if (collision.gameObject == carrot[i])
        //        if (!bBasket[i]) bBasket[i] = true;
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        //for (int i = 0; i < carrot.Count; i++)
        //{
        //    if (collision.gameObject == carrot[i])
        //        if (bBasket[i]) bBasket[i] = false;
        //}
    }
}
