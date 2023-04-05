//=============================================================================
// @File	: [GuestAnimation.cs]
// @Brief	: ゲスト用アニメーション制御スクリプト
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/21	スクリプト作成
// 2023/03/22	ポーズの処理追加
// 2023/03/30	足音削除
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GuestAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;


	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
        //ポーズ時の動作を登録
        PauseManager.OnPaused.Subscribe(x => { Pause(); }).AddTo(gameObject);
        PauseManager.OnResumed.Subscribe(x => { Resumed(); }).AddTo(gameObject);
    }
#if false
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{


    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (PauseManager.isPaused) return;
        if (!animator || !audioSource) return;





    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
#endif
    private void Pause()
    {
        animator.speed = 0.0f;
    }

    private void Resumed()
    {
        animator.speed = 1.0f;
    }
}
