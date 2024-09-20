using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected int healthPoints = 200;

    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")]
    public float moveSpeed;
    public float chaseSpeed;
    private bool manualMovement;
    private bool manualRotation;
    public float turnSpeed;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPostion;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }

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

    protected bool ShouldEnterBattleMode()
    {
        bool inAggresionRange = Vector3.Distance(transform.position, player.position) < aggresionRange;

        if (inAggresionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }

        return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImapctCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImapctCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    #region  Patrol logic
    private void InitializePatrolPoints()
    {
        patrolPointsPostion = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPostion[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPostion[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }

    #endregion

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

    #region  Animation events
    public void ActivateMnaualMovement(bool _manualMovement) => this.manualMovement = _manualMovement;
    public void ActivateMnaualRotation(bool _manualRotation) => this.manualRotation = _manualRotation;
    public bool ManualMovementActive() => this.manualMovement;
    public bool ManualRotationActive() => this.manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();


    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    #endregion
}
