using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// 指定量のダメージを受け取ります。
    /// </summary>
    /// <param name="amount">受け取るダメージ量。</param>
    /// <param name="hitPoint">ヒットしたワールド座標。</param>
    /// <param name="hitNormal">ヒット面の法線ベクトル。</param>
    void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal);
}
