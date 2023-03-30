//=============================================================================
// @File	: [NamedArrayDrawer.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/03/30	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer {
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
        try {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));

        } catch {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}
