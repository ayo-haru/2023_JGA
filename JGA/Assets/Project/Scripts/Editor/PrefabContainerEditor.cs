//=============================================================================
// @File	: [PrefabContainerEditor.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/26	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using static UnityEngine.Rendering.DebugUI;
using System;

//[UnityEditor.CustomEditor(typeof(PrefabContainer))]
public class PrefabContainerEditor : Editor {
    ////SerializedProperty prefabList;

    //PrefabContainer.Container[] prefabList;


    //private void OnEnable() {
    //    prefabList = serializedObject.FindProperty("list");
    //}

    //public override void OnInspectorGUI() {
    //    serializedObject.Update();

    //    //リストの名前を記載
    //    EditorGUILayout.LabelField("プレハブ");

    //    //表示
    //    for (int i = 0; i < prefabList.arraySize; ++i) {
    //        EditorGUILayout.PropertyField(prefabList.GetArrayElementAtIndex(i), new GUIContent(newstring(i)));
    //    }

    //}
}
