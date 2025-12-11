# WAVY-Prototype: AI agent quickstart (Unity 6 URP)

Unity URP action prototype: player vs enemies/towers with ragdoll deaths, score UI, and Input System.

## Project shape
- Scenes: `Start.unity` (entry), `Main.unity` / `Stage1.unity` for play; test scenes exist (`Mario Stage`, `spawner_destruction`, `Boss_Input`). Legacy: `Main(Deprecated)`, `Main Menu(Deprecated)`.
- Packages: URP 17.1.0, Input System 1.14.0, AI Navigation 2.0.7, UGUI 2.0.0.
- Scripts live in `Assets/Scripts`. Do **not** edit `PlayerInputActions.cs`; change `PlayerInputActions.inputactions` then regenerate via inspector. `InputSystem_Actions.inputactions` exists but gameplay uses `PlayerInputActions`.
- Prefabs and AI rely on `Player` tag for lookups (enemy chase/damage, tower hearts facing, camera target).

## Player loop and input
- Frame order: `PlayerManager.Update()` -> `InputManager.HandleAllInputs()` -> `PlayerCombat.HandleAllCombatInput()`; `PlayerLocomotion.HandleAllMovement()` in `FixedUpdate`; `CameraManager.HandleAllCameraMovement()` in `LateUpdate`.
- `InputManager` sets one-frame flags (`attackInput`, `beamInput`, `chargeInput`, `tailInput`) and movement/camera axes; combat handlers must clear flags (done inside `PlayerCombat`).
- `CameraManager` auto-finds `Player` tag (fallback to `PlayerScript`/`PlayerManager`); handles follow/rotate/collision. Keep `cameraPivot` assigned and collision layers set.
- `GameManager` wires Input System `Pause` action; toggles `Time.timeScale`, cursor lock, and pause menu. Use `ReturnToMainMenu` to load `Start` scene.

## Combat patterns (`PlayerCombat`)
- Tail: uses `TailAttackHitBox` component toggled via coroutine; angle+radius filter with `Physics.OverlapSphere`; knockback via `ApplyKnockback` (NavMeshAgent.Move or Rigidbody.AddForce fallback).
- Beam/Throw: mode switch `beamOrThrowMode` (Beam=spawn `beamPrefab` at `beamOffset` with capsule damage after `beamHitDelay`; Throw=spawn `throwProjectilePrefab` with ballistic velocity, optional `LandingArea`). Cooldowns per mode.
- Charge: moves via `CharacterController.Move` for `chargeDuration`; caches hit enemies/bosses/towers to avoid repeated hits (`chargeTowerHitInterval`).
- Animator use is optional: checks `Tail`/`Beam`/`Charge` triggers and `Attack` bool before falling back to `AnimatorManager.PlayTargetAnimation`.
- Layer filtering: `enemyLayers` defaults to all; adjust when adding new enemy layers.

## Enemies and damage
- Preferred entry point: `EnemyScript.ApplyDamage(int, Vector3?, Vector3?)` handles HP, hit FX (optional delay), knockback gating, ragdoll, score, EXP spawn, and corpse layer swap when `disableKnockbackAfterDeath` is true.
- AI: `EnemyScript` chases via `NavMeshAgent` once player within `ChaseStartDistance`; stops at `AtDistance` and attacks via `Target.SendMessage("TakeDamage", attackDamage)` if tagged `Player`. Ensure agents are on baked NavMesh.
- Ragdoll: `SimpleRagdoll.Die` called on death; lifts slightly (`liftBeforePhysics`) to avoid ground overlap. Honors `deathKnockback` settings.
- Legacy hits: `DamegeScript` (spelling intentional) still detected in `OnTriggerEnter`; `WeaponHitbox` + `IDamageable` also present.

## Towers and spawns
- `EnemyTowerHealth` holds up to 3 hearts × 2 HP; auto-fills heart images from `heartContainer` if inspector list empty; faces player/camera each frame.
- Self-damage when `Physics.OverlapSphere` finds colliders tagged `Player` on `playerAttackLayers`; tune `detectionRadius`/`damageInterval` to integrate new attacks.
- On destroy: optional `deathSpawnPrefab` radial instantiation, notifies every `BossScript` via `OnTowerDestroyed(position, stayDuration)`, then destroys self.

## Score and UI
- `ScoreManager` is a singleton (`DontDestroyOnLoad` when configured) exposing `ScoreChanged` UnityEvent; call `ScoreManager.Instance?.AddScore(amount)` on kills. Starting score set in inspector; static `CurrentScore` holds value.
- `ScoreUI` listens to `ScoreManager.ScoreChanged` to display `Score: {value}`; ensure TMP reference set. `ShowScore`/`PopUpController` used in menus.

## Workflow notes
- Play from `Start.unity`; confirm player object is tagged `Player` and has `InputManager`/`PlayerCombat`/`PlayerLocomotion` (as needed) plus `CharacterController` for charge.
- When editing input actions, regenerate `PlayerInputActions.cs` from the asset inspector (Generate C# Class) so bindings update; avoid touching the generated file directly.
- No automated tests; validate changes in the Unity Editor. If modifying navmesh agents or camera collisions, include manual repro steps in PRs/issues.
- Repo may be dirty; do not revert unrelated changes. Document new patterns here when adding systems.

## Quick manual validation
- Load `Start.unity`, press Play; ensure `ScoreManager` exists in scene or as bootstrap.
- Verify movement/camera, pause/resume, and that attacks fire once per input respecting cooldowns.
- Check enemies chase only within `ChaseStartDistance`, stop at `AtDistance`, and add score on death; corpses ragdoll without clipping.
- Destroy a tower: hearts decrement, overlap damage triggers, death spawns (if set) and bosses receive `OnTowerDestroyed`.
