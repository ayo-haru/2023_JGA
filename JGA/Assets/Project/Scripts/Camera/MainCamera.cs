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

    //ポーズ用のフラグ
    private bool pauseFlg;

    //客の情報取得用
    GameObject[] guestObj;
    

    //カメラの範囲取得用
    //映っているものの範囲
    Bounds[] boundGuest;
    //カメラの範囲
    Plane[] planes;

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

    //画面端の座標格納用
    [Header("フィールド左端")]
    [SerializeField] private Transform fieldLeftEdge = null;
    [Header("フィールド右端")]
    [SerializeField] private Transform fieldRightEdge = null;
    [Header("フィールド上端")]
    [SerializeField] private Transform fieldUpEdge = null;
    [Header("フィールド下端")]
    [SerializeField] private Transform fieldBottomEdge = null;

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

        //カメラの情報の受け取り
        cameraObj = GameObject.Find("CameraParent").GetComponent<CameraManager>();
        //初期化としてカメラの情報を格納
        maincamera = cameraObj.GetTransformObject();
		cameraParent = cameraObj.GetTransformObject(true);
		cameraChild = cameraObj.GetTransformObject(false);

        //客の情報を格納する
        guestObj = GameObject.FindGameObjectsWithTag("Guest");

        //客の範囲取得
        boundGuest = new Bounds[guestObj.Length];
        //カメラの範囲取得
        planes = GeometryUtility.CalculateFrustumPlanes(maincamera);

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
	void Start() {        
        //プレイヤーを格納
        playerobj = GameObject.Find("LookPos");
        firstCamPos = playerobj.transform.position;
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
        lerpRate = 0.0f;

    }

    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
        //一定時間の計算
        scriptStop += Time.deltaTime;
        
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
        if (pauseFlg)
        {
            return;
        }
        if (MySceneManager.GameData.isCatchPenguin)
        {
            cameraParent.position = firstCamPos;
        }
    }
    
    /// <summary>
    /// カメラの動き自体の処理
    /// </summary>
    private void CameraMove()
	{
        //if(fieldLeftEdge == null || fieldRightEdge == null || fieldUpEdge == null || fieldBottomEdge == null)
        //{
        //    return;
        //}
        if(pauseFlg)
         {
             return;
         }
        //==============================================================
        //端＋指定した距離にプレイヤーが入っていたらカメラを止める
        //if (fieldLeftEdge.position.x + edgeDistance >= playerobj.transform.position.x ||
        //    fieldRightEdge.position.x - edgeDistance <= playerobj.transform.position.x)
        //{
        //    if (fieldUpEdge.position.z - edgeDistance <= playerobj.transform.position.z ||
        //    fieldBottomEdge.position.z + edgeDistance >= playerobj.transform.position.z)
        //    {
        //        return;
        //    }
        //        var z = Mathf.Lerp(
        //            a: cameraParent.position.z,
        //            b: playerobj.transform.position.z,
        //            t: Time.deltaTime * smoothMove);

        //    cameraParent.position = new Vector3(cameraParent.position.x, cameraParent.position.y, z);

        //}
        //if(fieldUpEdge.position.z - edgeDistance <= playerobj.transform.position.z ||
        //     fieldBottomEdge.position.z + edgeDistance >= playerobj.transform.position.z)
        //{
        //    if (fieldLeftEdge.position.x + edgeDistance >= playerobj.transform.position.x ||
        //    fieldRightEdge.position.x - edgeDistance <= playerobj.transform.position.x)
        //    {
        //        return;
        //    }
        //        var x = Mathf.Lerp(
        //            a: cameraParent.position.x,
        //            b: playerobj.transform.position.x,
        //            t: Time.deltaTime * smoothMove);

        //    cameraParent.position = new Vector3(x, cameraParent.position.y, cameraParent.position.z);
        //}

        //if(fieldLeftEdge.position.x + edgeDistance <= playerobj.transform.position.x &&
        //    fieldRightEdge.position.x - edgeDistance >= playerobj.transform.position.x &&
        //    fieldUpEdge.position.z - edgeDistance >= playerobj.transform.position.z &&
        //    fieldBottomEdge.position.z + edgeDistance <= playerobj.transform.position.z)
        //{
            cameraParent.position = Vector3.Lerp(
            a: cameraParent.position,
            b: playerobj.transform.position,
            t: Time.deltaTime * smoothMove);
        //}
        //==============================================================

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
        if (pauseFlg)
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
   
    private void Pause()
    {
        pauseFlg = true;
    }

    private void ReGame()
    {
        pauseFlg = false;
    }
}
