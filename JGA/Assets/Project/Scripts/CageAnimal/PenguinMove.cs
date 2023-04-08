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
using UniRx;

public class PenguinMove : MonoBehaviour
{
	enum MoveType
	{
		
		WALK,
		RUN,
        IDLE,
        TURN,
		JUMP,
		APPEAL,
		SWIM,
		MAX_MOVE
	}

	private MoveType currentMoveType;
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

    private Rigidbody rb; 

	//動いてるときの時間
	private float moveTime;

	//ポーズ用フラグ
	private bool pauseFlg;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{

        currentMoveType = MoveType.IDLE;
		moveType    = MoveType.IDLE;
		moveFlg = false;
		moveTime = 0.0f;
		movedata = Random.Range(0, BoothAnimalManager.Instance.penguinsData.dataList.Count);
        rb = this.GetComponent<Rigidbody>();

        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { ReGame(); }).AddTo(gameObject);
    }

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		
		currentMoveIndex = BoothAnimalManager.Instance.penguinStartIndex;
        moveIndex = currentMoveIndex;


    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (pauseFlg)
        {
            return;
        }
        if (!moveFlg)
        {
            MoveEnter();
        }
        switch (currentMoveType)
        {
            case MoveType.IDLE:
                Idle();
                break;

            case MoveType.WALK:
                Walk();
                break;

            case MoveType.RUN:
                Run();
                break;
            case MoveType.TURN:
                Turn();
                break;
            case MoveType.JUMP:
                moveFlg = false;
                break;

            case MoveType.APPEAL:
                moveFlg = false;
                break;

            case MoveType.SWIM:
                moveFlg = false;
                break;
        }

        Debug.Log(currentMoveType);
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
            if(moveType == MoveType.TURN)
            {
                currentMoveType = (MoveType)Random.Range((int)MoveType.WALK, (int)MoveType.RUN + 1);
                return;
            }
            rb.velocity = Vector3.zero;
            //一通りの処理を終えたら行動を書き込む
            moveType = MoveType.IDLE;
            moveFlg = false;
        }
    }

	private void Walk()
	{
        //速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
														endPos,
                                                        BoothAnimalManager.Instance.penguinsData.dataList[movedata].walkSpeed * Time.deltaTime);
		//最期まで動いたら初期化し、アイドルに移る
		if(this.transform.position == endPos)
		{
            //最終地点の座標に行かないように記憶しておく
            rb.velocity = Vector3.zero;
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType= MoveType.WALK;
        }

    }

	private void Run()
	{

		//速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
                                                        endPos,
                                                        BoothAnimalManager.Instance.penguinsData.dataList[movedata].runSpeed * Time.deltaTime);
        //最期まで動いたら初期化し、アイドルに移る
        if (this.transform.position == endPos)
        {
            //最終地点の座標に行かないように記憶しておく
            rb.velocity = Vector3.zero;
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType= MoveType.RUN;
        }
    }

	private void Turn()
	{
        var endRot = Quaternion.LookRotation(endPos - this.transform.position);
        var befRot = this.transform.rotation;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, endRot, BoothAnimalManager.Instance.penguinsData.dataList[movedata].rotateAngle);

        rb.velocity = Vector3.zero;

        if (befRot == this.transform.rotation)
		{
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType= MoveType.TURN;
        }
    }
	//動きを決める関数
	private void MoveEnter()
	{
		//動き決定の範囲
        currentMoveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.TURN + 1);
        moveFlg = true;
        if(currentMoveType == MoveType.IDLE && moveType == MoveType.IDLE)
        {
            currentMoveType = (MoveType)Random.Range((int)MoveType.IDLE + 1, (int)MoveType.TURN + 1);
        }
		if(currentMoveType == MoveType.TURN)
		{
            //終了地点がなるべくかぶらないようにするため違う場所になるまで回す。
            while (currentMoveIndex == moveIndex)
            {
                currentMoveIndex = Random.Range(0, BoothAnimalManager.Instance.penguinsData.rangeList.Count);
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
        var startPoint = Random.insideUnitCircle * BoothAnimalManager.Instance.penguinsData.rangeArea;

        //決めた範囲分動かした場所を決定し返す
        retVector = new Vector3(BoothAnimalManager.Instance.penguinsData.rangeList[index].x + startPoint.x,
                                BoothAnimalManager.Instance.penguinsData.rangeList[index].y,
                                BoothAnimalManager.Instance.penguinsData.rangeList[index].z + startPoint.y);
		return retVector;
    }

    private void Pause()
    {
        pauseFlg = true;
    }

    private void ReGame()
    {
        pauseFlg = false;
    }
}
