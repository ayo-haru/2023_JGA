//=============================================================================
// @File	: [Option.cs]
// @Brief	: オプション動かす用（タイトル）
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/06/21	スクリプト作成
//=============================================================================
using UnityEngine;


public class Option : MonoBehaviour
{
	[SerializeField, Header("パネル移動速度")]private float PanelMoveValue = 100;

	[Header("パネル")]
	[SerializeField]
	private GameObject optionPanel;
	[SerializeField]
	private GameObject keyConfigPanel;

    //有効パネル
	private GameObject ActivePanel;

	[SerializeField]
	private AudioSource audioSource;
	private RectTransform rect;

	/// <summary>
	/// Prefabのインスタンス化直後に呼び出される：ゲームオブジェクトの参照を取得など
	/// </summary>
	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
        rect = GetComponent<RectTransform>();

		//最初は非表示
		if (gameObject.activeSelf)gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		ActivePanel = optionPanel;
	}

	private void FixedUpdate()
	{
		if (ActivePanel == optionPanel && rect.localPosition != Vector3.zero)
        {
            rect.localPosition = Vector3.MoveTowards(rect.localPosition, Vector3.zero, PanelMoveValue);
        }
		if (ActivePanel == keyConfigPanel && rect.localPosition != new Vector3(-1920.0f, 0, 0))
        {
            rect.localPosition = Vector3.MoveTowards(rect.localPosition, new Vector3(-1920.0f, 0, 0), PanelMoveValue);
        }
			
	}
#if false
    private void Update()
	{

	}
#endif
	public void ChangePanel(GameObject panelObj)
	{
		ChangePanel(panelObj.name);
	}
	public void ChangePanel(string panelName)
	{
		if (panelName.Equals(optionPanel.name))
        {
            ActivePanel = optionPanel;
        }
		if (panelName.Equals(keyConfigPanel.name))
        {
            ActivePanel = keyConfigPanel;
        }
	}
#region SE鳴らす関数
	public void SoundSelectSE()
	{
		if (!audioSource) return;
		SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
	}

	public void SoundDecisionSE()
	{
		if (!audioSource) return;
		SoundManager.Play(audioSource, SoundManager.ESE.DECISION_001);
	}
	public void SoundSlideSE()
	{
		if (!audioSource) return;
		SoundManager.Play(audioSource, SoundManager.ESE.SELECT_001);
	}
#endregion
}
