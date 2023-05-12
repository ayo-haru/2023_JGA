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
	public bool fishFlg;
    public GameObject fishObj;

    protected virtual void Start()
    {
        fishObj = GameObject.Find("Fish");
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Fish")
		{
			fishFlg = true;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Fish")
        {
            fishFlg = false;
        }
    }
}
