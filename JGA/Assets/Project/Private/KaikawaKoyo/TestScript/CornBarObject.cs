//=============================================================================
// @File	: [CornBarObject.cs]
// @Brief	: 
// @Author	: Kaikawa Koyo
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/22	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornBarObject : BaseObj
{

    private bool isPlay = false;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        Init();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        objType = ObjType.HIT_HOLD;
    }
    
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update()
    {
        // ポーズ処理
        if (PauseManager.isPaused) { return; }

        PlaySoundChecker();
        
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // 地面に当たった時の音を変更
        if (collision.gameObject.tag == "Ground")
        {
            if (!isPlaySound && isPlay)
            {
                PlayDrop(audioSourcesList[0], SoundManager.ESE.CARDBOARDBOX_002);
            }
            if (!isPlay)
                isPlay = true;
        }
        else
        {
            PlayHit();
        }
    }
    protected override void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // 掴まれたら音を出す
            if (player.IsHold)
            {
                PlayDrop(audioSourcesList[0], SoundManager.ESE.CAN_CATCH);
            }
        }
    }
}
