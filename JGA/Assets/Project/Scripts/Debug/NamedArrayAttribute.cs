//=============================================================================
// @File	: [NamedArrayAttribute.cs]
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
using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute {
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}
