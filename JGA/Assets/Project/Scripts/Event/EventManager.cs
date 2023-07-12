//=============================================================================
// @File	: [EventManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/11	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EventManager : MonoBehaviour
{
    //---イベント
    [Header("タイマーの丸のそれぞれのイベント")]
    [SerializeField]
    private EventParam[] events = default;
    private string[] eventChar = new string[] { "エントランスに向かおう", "鐘を鳴らそう", "しろくまブースへ向かおう", "うまブースへ向かおう", "メガホンを使ってみよう", "もうすぐ、時間切れ" };
    [SerializeField]
    [Header("イベントのテキスト")]
    private GameObject eventText;
    private GameObject eventTextInstance;
    private Text eventTextChar;

    // Start is called before the first frame update
    void Start() {
        //----- イベントを静的クラスに保存 -----
        GameData.events = new EventParam[this.events.Length];
        for (int i = 0; i < events.Length; i++) {
            /*
             * 上の ”new EventParam[this.events.Length]"ではサイズを確保しているだけで初期化はされていないので
             * 下の ”new EventParam()”というように ”()” をつけコンストラクタを呼び出し初期化する
             */
            GameData.events[i] = new EventParam();
            GameData.events[i].eventState = this.events[i].eventState;
            GameData.events[i].percent = this.events[i].percent;
        }

    }

    // Update is called once per frame
    void Update() {
        // テキスト
        //switch (events[_TimerUI.CurrentTimerPoint() - 1].eventState) {
        //    case MySceneManager.eEvent.GUEST_ENTER:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GUEST_ENTER];
        //        break;
        //    case MySceneManager.eEvent.SOUND_BELL:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.SOUND_BELL];
        //        break;
        //    case MySceneManager.eEvent.GO_POLARBEAR:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GO_POLARBEAR];
        //        break;
        //    case MySceneManager.eEvent.GO_HOURSE:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.GO_HOURSE];

        //        break;
        //    case MySceneManager.eEvent.OBJ_MEGAPHON:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.OBJ_MEGAPHON];

        //        break;
        //    case MySceneManager.eEvent.TIMEOUT_ALERT:
        //        eventTextChar.text = eventChar[(int)MySceneManager.eEvent.TIMEOUT_ALERT];

        //        break;
        //    default:
        //        Debug.LogWarning("イベント番号が無効な値を受け取りました");
        //        break;
        //};

    }
}
