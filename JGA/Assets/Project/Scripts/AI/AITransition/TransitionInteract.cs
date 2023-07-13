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
// 2023/04/3	(小楠)インタラクトタグを使ったオブジェクト検索に変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionInteract : AITransition
{
    private Transform playerTransform;
    private GuestData.Data data;
    private List<BaseObj> interactObjecs;
    public enum EReactionObject { ALL,CARBOARD,};
    [SerializeField, Header("反応するインタラクトオブジェクト")] EReactionObject reactionObject = EReactionObject.ALL;
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
        if (!playerTransform) playerTransform = GameObject.FindWithTag("Player").transform;
        if (data==null) data = GetComponent<AIManager>().GetGuestData();
        if (interactObjecs != null) return;

        interactObjecs = new List<BaseObj>();
        if (!GimickObjectManager.Instance) return;

        //ギミックマネージャから取得
        interactObjecs = GimickObjectManager.Instance.GetGimickObjectAll();

        switch (reactionObject)
        {
            case EReactionObject.ALL:
                break;
            case EReactionObject.CARBOARD:
                for(int i = 0; i < interactObjecs.Count; ++i)
                {
                    if (interactObjecs[i].TryGetComponent(out CardBoard cardBoard)) continue;
                    interactObjecs.RemoveAt(i);
                    --i;
                }
                break;
        }
    }

    public override bool IsTransition()
    {
        //エラーチェック
        if (!ErrorCheck()) return false;

        //プレイヤーが範囲内に居るか
        if (Vector3.Distance(transform.position, playerTransform.position) > data.reactionArea) return false;

        //客の音インタラクト反応位置
        Vector3 guestPos = transform.position + transform.forward * data.soundAreaOffset;
        //範囲内にあるインタラクトオブジェクトのフラグが立っているか
        for (int i = 0; i < interactObjecs.Count; ++i)
        {
            if (!interactObjecs[i].GetisPlaySound()) continue;
            //インタラクトオブジェクト位置
            //インタラクトされた時プレイヤーの子オブジェクトにされるため、ローカル座標からワールド座標に変換
            Vector3 interactPos = (interactObjecs[i].transform.parent) ? interactObjecs[i].transform.parent.TransformPoint(interactObjecs[i].transform.localPosition) : interactObjecs[i].transform.position;
            if (Vector3.Distance(guestPos, interactPos) <= data.reactionArea) return true;
        }
        return false;
    }

    public override bool ErrorCheck()
    {
        if (!playerTransform)Debug.LogError("プレイヤーのトランスフォームが取得されていません");
        if (data==null)Debug.LogError("ゲスト用データが取得されていません");
        if ((interactObjecs == null) ? true : interactObjecs.Count <= 0)Debug.LogWarning("インタラクトオブジェクトがありません");

        return playerTransform && (data!=null) && ((interactObjecs == null) ? false : interactObjecs.Count > 0);
    }
}
