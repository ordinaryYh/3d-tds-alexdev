using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float turnSpeed;
    public float aggresionRange;

    [Header("Attack data")]
    public float attackRange;
    public float attackMoveSpeed;

    [Header("Idle data")]
    public float idleTime;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;

    [SerializeField] private Transform[] patrolPoints;
    private int currentPatrolIndex;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }

    protected virtual void Update()
    {

    }

    private void InitializePatrolPoints()
    {
        foreach (var item in patrolPoints)
        {
            item.parent = null;
        }
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPoints[currentPatrolIndex].transform.position;

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public bool PlayerInAggresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;
    public bool PlayerInAttackRnage() => Vector3.Distance(transform.position, player.position) < attackRange;

    public void ActivateMnaualMovement(bool _manualMovement) => this.manualMovement = _manualMovement;
    public bool ManualMovementActive() => this.manualMovement;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();
}
