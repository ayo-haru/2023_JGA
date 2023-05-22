//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/05/21	スクリプト作成
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;       
using System.IO;    


[Serializable]  // これをつけないとシリアライズできない
public struct SaveData {
    public int lastStageNum;
    public Vector3 lastPlayerPos;
    public int guestCnt;
    public float timer;
    public float bgmVolume;
    public float seVolume;
}
/*
    シリアライズとは
    実行中のアプリケーションにおけるオブジェクトの値（状態）を、
    バイナリ形式やXML形式などに変換して、保存することをいう。
    シリアライズすることによって、オブジェクトをファイルとして
    永続的に保存したりできる。
 */

public static class SaveSystem {
    public static SaveData sd;
    public static bool shouldSave = false;
    public static bool canSave = false;
    const string SAVE_FILE_PATH = "save.json";  // セーブデータの名前

    public static void SaveLastStageNum(int _lastStageNum) {
        sd.lastStageNum = _lastStageNum;
        save();
    }

    public static void SaveLastPlayerPos(Vector3 _lastPlayerPos) {
        sd.lastPlayerPos = _lastPlayerPos;
        save();
    }

    public static void SaveGuestCnt(int _guestCnt) {
        sd.guestCnt = _guestCnt;
        save();
    }

    public static void SaveTimer(float _timer) {
        sd.timer = _timer;
        save();
    }

    public static void SaveBGMVolume(float _bgmVolume){  // BGMをJsonに保存する
        sd.bgmVolume = _bgmVolume;
        save();
    }
    public static void SaveSEVolume(float _seVolume){  // SEをJsonに保存する
        sd.seVolume = _seVolume;
        save();
    }


    public static void save() {
        string json = JsonUtility.ToJson(sd);   //  Jsonにシリアアライズ
#if UNITY_EDITOR
        string path = Directory.GetCurrentDirectory();  // GetCurrentDirectory...アプリケーションの現在のディレクトリを取得
#else
        string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
        path += ("/" + SAVE_FILE_PATH); // 保存場所のパスを格納
        FileStream fs = new FileStream(path, FileMode.Create,FileAccess.ReadWrite);
        StreamWriter writer = new StreamWriter(fs);    //  上書き
        writer.WriteLine(json); // 一行ずつ書き込みして改行
        writer.Flush();         // バッファに残る値をすべて書き出す
        writer.Close();         // 書き込みの終了（fclose()みたいなやつ）
    }

    public static bool load() {
        try
        {
#if UNITY_EDITOR
            string path = Directory.GetCurrentDirectory();  // GetCurrentDirectory...アプリケーションの現在のディレクトリを取得
#else
            string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
            //FileInfo info = new FileInfo(path + "/" + SAVE_FILE_PATH);  // 保存場所からのロード
            path += ("/" + SAVE_FILE_PATH); // 保存場所のパスを格納
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            StreamReader reader = new StreamReader(fs);
            //StreamReader reader = new StreamReader(info.OpenRead());    // info.OpenRead()でファイルパスがとれるっぽい
            string json = reader.ReadToEnd();                           // ReadToEndは一括読込らしいReadLineで一行ずつ読込
            sd = JsonUtility.FromJson<SaveData>(json);                  // FromJson...Jsonを読み取りインスタンスのデータを上書きする

            return true;    // セーブデータ有り
        }
        catch (Exception e)  //  例外処理
        {
            sd = new SaveData();

            return false;   // セーブデータなし
            //bgmとseの初期値設定
            //sd.bgmVolume = SoundManager.bgmVolume;
            //sd.seVolume = SoundManager.seVolume;
        }
    }
}

/*
 * Using文（Directoryでつかわれてる）
 * 「インスタンスを使用後に破棄する」という場合に用いられる構文
 * Usingの後の()内でインスタンスを作成し、{}内でそのインスタンスを使った処理を記述していく
 * {}内の処理を実行した後、そこを抜けだす際に()で作ったインスタンスを自動的に破棄する。
 * {}内でエラーが発生しても、必ず()で作られたインスタンスが破棄される
 */
