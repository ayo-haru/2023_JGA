//=============================================================================
// @File	: [TransitionInteract.cs]
// @Brief	: 遷移条件　ペンギンが近くにいてかつインタラクトしたら
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/03/07	スクリプト作成
// 2023/03/13	(小楠)インタラクトフラグ取得
// 2023/03/13	(伊地田)自動生成に対応
// 2023/03/31	(小楠)BaseObjクラスを使った処理に変更
// 2023/04/01	(小楠)エラー直した
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteract : AITransition
{
    private Transform playerTransform;
    private GuestData.Data data;
    private BaseObj[] interactObjecs;
#if false
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
		
	}

	/// <summary>
	/// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
	/// </summary>
	void FixedUpdate()
	{
		
	}

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
		
	}
#endif
    public override void InitTransition()
    {
        //コンポーネント、オブジェクトの取得
        if (!playerTransform) playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        //if (interactObjecs == null) interactObjecs = GameObject.FindGameObjectsWithTag("Interact");
        if (interactObjecs == null)
        {
            GameObject interactObject = GameObject.Find("InteractObject");
            if (interactObject)
            {
                interactObjecs = interactObject.GetComponentsInChildren<BaseObj>();
            }
            else
            {
                #region プロトタイプシーン用のインタラクトオブジェクト取得処理
                List<BaseObj> baseObj = new List<BaseObj>();
                foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
                {
                    BaseObj baseObject = obj.GetComponent<BaseObj>();
                    if (baseObject) baseObj.Add(baseObject);
                }
                interactObjecs = new BaseObj[baseObj.Count];
                for(int i = 0; i < baseObj.Count; ++i)
                {
                    interactObjecs[i] = baseObj[i];
                }
                #endregion
            }
        }
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        //プレイヤーが範囲内に居るか
        if (Vector3.Distance(transform.position, playerTransform.position) > data.reactionArea) return false;

        //範囲内にあるインタラクトオブジェクトのフラグが立っているか
        for(int i = 0; i < interactObjecs.Length; ++i)
        {
            if (Vector3.Distance(transform.position, interactObjecs[i].transform.position) > data.reactionArea) continue;
            if (interactObjecs[i].GetisPlaySound()) return true;
        }
        return false;
    }

    public override bool ErrorCheck()
    {
        if (!playerTransform)Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if ((interactObjecs == null) ? true : interactObjecs.Length <= 0)Debug.LogWarning("インタラクトオブジェクトがありません");

        return playerTransform && (data!=null) && ((interactObjecs == null) ? false : interactObjecs.Length > 0);
    }
}
