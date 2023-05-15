//=============================================================================
// @File	: [BearArea.cs]
// @Brief	: 熊のエリア(魚の探索)
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/11	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BearArea : MonoBehaviour
{
    public int fishNum;
    public bool fishFlg;
    public GameObject[] fishObj;
    public GameObject roomPos;

    protected virtual void Start()
    {
        fishNum = -1;
        var fish = GameObject.Find("Fish");
        fishObj = new GameObject[fish.transform.childCount];
        //FishBasket
        for (int i = 0; i < fish.transform.childCount; i++)
        {
            fishObj[i] = GameObject.Find("Fish").transform.GetChild(i).gameObject;
        }
        
        roomPos = GameObject.Find("BearRoomPos");
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name.Contains("Fish") && !fishFlg)
		{
            for(int i = 0;i < fishObj.Length;i++)
            {
                if (other.gameObject == fishObj[i])
                {
                    fishNum = i;
                }
            }
			fishFlg = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Fish"))
        {
            fishNum = -1;
            fishFlg = false;
        }
    }
}
