using UnityEngine;
using UnityEngine.AI;

public class Boss_Movement : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    float speed=10.0f;
    private NavMeshAgent agent;
    float flightHeight=20.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false; 
        agent.updateUpAxis = false;   
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);

        Vector3 nextPos = agent.nextPosition;

        Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }
}
