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
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(UIManager))]
public class UIManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }
}
