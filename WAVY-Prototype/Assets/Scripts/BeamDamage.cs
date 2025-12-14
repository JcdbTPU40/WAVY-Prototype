using UnityEngine;

public class BeamDamage : MonoBehaviour
{
  [Header("ダメージ設定")]
    [Tooltip("一度に与えるダメージ量")]
    [SerializeField] private int damageAmount = 5; 
    
    [Tooltip("ダメージを与える間隔（秒）。小さいほど、触れている間速くHPが削れる。")]
    [SerializeField] private float damageInterval = 0.5f; 
    
    private float damageTimer; 

    void Update()
    {
        damageTimer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        // プレイヤーにタグ "Player" が設定されているかを確認
        if (other.CompareTag("Player"))
        {
            // ダメージ間隔を超えたらダメージ処理を実行
            if (damageTimer >= damageInterval)
            {
                // プレイヤーのPlayerHealthコンポーネントを取得
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    // PlayerHealthのTakeDamageメソッドを呼び出す
                    playerHealth.TakeDamage(damageAmount);
                    
                    // タイマーをリセット
                    damageTimer = 0f;
                }
            }
        }
    }
}
