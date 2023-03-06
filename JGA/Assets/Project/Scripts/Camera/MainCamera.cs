//=============================================================================
// @File	: [MainCamera.cs]
// @Brief	: メインカメラの挙動
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 参考URL カメラ追従 https://programming.sincoston.com/unity-camera-follow-player/
//					　スムーズに動くカメラ https://nekojara.city/unity-smooth-damp
//                    animationカーブ https://kan-kikuchi.hatenablog.com/entry/AnimationCurve_nspector
//                    コールバック https://nn-hokuson.hatenablog.com/entry/2021/09/02/114100#%E3%82%B3%E3%83%BC%E3%83%AB%E3%83%90%E3%83%83%E3%82%AF%E3%81%A8%E3%81%AF
//					  
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	遅延作成
// 2023/03/02   ズームの処理作成(切替対応済み)
//=============================================================================ya
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    enum ZOOM
    {
        
        DEFAULT = 0,
        IN,
        OUT,
        GUESTOUT,
    }
    //ズームの状況
    ZOOM zoom = ZOOM.DEFAULT;
    ZOOM currentZoom = ZOOM.DEFAULT;


    //インプットアクション
    private MyContorller gameInput;
    //プレイヤー追従用のプレイヤー取得
    private GameObject playerobj;
    //プレイヤーの座標変更取得
    private Vector3 currentPlayerPos;
	//カメラとプレイヤーの座標の初期
	private Vector3 offset;
	//初期FOV格納用
	private float fov;
    //計算用FOV格納用
    private float currentFov;

    //画面端の座標格納用
    private Vector3 rightTop;
    private Vector3 leftBottom;

    private Vector3 rightBottom;
    private Vector3 leftTop;

    //客の情報取得用
    GameObject[] guestObj;

    //イージング実行中の現在の割合
    private float easingRate;
    //ズームインアウトの実行中フラグ
    private bool zoomFlg;
    //現在の客の数
    private int currentGuestValue;
    //処理を一定時間ごとに実行させるためのもの
    private float scriptStop;

    //カメラの情報受け取り用
    private CameraManager cameraObj;
    //メインカメラ格納用
    private Camera maincamera;
    //座標角度格納用
    private Transform cameraParent;
	//距離格納用
	private Transform cameraChild;

    //基本的にメインカメラのみで設定するところ
    [Header("到着までの大体の時間")]
    [SerializeField] private float smoothTime = 0.1f;
    [Header("最高速度")]
    [SerializeField] private float maxSpeed = 10.0f;
    [Header("カメラ移動のイージング設定")]
    [SerializeField] private AnimationCurve moveCurve = null;

    [Header("ズームイン倍率")]
    [SerializeField] private float zoomIn = 0.1f;
    [Header("ズームアウト倍率")]
    [SerializeField] private float zoomOut = 0.1f;
    [Header("ズームイン、アウトの時間")]
    [SerializeField] private float zoomTime = 1.0f;
    [Header("ズームから戻ってくる時間")]
    [SerializeField] private float zoomRetTime = 1.0f;
    //客が何人いたら引くかを指定できる変数
    [Header("客が何人いたらズームアウトするか")]
    [SerializeField] private int guestValue = 5;


    //テスト用
    [Header("デバッグ用ズームの方式を変更(false FOV , true position)")]
    [SerializeField] private bool zoomObjChange = false;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        //カメラの情報の受け取り
        cameraObj = GameObject.Find("CameraParent").GetComponent<CameraManager>();
        //プレイヤーを格納
        playerobj = GameObject.FindGameObjectWithTag("Player");
        //初期化としてカメラの情報を格納
        maincamera = cameraObj.GetTransformObject();
		cameraParent = cameraObj.GetTransformObject(true);
		cameraChild = cameraObj.GetTransformObject(false);
        //スクリーンの端を取る
        var screenEnd = cameraParent.localPosition.y - playerobj.transform.position.y;
        rightTop = maincamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,screenEnd));
        leftBottom = maincamera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, screenEnd));

        //客の情報を格納する
        guestObj = GameObject.FindGameObjectsWithTag("Scenery");

        gameInput = new MyContorller();
        //インプットアクション設定
        gameInput.Camera.ZoomIn.started += OnZoomIn;
        gameInput.Camera.ZoomIn.performed+= OnZoomIn;
        gameInput.Camera.ZoomIn.canceled += OnZoomIn;
        gameInput.Camera.ZoomOut.started += OnZoomOut;
        gameInput.Camera.ZoomOut.performed += OnZoomOut;
        gameInput.Camera.ZoomOut.canceled += OnZoomOut;

        gameInput.Enable();
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        //プレイヤーの初期位置とカメラの座標を固定
        offset = cameraParent.transform.position - playerobj.transform.position;
        //初期の視野角を格納
        if (!zoomObjChange)
        {
            fov = maincamera.fieldOfView;
            currentFov = maincamera.fieldOfView;
        }
        if(zoomObjChange)
        {
            fov = -cameraChild.transform.localPosition.z;
            currentFov = -cameraChild.transform.localPosition.z;
        }
        //イージング用の時間
        easingRate = 0.0f;
        //プレイヤーの座標保存
        currentPlayerPos = playerobj.transform.position;
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        //一定時間の計算
        scriptStop += Time.deltaTime;
        if(scriptStop >= 1.0f)
        {
            var screenEnd = 30.0f;
            
            rightTop = maincamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, screenEnd));
            leftBottom = maincamera.ScreenToWorldPoint(new Vector3(0, 0, screenEnd));
            rightBottom = maincamera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, screenEnd));
            leftTop = maincamera.ScreenToWorldPoint(new Vector3(0, Screen.height, screenEnd));

            GuestCount();
            scriptStop = 0.0f;
            Debug.Log(currentGuestValue + "人");
            Debug.Log(rightTop + "右上");
            Debug.Log(leftBottom + "左下");
            Debug.Log(rightBottom + "右下");
            Debug.Log(leftTop + "左うえ");
        }


        CameraMove();
        if(zoomFlg)
        {
            ZoomInOut();
        }
        
        //ズームすることを伝え計算させる
        if (currentGuestValue >= guestValue)
        {
            currentZoom = ZOOM.OUT;
            zoomFlg = true;
        }
        
        
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        
    }
    
    /// <summary>
    /// カメラの動き自体の処理
    /// </summary>
    private void CameraMove()
	{
        //カメラの座標の更新
        var targetpos = playerobj.transform.position + offset;
        //if (currentPlayerPos != playerobj.transform.position)
        //{
        //    easingRate += Time.deltaTime;
        //    if (easingRate <= 1.0f)
        //    {
        //        cameraParent.transform.position = 
        //            Vector3.Lerp(cameraParent.transform.position, targetpos, moveCurve.Evaluate(easingRate));
        //    }

        //}
        //currentPlayerPos = playerobj.transform.position;

        var currentVelocity = new Vector3();


        cameraParent.transform.position = Vector3.SmoothDamp(cameraParent.transform.position,
                                                            targetpos,
                                                           ref currentVelocity,
                                                           smoothTime,
                                                           maxSpeed);

    }
    //ズームインのボタンが押されたときに実行する関数
    private void OnZoomIn(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //押しているとき
            case InputActionPhase.Performed:
                //片方のボタンを押しているときにもう片方のボタンを実行させないための例外処理
                if (currentZoom == ZOOM.OUT) { break; }
                currentZoom = ZOOM.IN;
                zoomFlg = true;
                break;
            //離したとき
            case InputActionPhase.Canceled:
                if (currentZoom == ZOOM.OUT) { break; }
                currentZoom = ZOOM.DEFAULT;
                break;
        }

    }
    //ズームアウトのボタンが押されたときに実行する関数
    private void OnZoomOut(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //押しているとき
            case InputActionPhase.Performed:
                if(currentZoom == ZOOM.IN) { break; }
                currentZoom = ZOOM.OUT;
                zoomFlg = true;
                break;
            //離したとき
            case InputActionPhase.Canceled:
                if (currentZoom == ZOOM.IN) { break; }
                currentZoom = ZOOM.DEFAULT;
                break;
        }

    }

    //ズームインとズームアウトを決定動かす処理
    private void ZoomInOut()
	{
      
        switch (currentZoom)
        {
            case ZOOM.IN:
                if(currentFov > fov * zoomIn)
                {
                    currentFov -= ((fov - zoomIn * fov) / (60.0f * zoomTime));
                }
                zoom = ZOOM.IN;
                break;
                
            case ZOOM.OUT:
                if (currentFov < fov * zoomOut)
                {
                    currentFov += ((zoomOut * fov - fov) / (60.0f * zoomTime));
                }
                zoom = ZOOM.OUT;
                break;

            case ZOOM.DEFAULT:
                if (zoom == ZOOM.IN)
                {
                    if (currentFov < fov)
                    {
                        currentFov += ((fov - zoomIn * fov) / (60.0f * zoomRetTime));
                    }
                    else
                    {
                        zoomFlg = false;
                    }
                    break;
                }
                if (zoom == ZOOM.OUT)
                {
                    if (currentFov > fov)
                    {
                        currentFov -= ((zoomOut * fov - fov) / (60.0f * zoomRetTime));
                    }
                    else
                    {
                        zoomFlg = false;
                    }
                    break;
                }
                break;
        }

            if (!zoomObjChange)
                maincamera.fieldOfView = currentFov;
            if (zoomObjChange)
                cameraChild.localPosition = new Vector3(0.0f, 0.0f, -currentFov);
        
    }
    /// <summary>
    /// ゲストの数の増減
    /// </summary>
    /// <param name="math">true Add false del</param>
    public void GuestCount()
    {
        var nowGuestCount = 0;
        for(int i = 0; i < guestObj.Length; i++)
        {
            if (guestObj[i].transform.position.x <= rightTop.x && guestObj[i].transform.position.z <= rightTop.z &&
                guestObj[i].transform.position.x >= leftBottom.x && guestObj[i].transform.position.z >= leftBottom.z)
            {
                nowGuestCount++;
            }
        }
        currentGuestValue = nowGuestCount;
    }
   
}
