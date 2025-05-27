using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float radius = 9f;         // 現在の半径
    public float minRadius = 3f;    // 中心に近づける最小半径
    public float maxRadius = 9f;      // 外周の最大半径

    private float angle = 0f;
    private Vector2 moveInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        radius = maxRadius; // 初期位置は外周
    }

    void Update()
    {
        // 覗き中なら強制的に移動入力をゼロにする
        bool shift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        bool leftPeek = Keyboard.current.aKey.isPressed;
        bool rightPeek = Keyboard.current.dKey.isPressed;

        if ((shift && leftPeek) || (shift && rightPeek) || CameraFollowCircle.IsPeeking)
        {
            moveInput = Vector2.zero;
        }

        // 左右移動（円周上の移動）
        float arcLength = moveInput.x * moveSpeed * Time.deltaTime;
        float deltaAngle = arcLength / radius;
        angle += deltaAngle;

        // 奥移動（中心方向の移動）
        float deltaRadius = moveInput.y * moveSpeed * Time.deltaTime;
        radius -= deltaRadius; // y正で中心に近づく
        radius = Mathf.Clamp(radius, minRadius, maxRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 newPosition = new Vector3(x, transform.position.y, z);

        rb.MovePosition(newPosition);

        // 中心を見るように回転
        Vector3 lookTarget = new Vector3(0f, transform.position.y, 0f);
        Quaternion lookRotation = Quaternion.LookRotation(lookTarget - newPosition, Vector3.up);
        rb.MoveRotation(lookRotation);
    }

    public void OnMove(InputValue value)
    {
            moveInput = value.Get<Vector2>();
    }

}