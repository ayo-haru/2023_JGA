//=============================================================================
// @File	: [GuestData.cs]
// @Brief	: 客のデータ
// @Author	: Ogusu Yuuko
// @Editer	: Ogusu Yuuko,Ichida Mai
// @Detail	: 
// 
// [Date]
// 2023/03/02	スクリプト作成
// 2023/03/11	(小楠)ケージとの距離を追加
// 2023/03/25	(伊地田)複数設定用に変更
// 2023/03/27	(小楠)ペンギンに追従する時とブース内の速さを追加
// 2023/03/30	(小楠)ペンギンブースのトランフォームをリストにした
// 2023/04/10	(小楠)視野角を追加
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/CreateGestData")]
public class GuestData : ScriptableObject
{
    // 表示するデータ
    [System.Serializable]
    public class Data {
        [Header("表示名")]
        public string name;
        [Header("客の巡回ルート\n(初期位置にもなるので１個は入れてほしい)")]
        public MySceneManager.eRoot[] roots;
        [HideInInspector]   // 巡回ルートのトランスフォーム
        public List<Transform> rootTransforms;
        [HideInInspector]   // ペンギンブースの位置
        public List<Transform> penguinTF;
        [HideInInspector]   // エントランスの位置
        public Transform entranceTF;
        [HideInInspector]   // ランダム生成されたかどうか
        public bool isRandom;
        [Header("移動速度")]
        [Range(1, 3)] public float speed = 1;
        [Header("ペンギン追従速度")]
        [Range(0.5f, 1.0f)] public float followSpeed = 0.5f;
        [Header("ブース内移動速度")]
        [Range(0.1f, 1.0f)] public float inBoothSpeed = 0.5f;
        [Header("視線の長さ")]
        [Min(1)] public float rayLength = 10.0f;
        [Header("視野角")]
        [Range(30.0f, 180.0f)] public float viewAngle = 60.0f;
        [Header("反応する範囲")]
        [Range(15, 20), Tooltip("ペンギンに反応してくれる範囲(注目度がMAXの時)")] public float reactionArea = 15;
        [Header("ペンギンエリアに到着する距離")]
        [Range(1, 50)] public float arrivalPenguinArea = 30.0f;
        [Header("ペンギンとの距離")]
        [Range(2, 4), Tooltip("ペンギンとの距離")] public float distance = 2;
        [Header("感情の変化時間(!!! -> !!)")]
        [Range(1, 5)] public float firstCoolDownTime = 3.0f;
        [Header("感情の変化時間(!! -> !)")]
        [Range(1, 5)] public float secondCoolDownTime = 5.0f;
        [Header("待機時間")]
        [Min(0)] public float waitTime = 0;
        [Header("ケージとの距離（最大値）")]
        [Min(1)] public float cageDistance = 10.0f;
    }

    // 表示
    [SerializeField]
    public Data[] dataList = {
        new Data {
            name = "OGuest",
            isRandom = false,
            speed = 1,
            followSpeed = 0.5f,
            inBoothSpeed = 0.5f,
            rayLength = 10.0f,
            viewAngle = 60.0f,
            reactionArea = 15, 
            distance = 2,
            firstCoolDownTime = 3.0f,
            secondCoolDownTime = 5.0f, 
            waitTime = 0,
            cageDistance = 10.0f
        }
    };
}
