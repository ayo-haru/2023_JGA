//=============================================================================
// @File	: [AnimalBase.cs]
// @Brief	: 動物のクラス
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/21	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBase : MonoBehaviour
{
	protected  enum MoveType
	{
        WALK,
        RUN,
        IDLE,
        TURN,
        MAX_MOVE
    }


    protected Animator anim;                  //アニメーター格納用

    protected MoveType currentMoveType;       //現在の動き
    protected MoveType moveType;              //一つ前にした動き

    protected bool moveFlg;                   //動いているかどうかのフラグ

    protected int movedata;                   //動きのデータの情報の数値
    protected int currentMoveIndex;           //動きの数値
    protected int moveIndex;                  //現在の動きの数値


    protected Vector3 endPos;                 //終了地点を格納しておく変数

    protected Rigidbody rb;                   //リジッドボディ

    protected float moveTime;                 //動いてるときの時間

    protected bool pauseFlg;                  //ポーズ用フラグ

    protected bool turnFlg;                   //ターン用フラグ

    protected virtual void Awake()
	{

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


}
