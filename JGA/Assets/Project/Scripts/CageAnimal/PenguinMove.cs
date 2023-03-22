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
// 2023/03/21	動きがなるべくかぶらないように乱数にて終了地点の調整
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

	private MoveType currentMove;
	//現在の動き
	private MoveType moveType;
	//動いているかどうか
	private bool moveFlg;
	//動きのデータの情報の数値
	private int movedata;
	//動きの数値
	private int currentMoveIndex;
	//現在の動きの数値
	private int moveIndex;

	//終了地点を格納しておく変数
	private Vector3 endPos;

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

		//初期地点を少しランダムにする。
        
        this.transform.position = CircleRandamaiser(moveIndex);

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
		//〇秒～〇秒の間でアイドルする。
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
        //速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
														endPos,
														penguinsData.dataList[movedata].walkSpeed * Time.deltaTime);
		//最期まで動いたら初期化し、アイドルに移る
		if(this.transform.position == endPos)
		{
			//最終地点の座標に行かないように記憶しておく
			moveIndex = currentMoveIndex;
			moveType = MoveType.IDLE;
        }

    }

	private void Run()
	{
		//速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
                                                        endPos,
                                                        penguinsData.dataList[movedata].runSpeed * Time.deltaTime);
        //最期まで動いたら初期化し、アイドルに移る
        if (this.transform.position == endPos)
        {
            //最終地点の座標に行かないように記憶しておく
            moveIndex = currentMoveIndex;
            moveType = MoveType.IDLE;
        }
    }
	//動きを決める関数
	private void MoveEnter()
	{
		//動き決定の範囲
        moveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.RUN + 1);
        moveFlg = true;
		//次の動きが歩きになったとき
		if(moveType == MoveType.WALK)
		{
			//終了地点がなるべくかぶらないようにするため違う場所になるまで回す。
			while (currentMoveIndex == moveIndex)
			{
				currentMoveIndex = Random.Range(0, penguinsData.rangeList.Count);
			}
			//終了地点が決まったらランダムで若干ずらす。
			endPos = CircleRandamaiser(currentMoveIndex);

		}
        //次の動きが走りになったとき
        if (moveType == MoveType.RUN)
		{
            //終了地点がなるべくかぶらないようにするため違う場所になるまで回す。
            while (currentMoveIndex == moveIndex)
            {
                currentMoveIndex = Random.Range(0, penguinsData.rangeList.Count);
            }
            //終了地点が決まったらランダムで若干ずらす。
            endPos = CircleRandamaiser(currentMoveIndex);
        }
    }

    //0から半径分の範囲を取るためそれを指定された座標と計算することで若干位置をずらせる。
    private Vector3 CircleRandamaiser(int index)
	{
		var retVector = new Vector3();
		//範囲を決める。
        var startPoint = Random.insideUnitCircle * penguinsData.rangeArea;

        //決めた範囲分動かした場所を決定し返す
        retVector = new Vector3(penguinsData.rangeList[index].x + startPoint.x,
                                penguinsData.rangeList[index].y,
                                penguinsData.rangeList[index].z + startPoint.y);
		return retVector;
    }

}
