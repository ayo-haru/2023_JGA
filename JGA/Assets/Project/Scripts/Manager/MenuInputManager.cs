//=============================================================================
// @File	: [MenuInputManager.cs]
// @Brief	: メニュー入力管理用
// @Author	: Ogusu Yuuko
// @Editer	: 
// @Detail	: 
// 
// [Date]
// 2023/07/02	スクリプト作成
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public struct MenuSettingItem
{
    //デフォルトのボタン
    public readonly Selectable button;
    //bMouseの代入先
    public readonly List<Image> images;
    public readonly List<BoxCollider> boxColliders;
    public delegate void OnMove(InputAction.CallbackContext context);
    public OnMove onMove; 
    //ｺﾝｽﾄﾗｸﾀ
    public MenuSettingItem(Selectable button, List<Image> images)
    {
        this.button = button;
        this.images = new List<Image>();
        this.images = images;
        this.boxColliders = null;
        onMove = null;
    }
    public MenuSettingItem(Selectable button, List<BoxCollider> boxColliders)
    {
        this.button = button;
        this.images = null;
        this.boxColliders = new List<BoxCollider>();
        this.boxColliders = boxColliders;
        onMove = null;
    }
    public MenuSettingItem(Selectable button)
    {
        this.button = button;
        images = null;
        boxColliders = null;
        onMove = null;
    }
    public MenuSettingItem(int i = 0)
    {
        button = null;
        images = null;
        boxColliders = null;
        onMove = null;
    }
}

public class MenuInputManager : SingletonMonoBehaviour<MenuInputManager>
{
    [SerializeField] private InputActionReference actionMove;

    //マウス位置
    private static Vector3 mousePos = Vector3.zero;
    //マウス使用フラグ
    private static bool bMouse = false;
    //使用するボタン等
    private static Stack<MenuSettingItem> menus = new Stack<MenuSettingItem>();

    protected override void Awake()
    {
        base.Awake();
        //イベント登録
        actionMove.action.performed += MenuMove;
        actionMove.action.canceled += MenuMove;
        actionMove.ToInputAction().Disable();
        actionMove.ToInputAction().Enable();
    }
#if false
    /// <summary>
    /// 最初のフレーム更新の前に呼び出される
    /// </summary>
    void Start()
	{
        
    }
#endif

	/// <summary>
	/// 1フレームごとに呼び出される（端末の性能によって呼び出し回数が異なる）：inputなどの入力処理
	/// </summary>
	void Update()
	{
        //マウスの状態を更新
        Vector3 oldMousePos = mousePos;
        mousePos = Input.mousePosition;

        if (bMouse) return;

        //マウスが動かされたらマウス入力に切り替え
        if (Vector3.SqrMagnitude(oldMousePos - mousePos) >= 2.0f)
        {
            ChangeInput();
        }
    }

    void OnDestroy()
    {
        if (MenuInputManager.Instance != this) return;

        //イベント削除
        actionMove.action.performed -= MenuMove;
        actionMove.action.canceled -= MenuMove;
        actionMove.ToInputAction().Disable();
        menus.Clear();
    }

    private static void MenuMove(InputAction.CallbackContext context)
    {
        //メニューがない場合は処理しない
        if (menus.Count <= 0) return;

        //入力切替以外の処理
        if (menus.TryPeek(out MenuSettingItem result))
        {
            if (result.onMove != null)
            {
                result.onMove.Invoke(context);
            }
        }

        if (!bMouse) return;

        //マウス→コントローラ
        ChangeInput();
    }

    private static void InitInput()
    {
        mousePos = Input.mousePosition;
        bMouse = true;
        ChangeInput();
    }

    private static void ChangeInput()
    {
        //選択を解除
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
        //メニューがなかった場合はマウスカーソルを表示
        if(!menus.TryPeek(out MenuSettingItem result))
        {
            Cursor.visible = true;
            return;
        }

        if (!result.button) return;
        
        //マウス→コントローラ
        if (bMouse)
        {
            result.button.Select();
        }

        //入力を切り替え
        bMouse = !bMouse;
        Cursor.visible = bMouse;
        if(result.images != null)
        {
            for (int i = 0; i < result.images.Count; ++i)
            {
                result.images[i].raycastTarget = bMouse;
            }
        }
        else if(result.boxColliders != null)
        {
            for (int i = 0; i < result.boxColliders.Count; ++i)
            {
                result.boxColliders[i].enabled = bMouse;
            }
        }
    }
    /// <summary>
    /// メニュー画面登録
    /// </summary>
    public static void PushMenu(MenuSettingItem item)
    {
        menus.Push(item);
        InitInput();
    }
    /// <summary>
    /// メニュー画面削除
    /// </summary>
    public static void PopMenu()
    {
        menus.TryPop(out MenuSettingItem item);
        InitInput();
    }

    public static void Create()
    {
        if (MenuInputManager.Instance) return;

        GameObject instance = new GameObject();
        instance.SetActive(false);
        MenuInputManager manager = instance.AddComponent<MenuInputManager>();
        manager.enabled = true;
        instance = Instantiate(instance);
        instance.name = "MenuInputManager";
        instance.SetActive(true);
    }
}

