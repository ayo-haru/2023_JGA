//=============================================================================
// @File	: [MainCamera.cs]
// @Brief	: メインカメラの挙動
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 参考URL https://programming.sincoston.com/unity-camera-follow-player/
//					　https://nekojara.city/unity-smooth-damp
//					  
// 
// [Date]
// 2023/02/27	スクリプト作成
// 2023/02/28	遅延作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    enum ZOOM
    {
        NORMAL = 0,
        IN,
        OUT,
    }

    ZOOM zoom = ZOOM.NORMAL;
    ZOOM currentZoom = ZOOM.NORMAL;

    //プレイヤー追従用のプレイヤー取得
    private GameObject playerobj;
	//メインカメラ格納用
	private Camera maincamera;
	//カメラとプレイヤーの座標の初期
	private Vector3 offset;
	//初期FOV格納用
	private float fov;

    //カメラの情報受け取り用
    private CameraManager cameraObj;
	//カメラの全体のparameter格納用
	private CameraParameter cameraParamater;
	//座標角度格納用
	private Transform cameraParent;
	//距離格納用
	private Transform cameraChild;

    [Header("到着までの大体の時間")]
    [SerializeField] private float smoothTime = 0.1f;
    [Header("最高速度")]
    [SerializeField] private float maxSpeed = 10.0f;
    [Header("ズームイン倍率")]
    [SerializeField] private float zoomIn = 0.1f;
    [Header("ズームアウト倍率")]
    [SerializeField] private float zoomOut = 0.1f;

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        cameraObj = GameObject.Find("CameraParent").GetComponent<CameraManager>();
		cameraParamater = cameraObj.GetParameter();
        //プレイヤーを格納
        playerobj = GameObject.FindGameObjectWithTag("Player");
        //初期化としてメインカメラを格納
        maincamera = cameraObj.GetTransformObject();
		cameraParent = cameraObj.GetTransformObject(true);
		cameraChild = cameraObj.GetTransformObject(false);
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        offset = maincamera.transform.position - playerobj.transform.position;
		fov = maincamera.fieldOfView;
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        CameraMove();
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (Input.GetKeyUp(KeyCode.F1))
        {
            zoom = ZOOM.IN;
            ZoomInOut();
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            zoom = ZOOM.OUT;
            ZoomInOut();
        }
    }
    
    /// <summary>
    /// カメラの動き自体の処理
    /// </summary>
    private void CameraMove()
	{
		var targetpos = playerobj.transform.position + offset;
        var currentVelocity = new Vector3();
        maincamera.transform.position = Vector3.SmoothDamp(maincamera.transform.position,
														   targetpos,
														   ref currentVelocity,
                                                           smoothTime,
														   maxSpeed); 
    }

    //ズームインとズームアウトを決定動かす処理
	private void ZoomInOut()
	{
        switch(currentZoom)
		{
			case ZOOM.NORMAL:
                if(zoom == ZOOM.IN)
                {
                    fov *= zoomIn;
                    currentZoom = ZOOM.IN;
                    break;
                }
                if (zoom == ZOOM.OUT)
                {
                    fov *= zoomOut;
                    currentZoom = ZOOM.OUT;
                    break;
                }
                break;

            case ZOOM.IN:
                if (zoom == ZOOM.OUT)
                {
                    fov /= zoomIn;
                    currentZoom = ZOOM.NORMAL;
                    break;
                }
                break;

            case ZOOM.OUT:
                if (zoom == ZOOM.IN)
                {
                    fov /= zoomOut;
                    currentZoom = ZOOM.NORMAL;
                    break;
                }
                break;
        }

        maincamera.fieldOfView = fov;
    }
}
