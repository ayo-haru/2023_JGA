//=============================================================================
// @File	: [SystemManagerEditor.cs]
// @Brief	: システムマネージャーのエディタ
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/19	スクリプト作成
//=============================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CustomEditor(typeof(SystemManager))]
public class SystemManagerEditor : Editor {
    UIManagerEditor uiManagerEditor;
    SerializedObject uiManager;

    private void OnEnable() {
        uiManagerEditor = new UIManagerEditor();

        //uiManager = uiManagerEditor.GetSerializedObject();
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        {
            DrawDefaultInspector();

            //uiManagerEditor.DrawProperty();
        }


        // SetializedObjectのイテレータを取得
        //var iter = uiManager.GetIterator();
        // 最初の一個はスクリプトへの参照なのでスキップ
        // iter.NextVisible(true);
        // 残りのプロパティをすべて描画する
        // while (iter.NextVisible(false)) {
        //     描画しないプロパティはこんな感じで飛ばしておく
        //     if (iter.name == "_property1") {
        //         continue;
        //     }
        //     描画
        //     EditorGUILayout.PropertyField(iter, true);
        // }


        serializedObject.ApplyModifiedProperties();

    }
}
