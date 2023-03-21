//=============================================================================
// @File	: [GuestAnimation.cs]
// @Brief	: ゲスト用アニメーション制御スクリプト
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/21	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    private bool isWalk = false;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		
	}

	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
        isWalk = animator.GetBool("isWalk");
        WalkAnimation();
    }

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
        if (!animator || !audioSource) return;
        if (isWalk == animator.GetBool("isWalk")) return;

        isWalk = animator.GetBool("isWalk");

        WalkAnimation();
    }

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}

    private void WalkAnimation()
    {
        if (isWalk)
        {
            SoundManager.Play(audioSource, SoundManager.ESE.HUMAN_WALK_003);
        }
        else
        {
            audioSource.Stop();
        }
        
    }
}
