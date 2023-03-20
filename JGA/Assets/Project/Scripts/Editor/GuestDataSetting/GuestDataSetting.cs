////=============================================================================
//// @File	: [GuestDataSetting.cs]
//// @Brief	: 
//// @Author	: Ichida Mai
//// @Editer	: 
//// @Detail	: 
//// 
//// [Date]
//// 2023/03/16	スクリプト作成
////=============================================================================
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;

//public class GuestDataSetting : EditorWindow {
//    string displayName = "GuestData";

//    //----- GuestDataのパラメーターたち -----
//    GuestData guestData;
//    SerializedObject serializeObj;
//    SerializedProperty speed;
//    SerializedProperty rayLength;
//    SerializedProperty reactionArea;
//    SerializedProperty distance;
//    SerializedProperty firstCoolDownTime;
//    SerializedProperty secondCoolDownTime;
//    SerializedProperty waitTime;
//    SerializedProperty cageDistance;



//    [MenuItem("MyWindow/GuestDatsSettingWindow")]
//    static void Open() {
//        var window = GetWindow<GuestDataSetting>();
//        window.titleContent = new GUIContent("客データ設定");
//    }

//    private void OnEnable() {

//        guestData = ScriptableObject.CreateInstance<GuestData>();
//        serializeObj = new SerializedObject(guestData);
//        speed = serializeObj.FindProperty("speed");
//        rayLength = serializeObj.FindProperty("rayLength");
//        reactionArea = serializeObj.FindProperty("reactionArea");
//        distance = serializeObj.FindProperty("distance");
//        firstCoolDownTime = serializeObj.FindProperty("firstCoolDownTime");
//        secondCoolDownTime = serializeObj.FindProperty("secondCoolDownTime");
//        waitTime = serializeObj.FindProperty("waitTime");
//        cageDistance = serializeObj.FindProperty("cageDistance");
//    }

//    private void OnGUI() {
//        displayName = EditorGUILayout.TextField("客データ名", displayName);       
        
//        serializeObj.Update();

//        EditorGUILayout.PropertyField(speed);
//        EditorGUILayout.PropertyField(rayLength);
//        EditorGUILayout.PropertyField(reactionArea);
//        EditorGUILayout.PropertyField(distance);
//        EditorGUILayout.PropertyField(firstCoolDownTime);
//        EditorGUILayout.PropertyField(secondCoolDownTime);
//        EditorGUILayout.PropertyField(waitTime);
//        EditorGUILayout.PropertyField(cageDistance);

//        serializeObj.ApplyModifiedProperties();

//        if (GUILayout.Button("ボタン")) {
//            //Debug.Log("ボタンが押された");
            
//            string fileName = $"{displayName}.asset";
//            string path = "Assets/Project/Scripts/AI/GuestData/Datas";
//            if(!Directory.Exists(path)) {
//                Directory.CreateDirectory(path);
//            }
//            AssetDatabase.CreateAsset(guestData, Path.Combine(path,fileName));
//        }
//    }


//}
