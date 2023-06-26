//=============================================================================
// @File	: [MegaPhone.cs]
// @Brief	: メガホンの挙動
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/21	スクリプト作成
// 2023/04/21	音実装
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaPhone : BaseObj
{
    //ペンギンが持った後に落とすときまでのフラグ
    private bool fallFlg;

    private bool flyFlg;

    public override void OnStart()
    {
        Init(ObjType.HIT_HOLD);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    }

    public override void OnUpdate()
    {
        if (PauseManager.isPaused) return;              // ポーズ中処理

        PlaySoundChecker();

        if (player.IsMegaphone && fallFlg)
        {
            //メガホンでの鳴き声を鳴らす
            SoundManager.Play(audioSourcesList[1], SoundManager.ESE.PENGUIN_MEGAVOICE);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        // ポーズ処理
        if (PauseManager.isPaused) { return; }

        //プレイヤーと当たっていてプレイヤーが持っていなかったら
        if (collision.gameObject.tag == "Player" && !fallFlg)
        {
            PlayHit();
        }

        //地面に当たったときにプレイヤーが持っている状態から落としたら
        if (collision.gameObject.tag == "Ground" )
        {
            if (fallFlg)
            {
                PlayDrop();
                fallFlg = false;
            }
            if (flyFlg)
            {
                PlayDrop();
                flyFlg = false;
            }
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);

        // ポーズ処理
        if (PauseManager.isPaused) { return; }

        if (player.IsHit && other.tag == "Player")
        {
            PlayHit();
            flyFlg = true;
        }
        // プレイヤーの判定に触れているときに
        if (other.tag == "Player")
        {
            //持っている判定だったら
            if (player.IsHold && !fallFlg)
            {
                PlayHit();
                fallFlg = true;

            }

        }
    }
}
