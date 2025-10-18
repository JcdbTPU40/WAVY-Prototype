# WAVY-Prototype: AI agent quickstart (Unity URP)

Unity 6 URP prototype featuring player vs enemies/towers, ragdoll deaths, and score UI.

## Project shape
- Scenes: Start.unity (main menu entry), Main.unity / Stage1.unity for gameplay; playmode starts from Start.
- Packages: URP (com.unity.render-pipelines.universal 17.x), Input System 1.14, AI Navigation 2.x, UGUI 2.0.
- Core scripts live in Assets/Scripts; keep generated PlayerInputActions.cs untouched—edit PlayerInputActions.inputactions then regenerate.
- Prefabs expect `Player` tag for lookups (`EnemyScript`, `EnemyTowerHealth`, UI heart facing).

## Player systems
- Pipeline: `PlayerManager.Update()` calls `InputManager.HandleAllInputs()` then `PlayerCombat.HandleAllCombatInput()`; `CameraManager.HandleAllCameraMovement()` runs in LateUpdate.
- `InputManager` captures action booleans (`attackInput`, `beamInput`, `chargeInput`, `tailInput`) as one-frame flags—combat scripts must reset them after consumption.
- `PlayerCombat` gates tail/beam/charge via cooldown coroutines; tail uses `Physics.OverlapSphere` + angle filter, beam spawns `beamPrefab` and hits via `Physics.OverlapCapsule`, charge moves with `CharacterController.Move` and caches tower hit timestamps.
- Animator hooks are optional: triggers/bools checked before fallback to `AnimatorManager.PlayTargetAnimation`.

## Damage patterns
- Preferred enemy flow: call `EnemyScript.ApplyDamage(int, Vector3?, Vector3?)` to manage HP, FX, knockback, ragdoll, score, and EXP spawn.
- Interface flow: `WeaponHitbox` invokes `IDamageable.TakeDamage`; some enemies only implement `EnemyScript` so dual-support collisions as needed.
- Legacy projectiles rely on `DamegeScript` (intentional spelling) detected inside `EnemyScript.OnTriggerEnter`.

## Enemy behaviour
- `EnemyScript` uses `NavMeshAgent` to chase until `AtDistance`, then locks on and optionally attacks via `Target.SendMessage("TakeDamage", attackDamage)`.
- Knockback toggles the agent/Rigidbody, lifts slightly (`liftBeforePhysics`), and restores after `hitKnockbackRecoveryDelay`; death flips to ragdoll and optionally zeroes forces via `disableKnockbackAfterDeath`.
- Ensure NavMesh baked and agents placed on it; inspector `EnemySpeed`, `AtDistance`, and score fields drive runtime behaviour.

## Towers and spawns
- `EnemyTowerHealth` tracks up to 3 hearts × 2 HP, auto-populates heart images from `heartContainer`, and faces the player/camera each frame.
- Towers self-damage when `Physics.OverlapSphere` detects colliders tagged `Player`; adjust `playerAttackLayers` or tag to integrate new attacks.
- On death the tower optionally instantiates `deathSpawnPrefab` in a radius and destroys itself.

## Score and UI
- `ScoreManager` is a singleton (`DontDestroyOnLoad`) firing `ScoreChanged` UnityEvent; call `ScoreManager.Instance?.AddScore(amount)` on kill events.
- `ScoreUI` listens to the event and formats `Score: {value}`; ensure TMP reference is assigned in the inspector.

## Ragdolls and physics
- `SimpleRagdoll.SetRagdoll(true)` disables animator/agent, enables child rigidbodies with continuous collision, and applies impulses in `Die`.
- When you add forces, lift the character slightly (`LiftAboveGround`) before enabling physics to avoid tunneling.
- Keep corpse cleanup consistent by honoring `disableKnockbackAfterDeath` and `autoDestroySec`.

## Workflow tips
- Play from Start.unity; verify player input via new Input System (Player action map) and regenerate bindings after asset edits.
- No automated tests; iteration happens in the Unity Editor. If AI-origin changes touch input or navmesh, include repro steps for manual validation.
- Git repo may be dirty; avoid reverting user changes. Document new instructions or patterns in this file when adding systems.
