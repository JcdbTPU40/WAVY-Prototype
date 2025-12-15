# WAVY-Prototype: Copilot agent instructions (Unity 6 / URP)

Unity 6 (6000.1.2f1) のURPアクション試作。プレイヤーは「入力→戦闘→移動→カメラ」をコンポーネントで分離し、敵/タワー/スコアUI/ポーズと連携します。

## まず見る場所
- Scripts: `Assets/Scripts/`（メインループは `PlayerManager`）
- Scenes: `Assets/Scenes/Start.unity`（入口）, `Main.unity` / `Stage1.unity`（プレイ）, 他 `Boss_Input`/`Mario Stage`/`spawner_destruction`
- Packages: `Packages/manifest.json`（URP 17.1.0 / Input System 1.14.0 / AI Navigation 2.0.7 / UGUI 2.0.0）

## フレームの流れ（プレイヤー）
- `PlayerManager.Update()` → `InputManager.HandleAllInputs()` → `PlayerCombat.HandleAllCombatInput()`
- `PlayerLocomotion.HandleAllMovement()` は `FixedUpdate`、`CameraManager.HandleAllCameraMovement()` は `LateUpdate`
- 入力は「1フレームだけ生きるフラグ」設計：`InputManager` が `attackInput/chargeInput/tailInput` を立て、`PlayerCombat` が毎フレーム消費して必ずリセット

## Input System（重要）
- 実行時は生成済み `PlayerInputActions` を使用
- `Assets/PlayerInputActions.cs` は **編集しない**（生成物）
- 変更する場合は `Assets/PlayerInputActions.inputactions` を編集→InspectorからC#再生成
- `InputManager` は `PlayerInputActions.Player.*.performed` を購読して値/フラグを更新（Beam/Throwは仕様から削除され、関連フラグも無効化済み）

## Combat / ダメージ入口（ここに寄せる）
- 尻尾：`PlayerCombat.PerformTailAttackHitBox()` が `TailAttackHitBox.active` を短時間ON → `TailAttackHitBox` が SphereCast/Overlap で命中検出し、
	- 雑魚：`EnemyScript.ApplyDamage(damage, hitPoint, hitNormal)`
	- 中ボス：`BossEnemy.ApplyDamage(...)`
	- ボス：`BossScript.take_Damage(damage)` を呼ぶ（同一スイング内はHashSetで多重ヒット防止）
- 体当たり（Charge）：`PlayerCombat` が移動しつつ OverlapCapsule/OverlapSphere で命中検出。敵はHashSetで多重ダメージ防止、タワーは `chargeTowerHitInterval` で連打制限
- 雑魚の“入口”は `EnemyScript.ApplyDamage(...)`（HP/ヒットFX/ノックバック/死亡/スコア加算/経験値生成まで集約）。`OnTriggerEnter` は `DamegeScript`（綴り注意）も拾う

## AI / タグ / レイヤーの落とし穴
- `Player` タグは `EnemyScript`（追跡）と `CameraManager`（追従）双方の前提。無いと追跡/追従が壊れる
- `NavMeshAgent` 前提：未ベイクや `agent.isOnNavMesh == false` 時に `SetDestination` しない
- 接地：`PlayerLocomotion` は `groundTag`（デフォルト `Ground`）でフィルタして接地扱い
- 死亡後：`EnemyScript` は設定によりレイヤーを `Corpse` に切替（以降のヒット/ノックバック抑止の一環）

## UI / スコア / ポーズ
- スコア：`ScoreManager` はSingleton（必要なら `DontDestroyOnLoad`）+ `ScoreChanged`（UnityEvent）。`ScoreUI` は開始1フレーム待ってから購読して表示を同期
- ポーズ：`GameManager` が `PlayerInputActions.Player.Pause` を購読し `Time.timeScale` とポーズUI/カーソルを切替。必要に応じて `GamepadVirtualCursor` を有効化

## 手動検証（自動テストなし）
- `Start.unity` から再生し、Playerに `PlayerManager`/`InputManager`/`PlayerCombat`/`PlayerLocomotion`/`CharacterController` が付いていること
- 移動/カメラ/ポーズ、Tail/Chargeが「1入力1回」発火、敵が `ChaseStartDistance` 内で追跡し死亡時にスコア加算、タワー破壊で `BossScript.OnTowerDestroyed(pos, duration)` が呼ばれること
