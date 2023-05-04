//=============================================================================
// @File	: [BearMove.cs]
// @Brief	: 熊の動き
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BearMove : MonoBehaviour
{
    enum MoveType
    {
        WALK,
        RUN,
        IDLE,
        TURN,
		GIMMICK,
        MAX_MOVE
    }

    private Animator anim;                  //アニメーター格納用

    private MoveType currentMoveType;       //現在の動き
    private MoveType moveType;              //一つ前にした動き

    private bool moveFlg;                   //動いているかどうかのフラグ

    private int movedata;                   //動きのデータの情報の数値
    private int currentMoveIndex;           //動きの数値
    private int moveIndex;                  //現在の動きの数値

    //終了地点を格納しておく変数F
    private Vector3 endPos;

    private Rigidbody rb;

    //動いてるときの時間
    private float moveTime;

    //ポーズ用フラグ
    private bool pauseFlg;

    //ターン用フラグ
    private bool turnFlg;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        currentMoveType = MoveType.IDLE;
        moveType = MoveType.IDLE;
        moveFlg = false;
        turnFlg = false;
        moveTime = 0.0f;
        movedata = Random.Range(0, BoothAnimalManager.Instance.bearData.dataList.Count);
        rb = this.GetComponent<Rigidbody>();
        currentMoveIndex = BoothAnimalManager.Instance.bearStartIndex;

        anim = this.GetComponent<Animator>();

        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { ReGame(); }).AddTo(gameObject);
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
		if(pauseFlg)
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
            case MoveType.GIMMICK:

                break;


        }
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (pauseFlg)
        {
            return;
        }
    }

    private void Idle()
    {
        //〇秒～〇秒の間でアイドルする。
        moveTime += Time.deltaTime;
        rb.velocity = Vector3.zero;
        if (moveTime >= 3.0f)
        {
            moveIndex = currentMoveIndex;
            moveTime = 0.0f;

            //一通りの処理を終えたら行動を書き込む
            moveType = MoveType.IDLE;
            moveFlg = false;
        }
    }

    private void Walk()
    {
        //速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(this.transform.position,
                                                        endPos,
                                                        BoothAnimalManager.Instance.bearData.dataList[movedata].walkSpeed * Time.deltaTime);
        rb.velocity = Vector3.zero;
        //最期まで動いたら初期化し、アイドルに移る
        if (PositionAreaJudge(this.transform.position, endPos, 0.5f))
        {
            //最終地点の座標に行かないように記憶しておく
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType = MoveType.WALK;
        }

    }

    private void Run()
    {

        //速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(this.transform.position,
                                                        endPos,
                                                        BoothAnimalManager.Instance.bearData.dataList[movedata].runSpeed * Time.deltaTime);
        rb.velocity = Vector3.zero;
        //最期まで動いたら初期化し、アイドルに移る
        if (PositionAreaJudge(this.transform.position, endPos, 0.5f))
        {
            //最終地点の座標に行かないように記憶しておく

            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType = MoveType.RUN;
        }
    }

    private void Turn()
    {
        var endRot = Quaternion.LookRotation(endPos - this.transform.position);
        var befRot = this.transform.rotation;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, endRot, BoothAnimalManager.Instance.bearData.dataList[movedata].rotateAngle);

        rb.velocity = Vector3.zero;

        if (befRot == this.transform.rotation)
        {
            moveIndex = currentMoveIndex;
            moveFlg = false;
            turnFlg = true;
            moveType = MoveType.TURN;
        }
    }

    private void MoveEnter()
    {
        if(moveType == MoveType.GIMMICK)
        {
            return;
        }
        if (turnFlg == true)
        {
            currentMoveType = (MoveType)Random.Range((int)MoveType.WALK, (int)MoveType.RUN + 1);
            turnFlg = false;
        }
        else
        {
            currentMoveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.TURN + 1);
        }

        moveFlg = true;

        if (currentMoveType == MoveType.TURN)
        {

            //終了地点がなるべくかぶらないようにするため違う場所になるまで回す。
            if (currentMoveIndex  < BoothAnimalManager.Instance.bearData.rangeButtom)
            {
                while (currentMoveIndex == moveIndex)
                    currentMoveIndex = Random.Range(0, BoothAnimalManager.Instance.bearData.rangeButtom + 1);
            }
            else if(currentMoveIndex == BoothAnimalManager.Instance.bearData.rangeButtom)
            {
                while (currentMoveIndex == moveIndex)
                    currentMoveIndex = Random.Range(0, BoothAnimalManager.Instance.bearData.rangeButtom + 2);
            }
            else if(currentMoveIndex == BoothAnimalManager.Instance.bearData.rangeButtom + 1)
            {
                while (currentMoveIndex == moveIndex)
                    currentMoveIndex = Random.Range(BoothAnimalManager.Instance.bearData.rangeButtom,
                                                    BoothAnimalManager.Instance.bearData.rangeList.Count);
            }
            else if(currentMoveIndex > BoothAnimalManager.Instance.bearData.rangeButtom + 1)
            {
                while (currentMoveIndex == moveIndex)
                    currentMoveIndex = Random.Range(BoothAnimalManager.Instance.bearData.rangeButtom + 1, 
                                                    BoothAnimalManager.Instance.bearData.rangeList.Count);
            }

            Debug.Log("きまったで"+currentMoveIndex);
            //終了地点が決まったらランダムで若干ずらす。
            endPos = CircleRandamaiser(currentMoveIndex);

        }
    }

    //0から半径分の範囲を取るためそれを指定された座標と計算することで若干位置をずらせる。
    private Vector3 CircleRandamaiser(int index)
    {
        var retVector = new Vector3();
        //範囲を決める。
        var startPoint = Random.insideUnitCircle * BoothAnimalManager.Instance.bearData.rangeArea;



        //決めた範囲分動かした場所を決定し返す
        retVector = new Vector3(BoothAnimalManager.Instance.bearData.rangeList[index].x + startPoint.x,
                                BoothAnimalManager.Instance.bearData.rangeList[index].y,
                                BoothAnimalManager.Instance.bearData.rangeList[index].z + startPoint.y);
        return retVector;
    }

    public bool PositionAreaJudge(Vector3 target, Vector3 end, float Area)
    {
        //左上と右下を取ってその中に到着しているかを判別する
        var endLT = end;
        var endRB = end;

        endLT.x = end.x + Area;
        endLT.z = end.z + Area;
        endRB.x = end.x - Area;
        endRB.z = end.z - Area;

        if (target.x <= endLT.x && target.z <= endLT.z && target.x >= endRB.x && target.z >= endRB.z)
        {
            return true;
        }

        return false;
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