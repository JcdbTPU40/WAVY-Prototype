# プレイヤースクリプト整理完了レポート

## 変更概要

`PlayerScript.cs` と `PlayerLocomotion.cs` の重複機能を整理し、プロジェクト標準のアーキテクチャに統一しました。

## アーキテクチャ構成

### PlayerManager（統括クラス）
```
PlayerManager
├─ InputManager      (入力処理)
├─ PlayerLocomotion  (移動処理)
├─ PlayerCombat      (戦闘処理)
└─ CameraManager     (カメラ処理)
```

### 実行順序
1. **Update**: InputManager → PlayerCombat
2. **FixedUpdate**: PlayerLocomotion（物理演算と同期）
3. **LateUpdate**: CameraManager（移動後にカメラ追従）

## 変更内容

### 1. PlayerScript.cs
**変更前**: 
- CharacterController での移動制御
- Input System の直接使用
- アニメーション制御
- 戦闘処理の呼び出し

**変更後**:
- 空のクラスとして保持（後方互換性）
- CameraManager からの参照用
- すべての機能を他のクラスに移譲

### 2. PlayerLocomotion.cs
**変更前**:
- Rigidbody ベースの移動
- PlayerManager からコメントアウトされていた

**変更後**:
- **CharacterController ベースに変更**（プロジェクト標準）
- 重力処理を追加
- アニメーター連携を追加（Speed, time パラメータ）
- 物理押し出し処理（OnControllerColliderHit）
- PlayerManager から正式に有効化

### 3. PlayerManager.cs
**変更前**:
- PlayerLocomotion がコメントアウト
- PlayerCombat のみ有効

**変更後**:
- PlayerLocomotion を有効化
- 実行順序を明確化（Update/FixedUpdate/LateUpdate）
- 必須コンポーネントのエラーチェック追加

### 4. CameraManager.cs
**変更前**:
- `FindAnyObjectByType<PlayerScript>()` でプレイヤー検索

**変更後**:
- **Player タグで検索**（プロジェクト標準）
- フォールバックとして PlayerScript/PlayerManager も検索
- エラーハンドリング追加

## 使用方法

### 必須セットアップ
1. Player GameObjectに以下のコンポーネントをアタッチ:
   - `PlayerManager`
   - `InputManager`
   - `PlayerLocomotion`
   - `PlayerCombat`
   - `CharacterController`
   - `Animator`
   - `PlayerScript`（後方互換性のため）

2. Player GameObjectに **`Player`** タグを設定

3. Animator に以下のパラメータを追加（オプション）:
   - `Speed` (Float): 移動速度
   - `time` (Float): 移動時間

### Inspector 設定

**PlayerLocomotion**:
- Walking Speed: 歩き速度（デフォルト 2）
- Running Speed: 走り速度（デフォルト 7）
- Rotation Speed: 旋回速度（デフォルト 15）
- Gravity: 重力加速度（デフォルト 9.8）
- Push Power: 物理押し出し力（デフォルト 2）

**PlayerCombat**: 既存設定そのまま使用可能

## 削除された機能

### PlayerScript から削除:
- ✗ 移動処理 → PlayerLocomotion に移行
- ✗ Input System の直接使用 → InputManager に移行
- ✗ 重力処理 → PlayerLocomotion に移行
- ✗ アニメーション制御 → PlayerLocomotion に移行
- ✗ 戦闘処理呼び出し → PlayerManager に移行

すべての機能は適切なクラスに移行済みです。

## 今後の推奨事項

1. **PlayerScript.cs の完全削除**
   - CameraManager が完全に Player タグに移行できたら、`PlayerScript.cs` は削除可能です
   - 現在は後方互換性のために残しています

2. **AnimatorManager との統合確認**
   - `PlayerLocomotion` と `AnimatorManager` の連携が正しく動作するか確認してください

3. **CharacterController の設定**
   - Inspector で CharacterController の Radius、Height、Center を調整してください

4. **ダメージ処理の実装**
   - 旧 PlayerScript にあった `OnTriggerEnter` のダメージ処理を PlayerHealth などの専用クラスに移行することを推奨します

## テスト手順

1. Start.unity を開く
2. Player に必要なコンポーネントが全て揃っているか確認
3. Player タグが設定されているか確認
4. Play モードで以下を確認:
   - WASD で移動できるか
   - カメラが追従するか
   - アニメーションが再生されるか
   - 攻撃（Tail/Beam/Charge）が動作するか
   - 敵に対するダメージが正常か

## 注意事項

- **CharacterController を使用**: Rigidbody ではありません
- **Player タグ必須**: 敵やタワーが Player を検索するため
- **NavMesh 必要**: 敵の AI が動作するため、シーンに NavMesh を Bake してください

---

整理完了: 2025年11月14日
アーキテクチャ: PlayerManager パターン（プロジェクト標準）
