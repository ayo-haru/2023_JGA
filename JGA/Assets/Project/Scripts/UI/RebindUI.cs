//=============================================================================
// @File	: [RebindUI.cs]
// @Brief	: 
// @Author	: Sakai Ryotaro
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/05/15	スクリプト作成
//=============================================================================
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
	// リバインド対象のAction
	[SerializeField] private InputActionReference _actionRef;

	// リバインド対象のScheme
	public enum Mode
	{
		Keyboard,
		GamePad,
	};

	[SerializeField] private Mode mode = Mode.Keyboard;
	string _scheme = string.Empty;

	// 現在のBindingのパスを表示するテキスト
	[SerializeField] private Button _pathText;

	[SerializeField] private Button _resetButton;

	// リバインド中のマスク用オブジェクト
	[SerializeField] private GameObject _mask;

	private InputAction _action;
	private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

	// 初期化
	private void Awake()
	{
		if (_actionRef == null) return;

		// InputActionインスタンスを保持しておく
		_action = _actionRef.action;

		switch (mode)
		{
			case Mode.Keyboard:
				_scheme = "Keyboard";
				break;
			case Mode.GamePad:
				_scheme = "Gamepad";
				break;
		}

		// キーバインドの表示を反映する
		RefreshDisplay();

		//--- ボタン入力時の処理を定義
		_pathText.onClick.AddListener(StartRebinding);

		if (_resetButton == null)
		{
			var btns = transform.GetComponentsInChildren<Button>();
			for (int i = 0; i < btns.Length; i++)
			{
				if (btns[i].name == "Reset")
					_resetButton = btns[i];
			}
		}
		_resetButton.onClick.AddListener(ResetOverrides);
	}

	// 後処理
	private void OnDestroy()
	{
		// オペレーションは必ず破棄する必要がある
		CleanUpOperation();
	}

	// リバインドを開始する
	public void StartRebinding()
	{
		// もしActionが設定されていなければ、何もしない
		if (_action == null) return;

		// リバインド対象のBindingIndexを取得
		var bindingIndex = _action.GetBindingIndex(
			InputBinding.MaskByGroup(_scheme)
		);

		// リバインドを開始する
		OnStartRebinding(bindingIndex);
	}

	// 上書きされた情報をリセットする
	public void ResetOverrides()
	{
		// Bindingの上書きを全て解除する
		_action?.RemoveAllBindingOverrides();
		RefreshDisplay();
	}

	// 現在のキーバインド表示を更新
	public void RefreshDisplay()
	{
		if (_action == null || _pathText == null) return;

		//InputBinding binding = InputBinding.MaskByGroup(_scheme);

		//string name = _action.GetBindingDisplayString(binding);

		//_pathText.GetComponentInChildren<TMP_Text>().text = name;

		//Debug.Log($"InputBinding.MaskByGroup(_scheme):{binding}");
		//Debug.Log($"GetBindingDisplayString:{name}");

		_pathText.GetComponentInChildren<TMP_Text>().text = _action.GetBindingDisplayString();
	}

	private void OnStartRebinding(int bindingIndex)
	{
		// もしリバインド中なら、強制的にキャンセル
		// Cancelメソッドを実行すると、OnCancelイベントが発火する
		_rebindOperation?.Cancel();

		// リバインド前にActionを無効化する必要がある
		_action.Disable();

		// ブロッキング用マスクを表示
		if (_mask != null)
			_mask.SetActive(true);

		// リバインドが終了した時の処理を行うローカル関数
		void OnFinished(bool hideMask = true)
		{
			// オペレーションの後処理
			CleanUpOperation();

			// 一時的に無効化したActionを有効化する
			_action.Enable();

			// ブロッキング用マスクを非表示
			if (_mask != null && hideMask)
				_mask.SetActive(false);
		}

		// リバインドのオペレーションを作成し、
		// 各種コールバックの設定を実施し、
		// 開始する
		_rebindOperation = _action
			.PerformInteractiveRebinding(bindingIndex)
			.OnComplete(_ =>
			{
				// リバインドが完了した時の処理
				RefreshDisplay();

				var bindings = _action.bindings;
				var nextBindingIndex = bindingIndex + 1;

				if (nextBindingIndex <= bindings.Count - 1 && bindings[nextBindingIndex].isPartOfComposite)
				{
					// Composite Bindingの一部なら、次のBindingのリバインドを開始する
					OnFinished(false);
					OnStartRebinding(nextBindingIndex);
				}
				else
				{
					OnFinished();
				}
			})
			.OnCancel(_ =>
			{
				// リバインドがキャンセルされた時の処理
				OnFinished();
			})
			// 次のリバインドまでの待機時間を設ける
			.OnMatchWaitForAnother(0.2f)
			// キャンセルキーを設定する
			.WithCancelingThrough("<Keyboard>/escape")
			.Start(); // ここでリバインドを開始する

	}

	// リバインドオペレーションを破棄する
	private void CleanUpOperation()
	{
		// オペレーションを作成したら、Disposeしないとメモリリークする
		_rebindOperation?.Dispose();
		_rebindOperation = null;
	}
}