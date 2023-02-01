using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���ʂ̏�����������V�[���̌Ăяo�����s��
/// URL:https://noracle.jp/unity-initialize-scene/
/// </summary>
public class Initialize
{
    // ���������s���V�[���̖��O������
    private const string InitializeSceneName = "InitializeScene";

    // �����̃��t�@�����X:https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
    // [RuntimeInitializeOnLoadMethod]�Q�[�������[�h���ꂽ��ɌĂяo�����B
    // �����݂̃V�[����Awake()�̌�ɏ����p�V�[������������ď������V�[����Awake()������B
    // �@�ڂ�����summry��URL��
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void RuntimeInitializeApplication()
    {

        if (!SceneManager.GetSceneByName(InitializeSceneName).IsValid()){
            SceneManager.LoadScene(InitializeSceneName,LoadSceneMode.Additive);
        }
        else{
            Debug.LogError("�������p�V�[���̌Ăяo�����o���܂���ł����B");
        }

    }

}
