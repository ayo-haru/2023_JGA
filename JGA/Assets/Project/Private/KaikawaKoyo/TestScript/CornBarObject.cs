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
using UnityEngine.AI;

public class CornBarObject : BaseObj
{

    private bool isPlay = false;
    private bool isOnce = false;

    private NavMeshObstacle obstacle;
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
    {
        Init();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        objType = ObjType.HIT_HOLD;
        obstacle = gameObject.GetComponent<NavMeshObstacle>();
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
        // ポーズ処理
        if (PauseManager.isPaused) { return; }

        // 地面に当たった時の音を変更
        if (collision.gameObject.tag == "Ground")
        {
            if (!isPlaySound && isPlay)
            {
                PlayDrop(audioSourcesList[0], SoundManager.ESE.PLASTIC_FALL);
            }
            if (!isPlay)
                isPlay = true;
        }
    }
    protected override void OnTriggerStay(Collider other)
    {
        // ポーズ処理
        if (PauseManager.isPaused) { return; }

        // ペンギン叩いたときの処理
        if(other.gameObject.tag == "Player" && player.IsHit){

            if (!isOnce){
                PlayHit();
                obstacle.enabled = false;
                isOnce = true;
            }
        }

        // ペンギン掴んだ時の処理
        if(other.gameObject.tag == "Player" && player.IsHold){

            if (!isOnce){
                PlayHit(audioSourcesList[0], SoundManager.ESE.PENGUIN_CATCH);
                obstacle.enabled = false;
                isOnce = true;
            }
        }

        if (!player.IsHit && !player.IsHold){
            isOnce = false;
        }

    }
}
