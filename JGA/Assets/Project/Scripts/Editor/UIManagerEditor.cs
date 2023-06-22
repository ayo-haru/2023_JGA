////=============================================================================
//// @File	: [UIManagerEditor.cs]
//// @Brief	: 
//// @Author	: Ichida Mai
//// @Editer	: 
//// @Detail	: 
//// 
//// [Date]
//// 2023/06/19	スクリプト作成
////=============================================================================
//using System.Collections;
//using System.Collections.Generic;
using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
//    private static SerializedObject _instance;

//    private void OnEnable() {
//        _instance = serializedObject;
//    }

//    public override void OnInspectorGUI() {
//        //DrawDefaultInspector();
//        // SetializedObjectのイテレータを取得
//        var iter = serializedObject.GetIterator();
//        for (bool enterChildren = true; iter.NextVisible(enterChildren); enterChildren = false) {
//            using (new EditorGUI.DisabledScope("m_Script" == iter.propertyPath))
//                EditorGUILayout.PropertyField(iter, true);
//        }

//    }


//    public void DrawProperty() {
//        // SetializedObjectのイテレータを取得
//        var iter = serializedObject.GetIterator();
//        // 最初の一個はスクリプトへの参照なのでスキップ
//        //iter.NextVisible(true);
//        // 残りのプロパティをすべて描画する
//        //while (iter.NextVisible(false)) {
//        //    // 描画しないプロパティはこんな感じで飛ばしておく
//        //    if (iter.name == "_property1") {
//        //        continue;
//        //    }
//        //    // 描画
//        //    EditorGUILayout.PropertyField(iter, true);
//        //}
//        for (bool enterChildren = true; iter.NextVisible(enterChildren); enterChildren = false) {
//            using (new EditorGUI.DisabledScope("m_Script" == iter.propertyPath))
//                EditorGUILayout.PropertyField(iter, true);
//        }
//    }

//    public SerializedObject GetSerializedObject() {
//        return _instance;
//    }

//    //public Editor GetUIManager() {
//    //    return this;
//    //}
}
