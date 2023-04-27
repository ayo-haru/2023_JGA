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
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class ResultCamera : MonoBehaviour
{
	private GameObject mainCamera;
	private GameObject resultCamera;

    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

	private bool clear;

	[SerializeField] private float rotateTime;
    private float rotateFlame;
    

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
            ChangeCamera();
        }
		

            //----- ゲームクリア -----
        if (guestNumUI)
		{
			if (_GuestNumUI.isClear())
			{
				if (!clear)
				{
                    clear = true;
					ChangeCamera();
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
        Debug.Log(rotateFlame);
        if (rotateFlame >= rotateTime)
        {
            //clear = false;
            return;
        }
        rotateFlame += Time.deltaTime;

        var buf = resultCamera.transform.position;
        resultCamera.transform.RotateAround(Vector3.zero,
                                            Vector3.up,
                                            360 / rotateTime * Time.deltaTime);
        
    }

}
