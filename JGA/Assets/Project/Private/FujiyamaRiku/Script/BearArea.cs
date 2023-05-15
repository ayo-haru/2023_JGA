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
    private bool fishFlg;
    public bool dropFlg;
    private int count;
    public GameObject[] fishObj;
    public GameObject roomPos;
    private Player player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Fish"))
        {
            fishFlg = true;
            count++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name.Contains("Fish") && fishFlg)
		{
            if(!player.IsHold)
            {
                for(int i = 0; i < fishObj.Length;i++)
                {
                    if(other.name == fishObj[i].name)
                    {
                        Destroy(fishObj[i].GetComponent<FishObject>());
                        dropFlg = true;
                        fishNum = i;
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Fish"))
        {
            count--;
            if(count <= 0)
            {
                fishFlg = false;
            }

        }
    }
}
