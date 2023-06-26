//=============================================================================
// @File	: [TestWindow.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/23	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestWindow : EditorWindow
{
    [MenuItem("Tools/TestWindow")]
    static void Open() {
        var window = EditorWindow.GetWindow<TestWindow>("てすと");
        window.ShowUtility();
    }

    private void OnGUI() {
        if (GUILayout.Button("てすと")) {
            UIManagerEditor uIManager = ScriptableObject.CreateInstance<UIManagerEditor>();
        }
    }
}
