using UnityEngine;

// Player の基本コンポーネント - CameraManager からの参照用
// 移動は PlayerLocomotion、戦闘は PlayerCombat に分離済み
// このスクリプトは後方互換性のために最小限の機能のみ保持
// 実際のゲームロジックは PlayerManager が統括

public class PlayerScript : MonoBehaviour
{
    // CameraManager が PlayerScript.transform を参照するため、このクラスは残す
    // 機能は全て PlayerManager -> InputManager/PlayerLocomotion/PlayerCombat に移譲済み
}


