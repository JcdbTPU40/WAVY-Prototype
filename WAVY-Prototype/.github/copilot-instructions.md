# WAVY-Prototype: AI エージェント向けクイックスタート

Unity 6 URP のアクション・タワーディフェンス風プロトタイプ。プレイヤー vs 敵/タワー、ラグドール死亡、スコア UI が中心です。

## 全体像と構造
- シーン: エントリは `Assets/Scenes/Start.unity`。ゲームプレイは `Main.unity` / `Stage1.unity`。テスト用に `Mario Stage.unity` / `spawner_destruction.unity` / `Boss_Input.unity` があり、`Main(Deprecated).unity` / `Main Menu(Deprecated).unity` はレガシーです。
- パッケージ ( `Packages/manifest.json` ): URP、Input System、AI Navigation、UGUI を利用。URP/AI ナビ系の設定は Unity エディタで調整します。
- コアスクリプトは `Assets/Scripts` 配下。入力は `PlayerInputActions` アセット経由で自動生成された `PlayerInputActions.cs` を使用します (このファイルは直接編集しないこと)。
- プレハブは `Player` タグを前提に敵・タワー・UI から検索されます (例: `EnemyScript`, `EnemyTowerHealth`, `CanvasLookIntoPlayerCamera`)。

## プレイヤー入力と戦闘フロー
- メイン更新パイプライン: `PlayerManager.Update()` → `InputManager.HandleAllInputs()` → `PlayerCombat.HandleAllCombatInput()`。カメラ制御は `CameraManager.LateUpdate()` で実行されます。
- `InputManager` は `attackInput` / `beamInput` / `chargeInput` / `tailInput` を「1フレームだけ true になるフラグ」として保持します。これらは `PlayerCombat` の各ハンドラで処理後に必ず false に戻してください (二重入力防止)。
- 戦闘アクション:
  - テイル攻撃: `Physics.OverlapSphere` + 角度フィルタで近接ヒット判定。
  - ビーム: `beamPrefab` を生成し、`Physics.OverlapCapsule` でヒットを判定。
  - 突進(チャージ): `CharacterController.Move` で移動しつつ、タワーへのマルチヒット防止のために「最後に当てた時刻」をキャッシュしています。
- アニメーション: まず Animator のパラメータ (`Attack` bool, `Tail`/`Beam`/`Charge` トリガー) を優先し、設定されていない場合は `AnimatorManager.PlayTargetAnimation` へフォールバックする設計です。新アクション追加時はどちらのルートを使うか明示してください。

## ダメージと敵の挙動
- 推奨ダメージフロー: 敵へのダメージは極力 `EnemyScript.ApplyDamage(int, Vector3?, Vector3?)` 経由で行い、HP 減少・エフェクト再生・ノックバック・ラグドール化・スコア/EXP 生成を一括処理します。
- インターフェイスフロー: 武器の当たり判定は `WeaponHitbox` → `IDamageable.TakeDamage` を想定。ただし一部の敵は `EnemyScript` のみ実装しているため、`IDamageable` と `EnemyScript` の両対応にする場合があります。
- 旧式プロジェクタイル: `DamageScript.cs` (クラス名 `DamegeScript`) によるダメージが `EnemyScript.OnTriggerEnter` で検知される箇所が残っています。互換性を壊さないように注意してください。
- 敵 AI: `EnemyScript` は `NavMeshAgent` を用いてプレイヤーを追跡し、`AtDistance` 以内でロックオン・攻撃(`Target.SendMessage("TakeDamage", attackDamage)`) を行います。ノックバック時は NavMeshAgent/剛体の有効・無効を切り替え、`liftBeforePhysics` で少し持ち上げてから物理を有効にし、`hitKnockbackRecoveryDelay` 後に制御を戻します。

## タワー・スポーン・スコア
- タワー HP: `EnemyTowerHealth` は最大 3 ハート × 2 HP を管理し、`heartContainer` の子からハート UI を自動取得、毎フレームプレイヤー/カメラ方向を向けます。
- タワー被ダメージ: `Physics.OverlapSphere` で `Player` タグを検出して自傷する仕組みです。新しい攻撃システムと連携する場合は `playerAttackLayers` やタグを調整して統合します。
- タワー死亡時: 半径内に `deathSpawnPrefab` をスポーンし、自身を破壊する挙動がオプションで有効です。
- スコア: `ScoreManager` は `DontDestroyOnLoad` なシングルトンで `ScoreChanged` UnityEvent を発火します。キル処理からは `ScoreManager.Instance?.AddScore(amount)` を呼び出してください。`ScoreUI` はイベントを購読し `Score: {value}` 形式で TextMeshPro に表示します。

## ラグドール・物理挙動
- ラグドール切替: `SimpleRagdoll.SetRagdoll(true)` で Animator/NavMeshAgent を無効化し、子階層の Rigidbody を有効化して連続衝突判定に切り替えます。死亡処理では `Die` 内でインパルスを加えます。
- 物理安定化: 新たに力を加える場合は、まず `LiftAboveGround` 相当の処理でコライダーを少し持ち上げてから物理有効化し、トンネリングを避けてください。
- 死体の後処理: ノックバック無効化フラグ `disableKnockbackAfterDeath` と自動消滅時間 `autoDestroySec` を尊重し、他の敵と挙動がズレないようにします。

## 開発ワークフロー
- 実行: Unity エディタで `Assets/Scenes/Start.unity` を開き、Play で開始します。`Player` GameObject に `Player` タグが付いていることが前提です。
- 入力アセット編集: 入力定義を変更する場合は `PlayerInputActions.inputactions` を編集し、インスペクタの「Generate C# Class」から `PlayerInputActions.cs` を再生成します (`Assets/PlayerInputActions.cs` を直接編集しないこと)。`InputSystem_Actions.inputactions` という別アセットもありますが、実際のコードは `PlayerInputActions` 側のみを使用します。
- ナビメッシュ: AI を正しく動かすには、アクティブシーンで NavMesh をベイクし、敵を NavMesh 上に配置してください。スクリプト側からは `agent.isOnNavMesh` を前提としています。
- テスト: 自動テストはありません。特に Input System や NavMesh 周りを変更した場合は、エディタ上で再現手順を README やこのファイルに簡潔に追記してください。
- Git: 作業ブランチは汚れている可能性があるため、大きなリファクタリングではなく「既存パターンをなぞる小さめの変更」を優先し、新しいパターンを導入した場合はここに必ずメモを追加してください。

## 変更時のチェックリスト
- `Start.unity` からプレイし、入力 (Tail/Beam/Charge) が 1 入力 1 アクションになっているか、クールダウンが効いているか確認する。
- プレイヤー・敵・タワーが想定どおり `Player` / それ以外のタグ・レイヤーでヒットしているか確認する。
- NavMesh 上で敵がプレイヤーを追い、距離条件で攻撃に移行できているか確認する。
- 敵・タワー死亡時にスコア加算とラグドール/スポーン演出が破綻していないか確認する。
