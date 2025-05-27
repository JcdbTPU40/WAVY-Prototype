using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowCircle : MonoBehaviour
{
    public Transform player;
    public float offset = 6f;
    public float height = 8f;
    public float smoothSpeed = 5f;
    public float peekOffset = 2f;
    public static bool IsPeeking { get; private set; } = false;

    private Vector2 lookInput = Vector2.zero;

    void Update()
    {
        // Gamepad入力
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            lookInput = gamepad.rightStick.ReadValue();
        }
        else
        {
            // キーボード入力（Shift+A or Shift+D）
            bool shift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            bool leftPeek = Keyboard.current.aKey.isPressed;
            bool rightPeek = Keyboard.current.dKey.isPressed;

            if (shift && leftPeek)
                lookInput = new Vector2(-1f, 0f); // 左覗き
            else if (shift && rightPeek)
                lookInput = new Vector2(1f, 0f);  // 右覗き
            else
                lookInput = Vector2.zero;        // 覗きなし
        }

        IsPeeking = (lookInput.x != 0f);
    }

    void LateUpdate()
    {
        if (player == null) return;

        float playerRadius = new Vector2(player.position.x, player.position.z).magnitude;
        float angle = Mathf.Atan2(player.position.z, player.position.x);

        float camRadius = playerRadius + offset;
        float baseX = Mathf.Cos(angle) * camRadius;
        float baseZ = Mathf.Sin(angle) * camRadius;

        // 覗き見角度：最大 ±0.5ラジアン（約30度）
        float peekAngle = angle + (lookInput.x * 1.5f);
        float peekX = Mathf.Cos(peekAngle) * peekOffset;
        float peekZ = Mathf.Sin(peekAngle) * peekOffset;

        Vector3 targetPosition = new Vector3(baseX + peekX, height, baseZ + peekZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        transform.LookAt(player.position);
    }
}
