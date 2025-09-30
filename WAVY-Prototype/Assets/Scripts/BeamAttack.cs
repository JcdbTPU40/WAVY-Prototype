using UnityEngine;

public class BeamAttack : MonoBehaviour
{
    [Header("Beam Settings")]
    public float damage = 50f;
    
    private void Start()
    {
        // ビームが生成されたときに敵を検知
        DetectEnemiesInBeam();
    }

    private void DetectEnemiesInBeam()
    {
        // ビームのColliderを取得
        Collider beamCollider = GetComponent<Collider>();
        
        if (beamCollider == null)
        {
            Debug.LogWarning("ビームにColliderが設定されていません！");
            return;
        }

        // すべてのColliderを検出
        Collider[] allColliders = Physics.OverlapBox(
            transform.position, 
            beamCollider.bounds.size / 2, 
            transform.rotation
        );

        int enemyCount = 0;

        // Enemyタグを持つオブジェクトをチェック
        foreach (Collider collider in allColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemyCount++;
                Debug.Log($"敵 {collider.name} をビーム攻撃で削除");
                
                // 敵オブジェクトを削除
                Destroy(collider.gameObject);
            }
        }

        Debug.Log($"ビーム攻撃: {enemyCount}体の敵を検出・削除");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemyタグをチェック
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"ビームが敵 {other.name} に当たりました");
            
            // 敵を削除
            Destroy(other.gameObject);
        }
    }

    // デバッグ用：ビームの範囲を可視化
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}