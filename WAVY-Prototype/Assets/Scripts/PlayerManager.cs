using UnityEngine;

// PlayerManager - プレイヤーシステムの統括クラス
// 入力 -> 移動 -> 戦闘 -> カメラ の順序でコンポーネントを制御

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    PlayerCombat playerCombat;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = Object.FindAnyObjectByType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerCombat = GetComponent<PlayerCombat>();

        // 必須コンポーネントのチェック
        if (inputManager == null)
        {
            Debug.LogError("InputManager が見つかりません", this);
        }
        if (playerLocomotion == null)
        {
            Debug.LogWarning("PlayerLocomotion が見つかりません。移動機能が無効です。", this);
        }
        if (playerCombat == null)
        {
            Debug.LogWarning("PlayerCombat が見つかりません。戦闘機能が無効です。", this);
        }
    }

    private void Update()
    {
        // 1. 入力を処理
        if (inputManager != null)
        {
            inputManager.HandleAllInputs();
        }

        // 2. 戦闘入力を処理（InputManager の後に実行）
        if (playerCombat != null)
        {
            playerCombat.HandleAllCombatInput();
        }
    }

    private void FixedUpdate()
    {
        // 3. 移動処理（物理演算と同期）
        if (playerLocomotion != null)
        {
            playerLocomotion.HandleAllMovement();
        }
    }

    private void LateUpdate()
    {
        // 4. カメラ処理（キャラクター移動の後）
        if (cameraManager != null)
        {
            cameraManager.HandleAllCameraMovement();
        }
    }
}