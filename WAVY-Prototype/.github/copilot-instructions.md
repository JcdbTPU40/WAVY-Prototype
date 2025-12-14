# WAVY-Prototype: Copilot agent instructions (Unity 6 / URP)

Unity 6 (6000.1.2f1) のURPアクション試作。プレイヤーの入力→移動→戦闘→カメラをコンポーネントで分離し、敵/タワー/スコアUIと連携します。

## Repo quick facts
- Scripts: `Assets/Scripts/`（主要ループは `PlayerManager`）
- Scenes: `Assets/Scenes/Start.unity`（入口）, `Main.unity` / `Stage1.unity`（プレイ）, 他に `Boss_Input`/`Mario Stage`/`spawner_destruction` など
- Packages: URP 17.1.0 / Input System 1.14.0 / AI Navigation 2.0.7 / UGUI 2.0.0（`Packages/manifest.json`）

## Core frame flow (player)
- 実行順: `PlayerManager.Update()` → `InputManager.HandleAllInputs()` → `PlayerCombat.HandleAllCombatInput()`
- 物理: `PlayerLocomotion.HandleAllMovement()` は `FixedUpdate`
- カメラ: `CameraManager.HandleAllCameraMovement()` は `LateUpdate`
- `InputManager` は1フレームだけのフラグ（`attackInput/beamInput/chargeInput/tailInput`）を立てる設計。消費側（`PlayerCombat`）が毎回リセットする。

## Input System (重要)
- ゲームプレイは生成済み `PlayerInputActions` を使用。
- `Assets/PlayerInputActions.cs` は **編集しない**（生成物）。変更は `Assets/PlayerInputActions.inputactions` を編集→InspectorでC#再生成。
- ポーズは `GameManager` が `PlayerInputActions.Player.Pause` を購読し、`Time.timeScale` とカーソル/ポーズUIを切り替える。

## Combat / damage conventions
- 尻尾: `PlayerCombat` が `TailAttackHitBox.active` を短時間ONし、`TailAttackHitBox` が `EnemyScript.ApplyDamage(damage, hitPoint, hitNormal)` を呼ぶ。
- 敵ダメージの“入口”: `EnemyScript.ApplyDamage(...)`（HP・ヒットFX・ノックバック・死亡処理・スコア加算・経験値生成まで集約）。
- 死亡: `SimpleRagdoll.Die(...)` を呼び、必要に応じてレイヤーを `Corpse` に切替（`disableKnockbackAfterDeath`）。
- 互換: `EnemyScript.OnTriggerEnter` は `DamegeScript`（綴り注意）も拾う。`WeaponHitbox`/`IDamageable` も存在。

## AI / tags / layers
- 敵AIは `NavMeshAgent` 前提（追跡は `Player` タグを検索）。NavMesh未ベイクや `agent.isOnNavMesh` に注意。
- `CameraManager` も `Player` タグで追従対象を検索（無い場合 `PlayerScript`/`PlayerManager` にフォールバック）。
- 移動は `CharacterController` 前提。接地は `PlayerLocomotion` の `groundTag`（デフォルト `Ground`）でフィルタする。

## Towers / score UI hooks
- `EnemyTowerHealth`: ハートUI(最大3×2HP)、`OverlapSphere` + `Player` タグで自己ダメージ、破壊時に全 `BossScript` へ `OnTowerDestroyed(pos, duration)` 通知。
- `ScoreManager`: Singleton + `ScoreChanged` UnityEvent。`ScoreUI` は起動後に購読してテキスト更新。

## Manual validation (no automated tests)
- `Start.unity` から再生。Playerが `Player` タグで、`InputManager`/`PlayerCombat`/`PlayerLocomotion`/`CharacterController` が付くこと。
- 移動/カメラ/ポーズ、攻撃の1入力1回発火、敵追跡（`ChaseStartDistance`）と死亡時スコア・ラグドール、タワー破壊通知を確認。
