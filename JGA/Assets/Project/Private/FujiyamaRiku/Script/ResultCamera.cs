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
using UnityEngine;

public class ResultCamera : MonoBehaviour
{
	private GameObject mainCamera;
	private GameObject resultCamera;

    private GameObject guestNumUI;
    private GuestNumUI _GuestNumUI;

	private bool Crear;

	[SerializeField] private float RotateTime;
    [SerializeField] private float radius;
    private float NowTime;
    

    private Vector3 StartPos;
	private Vector3 EndPos;

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
        StartPos = resultCamera.transform.position;
        EndPos = resultCamera.transform.position;
        RotateTime = 5.0f;
        NowTime = 0.0f;
        Crear = false;
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
            Crear = true;
            ChangeCamera();
        }
		if (Crear)
		{
            CameraRotate();

        }

            //----- ゲームクリア -----
            if (guestNumUI)
		{
			if (_GuestNumUI.isClear())
			{
				if (!Crear)
				{
                    Crear = true;
					ChangeCamera();
				}
				CameraRotate();
            }

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
        

    }

}
