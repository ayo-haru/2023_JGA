//=============================================================================
// @File	: [MainCamera.cs]
// @Brief	: メインカメラの挙動
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 参考URL カメラ追従 https://programming.sincoston.com/unity-camera-follow-player/
//					　スムーズに動くカメラ https://nekojara.city/unity-smooth-damp
//                    animationカーブ https://kan-kikuchi.hatenablog.com/entry/AnimationCurve_nspector%BC%E3%83%AB%E3%83%90%E3%83%83%E3%82%AF%E3%81%A8%E3%81%AF
//					  
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	遅延作成
// 2023/03/02   ズームの処理作成(切替対応済み)
// 2023/03/10   客の人数でズームアウトする処理の変更
//=============================================================================ya
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using System.Linq;

public class MainCamera : MonoBehaviour
{
    enum ZOOM
    {
        
        DEFAULT = 0,
        IN,
        OUT,
        GUESTOUT,
        KEEPEROUT,
    }
    //ズームの状況
    ZOOM zoom = ZOOM.DEFAULT;
    ZOOM currentZoom = ZOOM.DEFAULT;


    //インプットアクション
    [SerializeField] private InputActionReference cameraZoomIn;
    [SerializeField] private InputActionReference cameraZoomOut;

    //プレイヤー追従用のプレイヤー取得
    [SerializeField]private GameObject targetObject;
    //プレイヤーの座標変更取得
    private Vector3 currentPlayerPos;
	//カメラとプレイヤーの座標の初期
	private Vector3 offset;
	//初期FOV格納用
	private float fov;
    //計算用FOV格納用
    private float currentFov;

    //ポーズ用のフラグ
    private bool pauseFlg;

    //客の情報取得用
    GameObject[] guestObj;
    GameObject[] zooKeeperObj;

    private int crearGuest;
    private int guestCheck;

    //カメラの範囲取得用
    //映っているものの範囲
    Bounds[] boundGuest;
    Bounds[] boundKeeper;
    //カメラの範囲
    Plane[] planes;

    private Vector3 keeperMiddle;
    private bool keeperFlg;

    //イージング実行中の現在の割合
    private float easingRate;
    //Lerpの数値を割合として保存する
    private float lerpRate;
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

    //カメラの初期位置格納用
    private Vector3 firstCamPos;

    //基本的にメインカメラのみで設定するところ
    [Header("遅延の数値(0に近いほど遅延がデカい。基本は1)")]
    [SerializeField] private float smoothMove = 1.0f;

    //画面端から何メートルでカメラを止まるようにするか
    [Header("画面端何メートルでカメラを停止させるか")]
    [SerializeField] private float edgeDistance = 5.0f;


