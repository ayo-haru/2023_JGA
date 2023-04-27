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
    
	/// <summary>
	/// 最初のフレーム更新の前に呼び出される
	/// </summary>
	void Start()
	{
		Init();
		_animator = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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
			PlayDrop(audioSource, SoundManager.ESE.CARDBOARDBOX_002);

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

                if(relativePoint.y > 0.5 && relativePoint.x < 1.0 && relativePoint.x > -1.0 && relativePoint.z < 1.0 && relativePoint.z > -1.0 && isNPosition)
                {
                    _animator.SetTrigger("Broken");
                }
            }
        }

	}

    protected override void OnTriggerEnter(Collider other)
    {
		//if (other.gameObject.tag == "Player"){
		//		_animator.SetTrigger("Broken");

  //      }
    }

}
