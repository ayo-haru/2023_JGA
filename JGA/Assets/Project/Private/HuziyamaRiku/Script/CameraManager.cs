//=============================================================================
// @File	: [CameraManager.cs]
// @Brief	: 
// @Author	: YOUR_NAME
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/02/28	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//エディタ拡張的な使い方をしていくカメラマネージャー
//基本的には情報をそれぞれのカメラに渡していく

/// <summary> カメラのパラメータ </summary>
[System.Serializable]
public class CameraParameter
{
    public GameObject playerObj;
    public Vector3 position;
    public Vector3 angles = new Vector3(10f, 0f, 0f);
    public float distance = 10f;
    public float fieldOfView = 60f;
    public Vector3 offsetPosition = new Vector3(0f, 1f, 0f);
    public Vector3 offsetAngles;
}

[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform parent;

    [SerializeField]
    private Transform child;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private CameraParameter parameter;


	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void LateUpdate()
	{
        if (!Application.isPlaying)
        {
            if (parent == null || child == null || camera == null)
            {
                return;
            }

            // パラメータを各種オブジェクトに反映
            parent.position = parameter.position;
            parent.eulerAngles = parameter.angles;

            var childPos = child.localPosition;
            childPos.z = -parameter.distance;
            child.localPosition = childPos;

            camera.fieldOfView = parameter.fieldOfView;
            camera.transform.localPosition = parameter.offsetPosition;
            camera.transform.localEulerAngles = parameter.offsetAngles;
        }
    }
    
    /// <summary>
    /// 必要なな座標を手に入れる
    /// </summary>
    /// <param name="GetMat">true：角度、座標, false：距離</param>
    /// <returns></returns>
    public Transform GetTransformObject(bool GetMat)
    {
        if (GetMat)
        {
            return parent;
        }
        else
        {
            return child;
        }
    }
    /// <summary>
    /// カメラのオブジェクトを手に入れる
    /// </summary>
    /// <returns></returns>
    public Camera GetTransformObject()
    {
        return camera;
    }
    /// <summary>
    /// parameterそのものを手に入れる
    /// </summary>
    /// <returns></returns>
    public CameraParameter GetParameter()
    {
        return parameter;
    }

}