    [Header("ズームイン倍率")]
    [SerializeField] private float zoomIn = 0.1f; 
    [Header("ズームアウト倍率")]
    [SerializeField] private float zoomOut = 0.1f;
    [Header("キー入力のズームイン、アウトの時間")]
    [SerializeField] private float zoomTime = 1.0f;
    [Header("ズームから戻ってくる時間")]
    [SerializeField] private float zoomRetTime = 1.0f;
    [Header("カメラズームののイージング設定")]
    [SerializeField] private AnimationCurve moveCurve = null;
    [Header("客の人数でズームアウトする時間")]
    [SerializeField] private float guestZoomOut = 1.0f;
    [Header("客のズームから戻ってくる時間")]
    [SerializeField] private float guestRetTime = 1.0f;
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
        //ポーズ用
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { ReGame(); }).AddTo(gameObject);

        crearGuest = 0;

        //カメラの情報の受け取り
        cameraObj = GameObject.Find("CameraParent").GetComponent<CameraManager>();
        //初期化としてカメラの情報を格納
        maincamera = cameraObj.GetTransformObject();

		cameraParent = cameraObj.GetTransformObject(true);
		cameraChild = cameraObj.GetTransformObject(false);

        //カメラの範囲取得
        planes = GeometryUtility.CalculateFrustumPlanes(maincamera);

        ////インプットアクション設定
        cameraZoomIn.action.started += OnZoomIn;
        cameraZoomIn.action.performed += OnZoomIn;
        cameraZoomIn.action.canceled += OnZoomIn;
        cameraZoomOut.action.started += OnZoomOut;
        cameraZoomOut.action.performed += OnZoomOut;
        cameraZoomOut.action.canceled += OnZoomOut;

        cameraZoomIn.ToInputAction().Enable();
        cameraZoomOut.ToInputAction().Enable();
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start() {

        guestCheck = GameData.guestData.Length;

        crearGuest = GameData.guestCnt;
        //客の情報を格納する
        guestObj = GameObject.FindGameObjectsWithTag("Guest");

        //客の範囲取得
        boundGuest = new Bounds[guestObj.Length];

        zooKeeperObj = GameObject.FindGameObjectsWithTag("ZooKeeper");
        boundKeeper = new Bounds[zooKeeperObj.Length];

        // 注視点座標をプレイヤーの座標に
        targetObject = GameObject.FindGameObjectWithTag("Player");
        firstCamPos = targetObject.transform.position;
        //プレイヤーの初期位置とカメラの座標を固定
        offset = cameraParent.transform.position - targetObject.transform.position;
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
        lerpRate = 0.0f;

    }

    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
        //一定時間の計算
        scriptStop += Time.deltaTime;

        if (crearGuest != GameData.guestCnt)
        {
            guestObj = GameObject.FindGameObjectsWithTag("Guest");
            //客の範囲取得
            boundGuest = new Bounds[guestObj.Length];
            crearGuest = GameData.guestCnt;
        }
        if (guestCheck != GameData.guestData.Length + GameData.randomGuestCnt)
        {
            guestObj = GameObject.FindGameObjectsWithTag("Guest");
            //客の範囲取得
            boundGuest = new Bounds[guestObj.Length];
            guestCheck = GameData.guestData.Length + GameData.randomGuestCnt;
        }

        if (scriptStop >= 1.0f)
        {
            
            GuestCount();
            if (currentGuestValue >= guestValue)
            {
                currentZoom = ZOOM.GUESTOUT;
                zoomFlg = true;
            }
            
            scriptStop = 0.0f;
        }
        KeeperCount();
        CameraMove();
        if (zoomFlg)
        {
            ZoomInOut();
        }

    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (pauseFlg && !GameData.isCatchPenguin)
        {
            return;
        }
        if (GameData.isCatchPenguin)
        {
            cameraParent.position = firstCamPos;
        }
    }
    
    /// <summary>
    /// カメラの動き自体の処理
    /// </summary>
    private void CameraMove()
	{
        if(pauseFlg && !GameData.isCatchPenguin)
         {
             return;
         }
        if (keeperFlg && currentZoom != ZOOM.GUESTOUT)
        {
            cameraParent.position = Vector3.Lerp(
            a: cameraParent.position,
            b: keeperMiddle,
            t: Time.deltaTime * smoothMove);
        }
        else
        {
            cameraParent.position = Vector3.Lerp(
            a: cameraParent.position,
            b: targetObject.transform.localPosition,
            t: Time.deltaTime * smoothMove);
        }

    }
    //ズームインのボタンが押されたときに実行する関数
    private void OnZoomIn(InputAction.CallbackContext context)
    {
        if(currentZoom == ZOOM.GUESTOUT)
        {
            return;
        }
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
        if (currentZoom == ZOOM.GUESTOUT)
        {
            return;
        }
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
        if (pauseFlg && !GameData.isCatchPenguin)
        {
            return;
        }
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

            case ZOOM.GUESTOUT:
                if(currentGuestValue < guestValue)
                {
                    currentZoom = ZOOM.DEFAULT;
                }
                if (currentFov < fov * zoomOut)
                {
                    //ズームのイージング
                    easingRate += 1.0f / (60.0f * guestZoomOut);
                    lerpRate = moveCurve.Evaluate(easingRate);
                    if (easingRate >= 1.0f)
                    {
                        easingRate = 0.0f;
                    }
                    currentFov = Mathf.Lerp(fov, fov * zoomOut, lerpRate);
                }
                zoom = ZOOM.GUESTOUT;
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
                if(zoom == ZOOM.GUESTOUT)
                {
                    if (currentFov > fov)
                    {
                        currentFov -= ((zoomOut * fov - fov) / (60.0f * guestRetTime));
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
    public void GuestCount()
    {
        var nowGuestCount = 0;
       
        //移動先の座標取得
        for (int i = 0; i < guestObj.Length; i++)
        {
            boundGuest[i].center = guestObj[i].transform.position;
            boundGuest[i].size *= 0.1f;
        }
        planes = GeometryUtility.CalculateFrustumPlanes(maincamera);
        
        //一つ一つ判定数を計算する
        for (int i = 0; i < guestObj.Length; i++)
        {
            bool isRendered = GeometryUtility.TestPlanesAABB(planes,boundGuest[i]);
            if (isRendered)
            {
                nowGuestCount++;
            }
        }
        currentGuestValue = nowGuestCount;
    }
   
    private void KeeperCount()
    {
        keeperFlg = false;
        
        //移動先の座標取得
        for (int i = 0; i < zooKeeperObj.Length; i++)
        {
            boundKeeper[i].center = zooKeeperObj[i].transform.position;
            boundKeeper[i].size *= 0.2f;
        }
        planes = GeometryUtility.CalculateFrustumPlanes(maincamera);

        var distance = new float[zooKeeperObj.Length];
        //一つ一つ判定数を計算する
        for (int i = 0; i < zooKeeperObj.Length; i++)
        {
            distance[i] = 9999.0f;
            bool isRendered = GeometryUtility.TestPlanesAABB(planes, boundKeeper[i]);
            if (isRendered)
            {
                distance[i] = (Vector3.Distance(targetObject.transform.position, zooKeeperObj[i].transform.position));
                keeperFlg = true;
            }
        }
        var min = Mathf.Min(distance);
        if (min >= 25)
        {
            keeperFlg = false;
        }
        if (keeperFlg == true)
        {
            for (int i = 0; i < distance.Length; i++)
            {
             
                if (min == distance[i])
                {
                    keeperMiddle = Vector3.Lerp(targetObject.transform.localPosition, zooKeeperObj[i].transform.localPosition, 0.5f);
                }
            }
        }
        else
        {
            keeperMiddle = new Vector3();
        }
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
