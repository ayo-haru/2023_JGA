//=============================================================================
// @File	: [CardBoard.cs]
// @Brief	: 段ボールの処理を記載
// @Author	: Yoshihara Asuka
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/30	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBoard : BaseObj
{
	[SerializeField] private Animator _animator;

	private bool isPlay = false;
    private bool isNPosition = false;
    private bool isBreak = false;

    [SerializeField] private Collider collision0;
    private GameObject collision1;
    private GameObject collision2;
    private GameObject collision3;
    
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	protected override void Start()
	{
		Init();
		_animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        collision1 = transform.GetChild(1).gameObject;
        collision2 = transform.GetChild(2).gameObject;
        collision3 = transform.GetChild(3).gameObject;
        collision1.SetActive(false);
        collision2.SetActive(false);
        collision3.SetActive(false);
        objType = ObjType.HIT_HOLD;
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		// ポーズ処理
		if (PauseManager.isPaused) { return; }

        PlaySoundChecker();
	}

	protected override void OnCollisionEnter(Collision collision)
	{

		// 段ボールが地面に当たった時の音を変更
		if(collision.gameObject.tag == "Ground"){
            if(!isPlaySound && isPlay)
            {
                PlayDrop(audioSource, SoundManager.ESE.CARDBOARDBOX_002);
            }

            // 段ボールの角度が正常か
            foreach (ContactPoint point in collision.contacts)
            {
                Vector3 relativePoint = transform.InverseTransformPoint(point.point);   // 物がぶつかった座標を取得

                if (relativePoint.y < -0.5) // 地面が下面と接しているか
                {
                    isNPosition = true;
                }
                else
                {
                    isNPosition = false;
                }
            }
            if(!isPlay)
                isPlay = true;
        }
		else{
			PlayHit();
		}

        // 上に物が乗ったら潰れる
        if (collision.gameObject.tag == "Interact")
        {
            foreach(ContactPoint point in collision.contacts)
            {
                Vector3 relativePoint = transform.InverseTransformPoint(point.point);   // 物がぶつかった座標を取得

                if(relativePoint.y > 0.5 && relativePoint.x < 1.0 && relativePoint.x > -1.0 && relativePoint.z < 1.0 && relativePoint.z > -1.0 && isNPosition && !isBreak)
                {
                    _animator.SetTrigger("Broken");
                    isBreak = true;
                    
                    // コライダー切替
                    collision0.enabled = false;
                    collision1.SetActive(true);
                    collision2.SetActive(true);
                    collision3.SetActive(true);

                    // 潰れたときの音を鳴らす
                    PlayDrop(audioSource, SoundManager.ESE.CARDBOARDBOX_002);

                    break;
                }
                if(isBreak)
                {
                    break;
                }
            }
        }
	}

    protected override void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // 殴られたら音を出す
            if (player.IsHit)
            {
                PlayHit();
            }
        }
    }
}
