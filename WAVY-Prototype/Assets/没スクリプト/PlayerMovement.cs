using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float radius = 9f;         // ���݂̔��a
    public float minRadius = 3f;    // ���S�ɋ߂Â���ŏ����a
    public float maxRadius = 9f;      // �O���̍ő唼�a

    private float angle = 0f;
    private Vector2 moveInput;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        radius = maxRadius; // �����ʒu�͊O��
    }

    void Update()
    {
        // �`�����Ȃ狭���I�Ɉړ����͂��[���ɂ���
        bool shift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        bool leftPeek = Keyboard.current.aKey.isPressed;
        bool rightPeek = Keyboard.current.dKey.isPressed;

        if ((shift && leftPeek) || (shift && rightPeek) || CameraFollowCircle.IsPeeking)
        {
            moveInput = Vector2.zero;
        }

        // ���E�ړ��i�~����̈ړ��j
        float arcLength = moveInput.x * moveSpeed * Time.deltaTime;
        float deltaAngle = arcLength / radius;
        angle += deltaAngle;

        // ���ړ��i���S�����̈ړ��j
        float deltaRadius = moveInput.y * moveSpeed * Time.deltaTime;
        radius -= deltaRadius; // y���Œ��S�ɋ߂Â�
        radius = Mathf.Clamp(radius, minRadius, maxRadius);

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        Vector3 newPosition = new Vector3(x, transform.position.y, z);

        rb.MovePosition(newPosition);

        // ���S������悤�ɉ�]
        Vector3 lookTarget = new Vector3(0f, transform.position.y, 0f);
        Quaternion lookRotation = Quaternion.LookRotation(lookTarget - newPosition, Vector3.up);
        rb.MoveRotation(lookRotation);
    }

    public void OnMove(InputValue value)
    {
            moveInput = value.Get<Vector2>();
    }

}