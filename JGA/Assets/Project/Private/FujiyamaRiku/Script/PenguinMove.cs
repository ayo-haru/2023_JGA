//=============================================================================
// @File	: [PenguinMove.cs]
// @Brief	: ペンギンの動き
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
// 2023/03/16	動きのルーチン作成
//
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
		APPEAL,
		SWIM,
		MAX_MOVE
	}

	private MoveType currentMove;
	private MoveType[] moveType;
	private bool[] moveFlg;
	private float dir;

	private PenguinsData penguinsData;
	private float[] moveTime;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        currentMove	= MoveType.IDLE;
		moveType    = new MoveType[2];
		moveFlg = new bool[2];
		dir = 1.0f;
		moveTime = new float[2];

		//for (int i = 0; i < cageObj.Length; i++)
		//{
		//	cageObj[i] = cageParent.transform.GetChild(i).transform;
		//}

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
		for (int i = 0; i < moveType.Length; i++)
		{
			if (!moveFlg[i])
			{
				moveType[i] = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.MAX_MOVE);
                moveType[i] = MoveType.WALK;
				moveFlg[i] = true;
			}
			switch (moveType[i])
			{
				case MoveType.IDLE:
					break;

				case MoveType.WALK:
					currentMove = MoveType.WALK;
					Walk();
					break;

				case MoveType.RUN:
					break;

				case MoveType.APPEAL:
					break;

				case MoveType.SWIM:
					break;
			}
		}
		Debug.Log(moveType);
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

	private void Walk()
	{
		var move = this.transform.position.x + dir * 0.05f;
		this.transform.position = new Vector3(move, this.transform.position.y, this.transform.position.z);

    }

}
