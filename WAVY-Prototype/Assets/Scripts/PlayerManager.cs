using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    PlayerCombat playerCombat; // 攻撃システムを追加

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = Object.FindAnyObjectByType<CameraManager>();
        //playerLocomotion = GetComponent<PlayerLocomotion>();
        playerCombat = GetComponent<PlayerCombat>(); // 攻撃システムのコンポーネントを取得
    }

    private void Update()
    {
        inputManager.HandleAllInputs();

        playerCombat.HandleAllCombatInput();
    }

    private void FixedUpdate()
    {
        //playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
    }
}