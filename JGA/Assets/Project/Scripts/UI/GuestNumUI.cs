//=============================================================================
// @File	: [GuestNumUI.cs]
// @Brief	: ゲスト人数表示用UI
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/19	スクリプト作成
// 2023/04/17	アニメーション追加
// 2023/05/27	カウントした時の色変更
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuestNumUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guestNum;
    //目標人数
    [SerializeField,Range(1,99)] private int clearNum = 10;
    //アニメーション時間
    [SerializeField,Range(0,1)] private float animTime = 0.5f;
    private float fTimer = 0.0f;
    //拡大率
    [SerializeField, Range(1, 2)] private float scaleValue = 1.5f;
    
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
        guestNum.text = string.Format("{0:0} / {1:0}", MySceneManager.GameData.guestCnt, clearNum);
    }
    /// <summary>
    /// 一定時間ごとに呼び出されるメソッド（端末に依存せずに再現性がある）：rigidbodyなどの物理演算
    /// </summary>
    void FixedUpdate()
	{
        if (fTimer <= 0.0f) return;

        fTimer -= Time.deltaTime;
        if(fTimer <= 0.0f)
        {
            guestNum.text = string.Format("{0:0} / {1:0}", MySceneManager.GameData.guestCnt, clearNum);
        }
	}

#if UNITY_EDITOR
    /// <summary>
    /// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
    /// </summary>
    void Update() {
        //デバッグ用f1でクリア画面になる
        if (Input.GetKeyUp(KeyCode.F1)) {
            MySceneManager.GameData.guestCnt = clearNum;
        }
    }
#endif

    public void Add()
    {
        if (MySceneManager.GameData.guestCnt >= 99) return;
        ++MySceneManager.GameData.guestCnt;
        fTimer = animTime;
        guestNum.text = string.Format("<color=blue><size={0:0}>{1:0}</size></color> / {2:0}", guestNum.fontSize * scaleValue, MySceneManager.GameData.guestCnt, clearNum);

        SaveManager.SaveAll();  // 客加算時にオートセーブ
    }

    public bool isClear() {
        return MySceneManager.GameData.guestCnt >= clearNum;
    }

}
