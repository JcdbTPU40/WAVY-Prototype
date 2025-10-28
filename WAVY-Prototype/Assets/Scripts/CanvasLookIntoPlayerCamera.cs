using UnityEngine;

public class CanvasLookIntoPlayerCamera : MonoBehaviour
{
    [Header("Target Camera")]
    [Tooltip("手動で参照するカメラ。未指定なら Player タグ配下のカメラ→Camera.main の順で解決します。")]
    public Transform overrideCamera;
    [Tooltip("Player をタグで辿ってカメラを探します。")]
    public string playerTag = "Player";

    [Header("Billboard Options")]
    [Tooltip("true: Y軸のみ回転（地面水平の看板風） / false: カメラへ完全に正対")]
    public bool yAxisOnly = true;
    [Tooltip("回転の補間速度（0で即時追従）")]
    [Min(0f)] public float rotationLerpSpeed = 0f;

    Transform cam;

    void Awake()
    {
        ResolveCamera(true);
    }

    void OnEnable()
    {
        // 再有効化時にカメラが無ければ再解決
        if (cam == null) ResolveCamera(true);
    }

    void LateUpdate()
    {
        if (cam == null || !cam.gameObject.activeInHierarchy)
        {
            // 失われた場合は軽く再探索
            ResolveCamera(false);
            if (cam == null) return;
        }

        Vector3 targetPos = cam.position;
        Vector3 dir = targetPos - transform.position;
        if (yAxisOnly)
        {
            dir.y = 0f; // 水平のみで正対
        }

        if (dir.sqrMagnitude < 1e-6f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        if (rotationLerpSpeed > 0f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
        }
        else
        {
            transform.rotation = targetRot;
        }
    }

    void ResolveCamera(bool firstTry)
    {
        // 明示指定があればそれを採用
        if (overrideCamera != null)
        {
            cam = overrideCamera;
            return;
        }

        // Playerタグ配下からカメラを探索
        if (!string.IsNullOrEmpty(playerTag))
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                var playerCam = player.GetComponentInChildren<Camera>(true);
                if (playerCam != null)
                {
                    cam = playerCam.transform;
                    return;
                }
            }
        }

        // それでも無ければ Camera.main にフォールバック
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
            return;
        }

        if (firstTry)
        {
            Debug.LogWarning("CanvasLookIntoPlayerCamera: 参照できるカメラが見つかりませんでした。overrideCamera を指定するか Player タグ/Camera.main を確認してください。", this);
        }
    }
}
