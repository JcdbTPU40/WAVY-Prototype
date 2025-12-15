// SwarmNavAgent.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SwarmNavAgent : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    public float radius = 5.0f;
    public float jitter = 1f; // 到達点のランダムズレ

    void Awake() => agent = GetComponent<NavMeshAgent>();

    void Start()
    {
        if (target != null) MoveToRandomAroundTarget();
    }

    public void MoveToRandomAroundTarget()
    {
        if (target == null) return;
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        Vector3 dir = Random.insideUnitCircle.normalized;
        Vector3 dest = target.position + new Vector3(dir.x, 0, dir.y) * Random.Range(4.0f, radius);
        // small jitter
        dest += new Vector3(Random.Range(-jitter, jitter), 0, Random.Range(-jitter, jitter));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dest, out hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(dest);
        }
    }

    void Update()
    {
        if (target == null) return;
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        // 到達したら再配置する
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            MoveToRandomAroundTarget();
    }
}
