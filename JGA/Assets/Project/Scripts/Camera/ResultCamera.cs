//=============================================================================
// @File	: [ResultCamera.cs]
// @Brief	: カメラの処理
// @Author	: Fujiyama Riku
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/04/22	スクリプト作成
// 2023/04/22	カメラの切り替え実装
// 2023/04/24	クリアのカメラを実装しようとしたがちょっと難しいかった。もう少し待たれたし
// 2023/04/26   回転を実装&コメント書いた
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ResultCamera : MonoBehaviour
{
	private GameObject mainCamera;                  //メインカメラ格納用
	private GameObject resultCamera;                //リザルトカメラ格納用

    //クリア判定取得用変数
    private GameObject guestNumUI;                 
    private GuestNumUI _GuestNumUI;

	private bool clear;                             //クリア時一回のみの処理をするときに使う

    public bool rotateFlg;

    [Header("回転しきるまでの時間")]
	[SerializeField] private float rotateTime;      //回転している時間を指定
    private float rotateFlame;                      //回転時間を計算するため用
    

    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
        

        mainCamera = GameObject.Find("CameraParent");
        if (!mainCamera)
        {
            Debug.LogWarning("mainCameraがシーン上にありません");
        }
        resultCamera = GameObject.Find("ResultCamera");
        if (!resultCamera)
        {
            Debug.LogWarning("resultCameraがシーン上にありません");
        }
        //初期値のままだった時に最低限の数値を入れる
        if (rotateTime == 0)
        {
            rotateTime = 10.0f;
        }
        clear = false;
        rotateFlame = 0;
    }

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        mainCamera.SetActive(true);
        resultCamera.SetActive(false);

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
		if(Input.GetKeyDown(KeyCode.K)) 
		{
            clear = true;
           // ChangeCamera();
        }
		

            //----- ゲームクリア -----
        if (guestNumUI)
		{
			if (_GuestNumUI.isClear())
			{
				if (!clear)
				{
                    clear = true;
					//ChangeCamera();
				}
            }

		}
        if (clear)
        {
            CameraRotate();
        }
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

	private void ChangeCamera()
	{
		mainCamera.SetActive(!mainCamera.activeSelf);
        resultCamera.SetActive(!resultCamera.activeSelf);

	}
	private void CameraRotate()
	{
        //フレーム数(時間を計算して一周したかどうかをたしかめる)
        if (rotateFlame >= rotateTime)
        {
            rotateFlg = true;
            return;
        }
        rotateFlame += Time.deltaTime;

        //カメラを中心に向けて360から〇秒で回転を終わらせる処理
        mainCamera.transform.RotateAround(Vector3.zero,
                                            Vector3.up,
                                            360 / rotateTime * Time.deltaTime);

    }

}
