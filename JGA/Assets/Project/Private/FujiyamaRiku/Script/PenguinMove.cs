//=============================================================================
// @File	: [PenguinMove.cs]
// @Brief	: ペンギンの動き
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 移動の処理 https://tsubakit1.hateblo.jp/entry/2015/02/20/235021
// 
// [Date]
// 2023/03/15	スクリプト作成
// 2023/03/16	動きのルーチン作成
// 2023/03/20	動きの方式を変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinMove : MonoBehaviour
{
	enum MoveType
	{
		IDLE,
		WALK,
		RUN,
		JUMP,
		APPEAL,
		SWIM,
		MAX_MOVE
	}

	//
	private MoveType currentMove;
	//現在の動き
	private MoveType moveType;
	//動いているかどうか
	private bool moveFlg;
	//動きのデータの情報の数値
	private int movedata;
	//動きのインデックス
	private int currentMoveIndex;
	//現在の動きのインデックス
	private int moveIndex;

	//ペンギンの総合データ
    [SerializeField]private PenguinsData penguinsData;
	//動いてるときの時間
	private float moveTime;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        currentMove	= MoveType.IDLE;
		moveType    = MoveType.IDLE;
		moveFlg = false;
		moveTime = 0.0f;
		movedata = Random.Range(0, penguinsData.dataList.Count);
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		currentMoveIndex = Random.Range(0, penguinsData.rangeList.Count);
		moveIndex = currentMoveIndex;
        this.transform.position = penguinsData.rangeList[moveIndex];
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
			if (!moveFlg)
			{
				MoveEnter();
            }
			switch (moveType)
			{
				case MoveType.IDLE:
					currentMove = MoveType.IDLE;
					Idle();
                    break;

				case MoveType.WALK:
					currentMove = MoveType.WALK;
					Walk();
					break;

				case MoveType.RUN:
					currentMove = MoveType.RUN;
					Run();
					break;

				case MoveType.JUMP:
					break;

				case MoveType.APPEAL:
					break;

				case MoveType.SWIM:
					break;
			}
		
		Debug.Log(moveType);
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

	private void Idle()
	{
        moveTime += Time.deltaTime;

		if (moveTime >= 1.0f)
		{
            moveIndex = currentMoveIndex;
            moveTime = 0.0f;
            moveFlg = false;
        }
    }

	private void Walk()
	{
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
														penguinsData.rangeList[currentMoveIndex],
														penguinsData.dataList[movedata].walkSpeed * Time.deltaTime);

		if(this.transform.position == penguinsData.rangeList[currentMoveIndex])
		{
			moveIndex = currentMoveIndex;
			moveType = MoveType.IDLE;
        }

    }

	private void Run()
	{
        this.transform.position = Vector3.MoveTowards(this.transform.position,
                                                        penguinsData.rangeList[currentMoveIndex],
                                                        penguinsData.dataList[movedata].runSpeed * Time.deltaTime);

        if (this.transform.position == penguinsData.rangeList[currentMoveIndex])
        {
            moveIndex = currentMoveIndex;
            moveType = MoveType.IDLE;
        }
    }

	private void MoveEnter()
	{
        moveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.RUN + 1);
        moveFlg = true;
		if(moveType == MoveType.WALK)
		{
			while (currentMoveIndex == moveIndex)
			{
				currentMoveIndex = Random.Range(0, penguinsData.rangeList.Count);
			}
        }
        if (moveType == MoveType.RUN)
		{
            while (currentMoveIndex == moveIndex)
            {
                currentMoveIndex = Random.Range(0, penguinsData.rangeList.Count);
            }
        }
    }

}
