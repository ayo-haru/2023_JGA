//=============================================================================
// @File	: [PenguinMove.cs]
// @Brief	: ペンギンの動き
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/15	スクリプト作成
// 2023/03/16			動きのルーチン作成
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
	private MoveType moveType;
	private bool moveFlg;
	private Transform[] cageObj;
	private Vector3[] cageArea;
	private float dir;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        currentMove	= MoveType.IDLE;
		moveType    = MoveType.IDLE;
		moveFlg = false;
		dir = 1.0f;
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		var cageParent = GameObject.Find("PenguinCage01");
		cageObj = new Transform[cageParent.transform.childCount];
        cageArea = new Vector3[cageParent.transform.childCount];

        for (int i = 0; i < cageObj.Length;i++)
		{
			cageObj[i] = cageParent.transform.GetChild(i).transform;
		}
		cageArea[0] = cageObj[0].position;
        cageArea[1] = cageObj[1].position;

    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		if(!moveFlg)
		{
			moveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.MAX_MOVE);
			moveType = MoveType.WALK;
			moveFlg = true;
        }
		switch (moveType) 
		{
			case MoveType.IDLE:
			break;

			case MoveType.WALK:
				Walk();
            break;

			case MoveType.RUN:
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

	private void Walk()
	{
		var move = this.transform.position.x + dir * 0.05f;
		this.transform.position = new Vector3(move, this.transform.position.y, this.transform.position.z);

        if (cageArea[0].x <= this.transform.position.x)
		{
			dir = -1.0f;
        }
        if (cageArea[1].x >= this.transform.position.x)
        {
            dir = 1.0f;
        }
    }

}
