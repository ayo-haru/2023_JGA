//=============================================================================
// @File	: [StageSceneManager.cs]
// @Brief	: 
// @Author	: Ichida Mai
// @Editer	: Ogusu Yuuko
// @Detail	: 
// 
// [Date]
// 2023/05/21	�X�N���v�g�쐬
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;       
using System.IO;    


[Serializable]  // ��������Ȃ��ƃV���A���C�Y�ł��Ȃ�
public struct SaveData {
    public int lastStageNum;
    public Vector3 lastPlayerPos;
    public int guestCnt;
    public float timer;
    public float bgmVolume;
    public float seVolume;
}
/*
    �V���A���C�Y�Ƃ�
    ���s���̃A�v���P�[�V�����ɂ�����I�u�W�F�N�g�̒l�i��ԁj���A
    �o�C�i���`����XML�`���Ȃǂɕϊ����āA�ۑ����邱�Ƃ������B
    �V���A���C�Y���邱�Ƃɂ���āA�I�u�W�F�N�g���t�@�C���Ƃ���
    �i���I�ɕۑ�������ł���B
 */

public static class SaveSystem {
    public static SaveData sd;
    public static bool shouldSave = false;
    public static bool canSave = false;
    const string SAVE_FILE_PATH = "save.json";  // �Z�[�u�f�[�^�̖��O

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

    public static void SaveBGMVolume(float _bgmVolume){  // BGM��Json�ɕۑ�����
        sd.bgmVolume = _bgmVolume;
        save();
    }
    public static void SaveSEVolume(float _seVolume){  // SE��Json�ɕۑ�����
        sd.seVolume = _seVolume;
        save();
    }


    public static void save() {
        string json = JsonUtility.ToJson(sd);   //  Json�ɃV���A�A���C�Y
#if UNITY_EDITOR
        string path = Directory.GetCurrentDirectory();  // GetCurrentDirectory...�A�v���P�[�V�����̌��݂̃f�B���N�g�����擾
#else
        string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
        path += ("/" + SAVE_FILE_PATH); // �ۑ��ꏊ�̃p�X���i�[
        FileStream fs = new FileStream(path, FileMode.Create,FileAccess.ReadWrite);
        StreamWriter writer = new StreamWriter(fs);    //  �㏑��
        writer.WriteLine(json); // ��s���������݂��ĉ��s
        writer.Flush();         // �o�b�t�@�Ɏc��l�����ׂď����o��
        writer.Close();         // �������݂̏I���ifclose()�݂����Ȃ�j
    }

    public static bool load() {
        try
        {
#if UNITY_EDITOR
            string path = Directory.GetCurrentDirectory();  // GetCurrentDirectory...�A�v���P�[�V�����̌��݂̃f�B���N�g�����擾
#else
            string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
            //FileInfo info = new FileInfo(path + "/" + SAVE_FILE_PATH);  // �ۑ��ꏊ����̃��[�h
            path += ("/" + SAVE_FILE_PATH); // �ۑ��ꏊ�̃p�X���i�[
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            StreamReader reader = new StreamReader(fs);
            //StreamReader reader = new StreamReader(info.OpenRead());    // info.OpenRead()�Ńt�@�C���p�X���Ƃ����ۂ�
            string json = reader.ReadToEnd();                           // ReadToEnd�͈ꊇ�Ǎ��炵��ReadLine�ň�s���Ǎ�
            sd = JsonUtility.FromJson<SaveData>(json);                  // FromJson...Json��ǂݎ��C���X�^���X�̃f�[�^���㏑������

            return true;    // �Z�[�u�f�[�^�L��
        }
        catch (Exception e)  //  ��O����
        {
            sd = new SaveData();

            return false;   // �Z�[�u�f�[�^�Ȃ�
            //bgm��se�̏����l�ݒ�
            //sd.bgmVolume = SoundManager.bgmVolume;
            //sd.seVolume = SoundManager.seVolume;
        }
    }
}

/*
 * Using���iDirectory�ł����Ă�j
 * �u�C���X�^���X���g�p��ɔj������v�Ƃ����ꍇ�ɗp������\��
 * Using�̌��()���ŃC���X�^���X���쐬���A{}���ł��̃C���X�^���X���g�����������L�q���Ă���
 * {}���̏��������s������A�����𔲂������ۂ�()�ō�����C���X�^���X�������I�ɔj������B
 * {}���ŃG���[���������Ă��A�K��()�ō��ꂽ�C���X�^���X���j�������
 */
