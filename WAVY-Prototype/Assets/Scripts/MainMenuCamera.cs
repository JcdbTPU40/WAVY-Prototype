using UnityEngine;

/// <summary>
/// メインメニュー画面で背景用のゆったりとしたカメラ演出を行う。
/// 指定した中心点をゆっくり周回または往復させることができる。
/// </summary>
public class MainMenuCamera : MonoBehaviour
{
    public enum MotionMode
    {
        Orbit,
        PingPong
    }

    [Header("共通設定")]
    [SerializeField] private MotionMode motionMode = MotionMode.Orbit;
    [SerializeField] private Transform focusTarget;            // 注視するターゲット（未指定なら開始位置の前方）
    [SerializeField] private Vector3 focusOffset = Vector3.zero;// ターゲットからのオフセット
    [SerializeField] private float lookSmoothing = 2f;          // ターゲット方向を見るときの補間速度

    [Header("周回設定 (Orbit)")]
    [SerializeField] private float orbitRadius = 10f;           // ターゲット周回時の半径
    [SerializeField] private float orbitSpeed = 5f;             // 角速度（度/秒）
    [SerializeField] private Vector3 orbitAxis = Vector3.up;    // 回転軸

    [Header("往復設定 (PingPong)")]
    [SerializeField] private Vector3 moveAxis = Vector3.right;  // 往復する方向
    [SerializeField] private float moveDistance = 5f;           // 往復距離
    [SerializeField] private float moveDuration = 10f;          // 往復にかかる時間

    [Header("ズーム演出")]
    [SerializeField] private bool enableZoom = false;
    [SerializeField] private float zoomAmplitude = 1f;          // ズームの振幅
    [SerializeField] private float zoomFrequency = 0.2f;        // ズームの周期

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float moveTimer;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (focusTarget == null)
        {
            // ターゲットが設定されていなければ、カメラの前方方向に仮想ターゲットを置く
            GameObject fallbackTarget = new GameObject("MainMenuCameraFocus");
            fallbackTarget.transform.position = transform.position + transform.forward * orbitRadius;
            focusTarget = fallbackTarget.transform;
        }
    }

    private void Update()
    {
        switch (motionMode)
        {
            case MotionMode.Orbit:
                UpdateOrbitMotion();
                break;
            case MotionMode.PingPong:
                UpdatePingPongMotion();
                break;
        }

        if (enableZoom)
        {
            ApplyZoomEffect();
        }

        SmoothLookAtTarget();
    }

    private void UpdateOrbitMotion()
    {
        if (focusTarget == null)
        {
            return;
        }

        float angle = orbitSpeed * Mathf.Deg2Rad * Time.time;
        Quaternion rotation = Quaternion.AngleAxis(orbitSpeed * Time.deltaTime, orbitAxis.normalized);

        Vector3 offset = transform.position - focusTarget.position;
        if (offset.sqrMagnitude < Mathf.Epsilon)
        {
            offset = new Vector3(orbitRadius, 0f, 0f);
        }

        offset = rotation * offset.normalized * orbitRadius;
        transform.position = focusTarget.position + offset;
    }

    private void UpdatePingPongMotion()
    {
        moveTimer += Time.deltaTime;
        if (moveDuration <= Mathf.Epsilon)
        {
            moveDuration = 0.1f;
        }

        float t = Mathf.PingPong(moveTimer / moveDuration, 1f);

        Vector3 offset = moveAxis.normalized * moveDistance;
        transform.position = initialPosition + Vector3.Lerp(-offset, offset, t);
    }

    private void ApplyZoomEffect()
    {
        if (focusTarget == null)
        {
            return;
        }

        float zoom = Mathf.Sin(Time.time * Mathf.PI * 2f * zoomFrequency) * zoomAmplitude;
        Vector3 direction = (transform.position - focusTarget.position).normalized;
        transform.position += direction * zoom * Time.deltaTime;
    }

    private void SmoothLookAtTarget()
    {
        if (focusTarget == null)
        {
            return;
        }

        Vector3 targetPosition = focusTarget.position + focusOffset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lookSmoothing);
    }

    private void OnDrawGizmosSelected()
    {
        if (focusTarget == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, focusTarget.position + focusOffset);

        if (motionMode == MotionMode.Orbit)
        {
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireSphere(focusTarget.position, orbitRadius);
        }
        else
        {
            Gizmos.color = new Color(0.3f, 1f, 0.6f, 0.5f);
            Vector3 offset = moveAxis.normalized * moveDistance;
            Gizmos.DrawLine(initialPosition - offset, initialPosition + offset);
        }
    }
}
