using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamepadVirtualCursor : MonoBehaviour
{
    [Header("UI Cursor")]
    [SerializeField] private RectTransform cursorRect;
    [SerializeField] private Canvas cursorCanvas;

    [Header("Move")]
    [SerializeField] private float cursorSpeed = 1200f;   // px/sec
    [SerializeField] private float deadZone = 0.15f;

    private Mouse virtualMouse;
    private Gamepad pairedGamepad;

    private MouseState virtualMouseState;

    private Vector2 virtualPos;      // screen position
    private bool usingGamepadCursor; // 今どっちを使ってるか

    private const float MouseMoveThresholdSqr = 0.01f;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        SceneManager.sceneLoaded += OnSceneLoaded;
        // いまのシーンでも一回探す
        FindCursorInScene();

        // 起動時に接続済みを拾う
        if (Gamepad.current != null) EnableGamepadCursor(Gamepad.current);
        else EnableMouseCursor();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        DisableVirtualMouse();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCursorInScene();
    }

    void Update()
    {
        // 入力デバイスの“触った方”に寄せてモードを切り替える
        bool mouseMoved = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > MouseMoveThresholdSqr;

        // Mouseが動いたフレームはMouse優先（同フレームに両方動いてもフリップしないように）
        if (mouseMoved)
        {
            if (usingGamepadCursor) EnableMouseCursor();
        }
        else
        {
            Gamepad gp = pairedGamepad != null ? pairedGamepad : Gamepad.current;
            if (!usingGamepadCursor && gp != null && IsGamepadActive(gp))
            {
                EnableGamepadCursor(gp);
            }
        }

        if (!usingGamepadCursor) return;
        if (pairedGamepad == null || virtualMouse == null) return;

        // 左スティックで移動
        Vector2 stick = pairedGamepad.leftStick.ReadValue();
        if (stick.magnitude < deadZone) stick = Vector2.zero;

        virtualPos += stick * (cursorSpeed * Time.unscaledDeltaTime);

        // 画面外に出ないように
        virtualPos.x = Mathf.Clamp(virtualPos.x, 0, Screen.width);
        virtualPos.y = Mathf.Clamp(virtualPos.y, 0, Screen.height);

        // Virtual Mouse に状態反映（InputSystem側）
        // ※Button は bitfield なので control 単体に Change を当てると例外になることがある
        Vector2 prevPos = virtualMouseState.position;
        virtualMouseState.position = virtualPos;
        virtualMouseState.delta = virtualPos - prevPos;

        // 見た目のUIカーソルも動かす
        UpdateCursorVisual(virtualPos);

        // buttonEast(A/×) で左クリック
        bool press = pairedGamepad.buttonEast.isPressed;
        virtualMouseState = virtualMouseState.WithButton(MouseButton.Left, press);

        InputState.Change(virtualMouse, virtualMouseState);
    }

    private bool IsGamepadActive(Gamepad gp)
    {
        // “操作した”判定：スティック/十字/ボタン/トリガーのどれかが閾値を超えたらtrue
        // ※必要ならここに監視するボタンを追加してOK
        Vector2 ls = gp.leftStick.ReadValue();
        Vector2 rs = gp.rightStick.ReadValue();
        if (ls.magnitude >= deadZone) return true;
        if (rs.magnitude >= deadZone) return true;

        if (gp.dpad.ReadValue().sqrMagnitude > 0.01f) return true;

        if (gp.buttonSouth.isPressed) return true;
        if (gp.buttonEast.isPressed) return true;
        if (gp.buttonWest.isPressed) return true;
        if (gp.buttonNorth.isPressed) return true;

        if (gp.leftShoulder.isPressed) return true;
        if (gp.rightShoulder.isPressed) return true;
        if (gp.startButton.isPressed) return true;
        if (gp.selectButton.isPressed) return true;

        if (gp.leftTrigger.ReadValue() > 0.15f) return true;
        if (gp.rightTrigger.ReadValue() > 0.15f) return true;

        return false;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad gp)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                EnableGamepadCursor(gp);
            }
            else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
            {
                if (pairedGamepad == gp) EnableMouseCursor();
            }
        }
    }

    private void EnableGamepadCursor(Gamepad gp)
    {
        pairedGamepad = gp;

        if (virtualMouse == null)
        {
            virtualMouse = InputSystem.AddDevice<Mouse>("VirtualMouse");
        }

        // 中央から開始
        virtualPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        virtualMouseState = default;
        virtualMouseState.position = virtualPos;
        virtualMouseState.delta = Vector2.zero;
        virtualMouseState.scroll = Vector2.zero;
        virtualMouseState.buttons = 0;
        InputState.Change(virtualMouse, virtualMouseState);

        // InputUserに紐づけ（必須ではないけど安定する）
        if (InputUser.all.Count > 0)
        {
            InputUser.PerformPairingWithDevice(virtualMouse, InputUser.all[0]);
            InputUser.PerformPairingWithDevice(pairedGamepad, InputUser.all[0]);
        }

        usingGamepadCursor = true;

        // “見た目カーソル”ON、OSマウスカーソルOFF
        if (cursorRect) cursorRect.gameObject.SetActive(true);
        Cursor.visible = false;

        UpdateCursorVisual(virtualPos);
    }

    private void EnableMouseCursor()
    {
        usingGamepadCursor = false;

        if (cursorRect) cursorRect.gameObject.SetActive(false);
        Cursor.visible = true;

        pairedGamepad = null;
        DisableVirtualMouse();
    }

    private void DisableVirtualMouse()
    {
        if (virtualMouse != null)
        {
            InputSystem.RemoveDevice(virtualMouse);
            virtualMouse = null;
        }
    }

    private void UpdateCursorVisual(Vector2 screenPos)
    {
        if (cursorRect == null || cursorCanvas == null) return;

        // CanvasがScreen Space Overlay想定
        // Screen Space - Camera / World の場合は変換が変わるので言って！
        cursorRect.position = screenPos;
    }

    void FindCursorInScene()
    {
        // 名前で探す（Hierarchyの名前が GamepadCursor 想定）
        var go = GameObject.Find("GamepadCursor");
        if (go != null)
        {
            cursorRect = go.GetComponent<RectTransform>();
            cursorCanvas = go.GetComponentInParent<Canvas>();
            Debug.Log($"[VirtualCursor] Rebind cursor in scene: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            Debug.LogWarning($"[VirtualCursor] GamepadCursor not found in scene: {SceneManager.GetActiveScene().name}");
        }
    }
}
