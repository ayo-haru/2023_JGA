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
// 2023/04/14   アニメーションを追加
// 2023/04/24   クリア用演出追加
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
        APPEAL,
        JUMP,
		SWIM,
        CLEAR_APPEAL,
		MAX_MOVE
	}

    private Animator anim;                  //アニメーター格納用

	private MoveType currentMoveType;       //現在の動き
	private MoveType moveType;              //一つ前にした動き

	private bool moveFlg;                   //動いているかどうかのフラグ
	
	private int movedata;                   //動きのデータの情報の数値
	private int currentMoveIndex;           //動きの数値
	private int moveIndex;                  //現在の動きの数値

    private Vector3 endPos;                 //終了地点を格納しておく変数

    private Rigidbody rb;                   //押し合う用

	private float moveTime;                 //動いてるときの時間

    private bool pauseFlg;                  //ポーズ用フラグ

    private bool turnFlg;                   //ターン用フラグ

    //クリア用に使う変数
    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{

        currentMoveType = MoveType.IDLE;
		moveType    = MoveType.IDLE;
		moveFlg = false;
        turnFlg = false;
		moveTime = 0.0f;
		movedata = Random.Range(0, BoothAnimalManager.Instance.penguinsData.dataList.Count);
        rb = this.GetComponent<Rigidbody>();

        anim = this.GetComponent<Animator>();

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
        //----- 客人数カウントUIの取得 -----
        guestNumUI = GameObject.Find("GuestNumUI");
        if (guestNumUI)
        {
            _GuestNumUI = guestNumUI.GetComponent<GuestNumUI>();
        }
        else
        {
            Debug.LogWarning("GuestNumUIがシーン上にありません");
        }

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
        //----- ゲームクリア -----
        if (guestNumUI)
        {
            if (_GuestNumUI.isClear())
            {
                currentMoveType = MoveType.CLEAR_APPEAL;
                CrearAppeal();
            }
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
                Appeal();
                break;
            case MoveType.SWIM:
                moveFlg = false;
                break;
        }
      
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
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
														endPos,
                                                        BoothAnimalManager.Instance.penguinsData.dataList[movedata].walkSpeed * Time.deltaTime);
        rb.velocity = Vector3.zero;
        //最期まで動いたら初期化し、アイドルに移る
        if (PositionAreaJudge(this.transform.position, endPos, 0.5f))
		{
            //最終地点の座標に行かないように記憶しておく
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType= MoveType.WALK;
            anim.SetBool("WalkFlg", false);
        }

    }

	private void Run()
	{

		//速度を決定してその速度で終了地点まで動く
        this.transform.position = Vector3.MoveTowards(	this.transform.position,
                                                        endPos,
                                                        BoothAnimalManager.Instance.penguinsData.dataList[movedata].runSpeed * Time.deltaTime);
        rb.velocity = Vector3.zero;
        //最期まで動いたら初期化し、アイドルに移る
        if (PositionAreaJudge(this.transform.position, endPos, 0.5f))
        {
            //最終地点の座標に行かないように記憶しておく
            
            moveIndex = currentMoveIndex;
            currentMoveType = MoveType.IDLE;
            moveType= MoveType.RUN;
            anim.SetBool("RunFlg", false);
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
            moveFlg = false;
            turnFlg = true;
            moveType = MoveType.TURN;
            anim.SetBool("TurnFlg", false);
        }
    }

    private void Appeal()
    {
        //〇秒～〇秒の間でアピールする。
        moveTime += Time.deltaTime;
        rb.velocity = Vector3.zero;
        if (moveTime >= 2.0f)
        {
            moveIndex = currentMoveIndex;
            moveTime = 0.0f;
            
            //一通りの処理を終えたら行動を書き込む
            moveType = MoveType.APPEAL;
            moveFlg = false;
            anim.SetBool("AppealFlg", false);
        }
    }

    private void CrearAppeal()
    {
        anim.SetBool("WalkFlg", false);
        anim.SetBool("RunFlg", false);
        anim.SetBool("TurnFlg", false);
        anim.SetBool("AppealFlg", true);
    }


    //動きを決める関数
    private void MoveEnter()
	{
        //動き決定の範囲
        //アピールばかりだとちょっとむかついたので一回やったら最低でも一回は違う行動に出る
        if (turnFlg == true)
        {
            currentMoveType = (MoveType)Random.Range((int)MoveType.WALK, (int)MoveType.RUN + 1);
            turnFlg = false;
        }
        else
        {
            if (moveType != MoveType.APPEAL)
                currentMoveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.APPEAL + 1);
            else
                currentMoveType = (MoveType)Random.Range((int)MoveType.IDLE, (int)MoveType.TURN + 1);
        }

        moveFlg = true;
        if (currentMoveType == MoveType.WALK)
        {
            anim.SetBool("WalkFlg", true);
        }
        if (currentMoveType == MoveType.RUN)
        {
            anim.SetBool("RunFlg", true);
        }
        if (currentMoveType == MoveType.APPEAL)
        {
            anim.SetBool("AppealFlg", true);
        }
        if (currentMoveType == MoveType.TURN)
		{
            anim.SetBool("TurnFlg", true);
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

    public bool PositionAreaJudge(Vector3 target,Vector3 end , float Area)
    {
        //左上と右下を取ってその中に到着しているかを判別する
        var endLT = end;
        var endRB = end;

        endLT.x = end.x + Area;
        endLT.z = end.z + Area;
        endRB.x = end.x - Area;
        endRB.z = end.z - Area;

        if(target.x <= endLT.x && target.z <= endLT.z && target.x >= endRB.x && target.z >= endRB.z)
        {
            return true;
        }

        return false;
    }

    private void Pause()
    {
        pauseFlg = true;
        anim.speed = 0.0f;
    }

    private void ReGame()
    {
        pauseFlg = false;
        anim.speed = 1.0f;
    }
}
