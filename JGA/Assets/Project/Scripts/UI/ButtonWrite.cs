//=============================================================================
// @File	: [ButtonWrite.cs]
// @Brief	: UIWriteShaderの値変更用のスクリプト
// @Author	: 小楠裕子
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/19	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWrite : MonoBehaviour
{
    [SerializeField] Material mat;
    private Material myMaterial;
    [SerializeField] private float threshold = 0.0f;
    [SerializeField] private Color pink = new Color(0.8980392f, 0.6745098f, 0.8039216f, 1f);
#if false
    /// <summary>
    /// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
    /// </summary>
    void Awake()
	{
		
	}
#endif
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        //マテリアル、テクスチャを設定
        if (!mat) return;
        if (!TryGetComponent(out Image image)) return;
        myMaterial = new Material(mat);
        image.material = myMaterial;
        myMaterial.SetTexture("_MainTex", image.mainTexture);
	}
#if false
    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
		
	}
#endif
	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        if (!myMaterial) return;
        myMaterial.SetFloat("_Threshold", threshold);
    }

    public void WhiteToPink()
    {
        if (!myMaterial) return;
        myMaterial.SetColor("_ChangeColor", pink);
        myMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 1));
    }

    public void ClearToWhite()
    {
        if (!myMaterial) return;
        myMaterial.SetColor("_ChangeColor", new Color(1, 1, 1, 1));
        myMaterial.SetColor("_BaseColor", new Color(1, 1, 1, 0));
    }
}
