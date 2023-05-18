//=============================================================================
// @File	: [FreezeObject.cs]
// @Brief	: 
// @Author	: KAIKAWA KOYO
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/08	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FreezeObject : MonoBehaviour
{
    private Player player;
    [SerializeField] private float shaketime = 0.3f;    // 揺れる時間
    [SerializeField] private float shakepower = 0.2f;   // 揺れの強さ
    [SerializeField] private int   shakevibrato = 100;  // 揺れの数
    [SerializeField] private float shakerandom = 100;   // 揺れのランダム度合い
    private Tweener shakeTweener;
    private Vector3 InitPosition;
    private bool IsShake;
    private float delaytime;

    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
		player = GameObject.FindWithTag("Player").GetComponent<Player>();
        InitPosition = transform.position;
        delaytime = 0.2f;
    }

    /// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
    {
        if (IsShake)
        {
            Invoke(nameof(ResetPosition), shaketime);   // 揺れ終わったら場所を戻す
            IsShake = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // はたかれたら揺らす
            if (player.IsHitMotion)
            {
                Invoke(nameof(StartShake), delaytime);
            }
        }
    }

    // 揺れる処理
    private void StartShake()
    {
        if(shakeTweener != null)
        {
            shakeTweener.Kill();
            gameObject.transform.position = InitPosition;
        }
        shakeTweener = gameObject.transform.DOShakePosition(shaketime, shakepower, shakevibrato, shakerandom);
        IsShake = true;
    }

    private void ResetPosition()
    {
        gameObject.transform.position = InitPosition;
    }
}
