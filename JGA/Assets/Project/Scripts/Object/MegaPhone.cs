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

    protected override void Awake()
    {
        Init();
        objType = ObjType.HIT_HOLD;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
        //SEが鳴ってるときになっているフラグを返す
        PlaySoundChecker();

        if(player.IsMegaphone && fallFlg)
        {
            //メガホンでの鳴き声を鳴らす
            SoundManager.Play(audioSourcesList[0], SoundManager.ESE.PENGUIN_MEGAVOICE);
        }

    }
    protected override void OnCollisionEnter(Collision collison)
    {
        //プレイヤーと当たっていてプレイヤーが持っていなかったら
        if (collison.gameObject.tag == "Player" && !fallFlg)
        {
            PlayHit();
        }

        

        //地面に当たったときにプレイヤーが持っている状態から落としたら
        if (collison.gameObject.tag == "Ground" )
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
