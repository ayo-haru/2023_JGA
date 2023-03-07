//=============================================================================
// @File	: [CameraManager.cs]
// @Brief	: カメラ作成。
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/06	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MyCameraManager : MonoBehaviour
{
	/// <summary>
	/// カメラのパラメータ
	/// 初期値を代入済み
	/// </summary>
	[System.Serializable]
	public class Parameter
	{
		public Transform trackTarget;									// 被写体の座標
		public Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);		// 座標(Parent)
		public Vector3 angles = new Vector3(10.0f, 0.0f, 0.0f);         // 角度(parent)
		public float distance = 7f;										// 距離(注視点からの)Childの座標を調整
		public float FOV = 45f;											// 視野角
		public Vector3 offSetPosition = new Vector3(0.0f, 1.0f, 0.0f);	// 座標(MainCamera)
		public Vector3 offSetAngles = new Vector3(0.0f, 0.0f, 0.0f);	// 角度(MainCamera)

	}


	//---- Variables ----
	[SerializeField]
	private Transform parent;

    [SerializeField]
	private Transform child;

    [SerializeField]
	private Camera myCamera;

    [SerializeField]
	private Parameter parameter;

    private void Start()
    {
        //parameter.trackTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // 追従するオブジェクトの更新処理が終わった後にカメラの更新を行うため、LateUpdate()を使用
    private void LateUpdate()
    {
		// カメラ情報が登録されていない、また被写体を登録していないなら処理終了
        if(parent == null || child == null || myCamera == null){ 
			return;
		}

		// 被写体が指定されているなら、positionを被写体の座標に上書き
		if(parameter.trackTarget != null){
			parameter.position = parameter.trackTarget.position;
			//LateMove();
        }

		// カメラのパラメータを各種カメラオブジェクトに反映する
		SetCameraParam();
    }

	private void SetCameraParam()
    {
		// Parent(=カメラのワールド座標を調整)
		parent.position = parameter.position;
		parent.eulerAngles = parameter.angles;

		// Child(=被写体との距離を調整)
		var childPos = parameter.position;  // childオブジェクトのローカル座標を使用するため変数に格納
		childPos.z = -parameter.distance;
		child.localPosition = childPos;

		// MainCamera(=カメラのローカル座標を調整)
		myCamera.fieldOfView = parameter.FOV;
		myCamera.transform.localPosition = parameter.offSetPosition;
		myCamera.transform.localEulerAngles = parameter.offSetAngles;
	}

    private void LateMove()
	{ 
		parameter.position = Vector3.Lerp(
			a:parameter.position,
			b:parameter.trackTarget.position,
			t:Time.deltaTime * 2f);
    }
}
