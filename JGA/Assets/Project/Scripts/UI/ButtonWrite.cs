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
    private Animator animator;
    [SerializeField,Range(0.5f,2.0f)] float writeSpeed = 1.0f;

    [SerializeField] private bool play = true;

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
        if (TryGetComponent(out Image image))
        {
            myMaterial = new Material(mat);
            image.material = myMaterial;
            myMaterial.SetTexture("_MainTex", image.mainTexture);
        }
        else if(TryGetComponent(out SpriteRenderer sr))
        {
            myMaterial = new Material(mat);
            sr.material = myMaterial;
            myMaterial.SetTexture("_MainTex", sr.sprite.texture);
        }
        //アニメーション設定
        if(TryGetComponent(out animator))
        {
            animator.SetFloat("speed", play ? writeSpeed : 0.0f);
        }
	}

    private void OnDestroy()
    {
        if (myMaterial)
        {
            Destroy(myMaterial);
            myMaterial = null;
        }
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
        animator.SetFloat("speed", play ? writeSpeed : 0.0f);
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

    public void StartWriteAnimation()
    {
        if (!animator) return;
        animator.SetFloat("speed",writeSpeed);
    }
}
