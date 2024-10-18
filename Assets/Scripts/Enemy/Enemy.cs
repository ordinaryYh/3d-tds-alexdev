using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    public LayerMask whatIsAlly;
    public LayerMask whatIsPlayer;
    [Space]

    [Header("Idle data")]
    public float idleTime;
    public float aggresionRange;

    [Header("Move data")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 3;
    public float turnSpeed;
    private bool manualMovement;
    private bool manualRotation;

    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] patrolPointsPosition;
    private int currentPatrolIndex;

    public bool inBattleMode { get; private set; }
    protected bool isMeleeAttackReady;

    public Transform player { get; private set; }
    public Animator anim { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemyStateMachine stateMachine { get; private set; }
    public Enemy_Visuals visuals { get; private set; }

    public Ragdoll ragdoll { get; private set; }

    public Enemy_Health health { get; private set; }

    public Enemy_DropController dropController { get; private set; }


    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();

        ragdoll = GetComponent<Ragdoll>();
        visuals = GetComponent<Enemy_Visuals>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameManager.instance.player.transform;

        health = GetComponent<Enemy_Health>();

        dropController = GetComponent<Enemy_DropController>();
    }

    protected virtual void Start()
    {
        InitializePatrolPoints();
    }



    protected virtual void Update()
    {
        if (ShouldEnterBattleMode())
            EnterBattleMode();
    }

    public virtual void MeleeAttackCheck(Transform[] damagePoints, float attackCheckRadius, GameObject fx, int _damage)
    {
        if (isMeleeAttackReady == false)
            return;

        foreach (var attackPoint in damagePoints)
        {
            Collider[] detectedHits =
                Physics.OverlapSphere(attackPoint.position, attackCheckRadius, whatIsPlayer);

            for (int i = 0; i < detectedHits.Length; i++)
            {
                IDamageble damage = detectedHits[i].GetComponent<IDamageble>();

                if (damage != null)
                {
                    //如果找到了一个damage就直接返回，和手雷脚本的做法有一点不同
                    damage.TakeDamage(_damage);
                    isMeleeAttackReady = false;
                    GameObject newAttackFx = ObjectPool.instance.GetObject(fx, attackPoint);
                    ObjectPool.instance.ReturnObject(newAttackFx, 1);
                    return;
                }
            }
        }
    }

    public void EnableMeleeAttack(bool enable) => isMeleeAttackReady = enable;

    //这里需要子类进行重写melee和range都需要重写这个函数
    protected virtual void InitializePerk()
    {

    }

    //这个函数是给enemy升级用的
    public virtual void MakeEnemyVIP()
    {
        int additionalHealth = Mathf.RoundToInt(health.currentHealth * 1.5f);

        health.currentHealth += additionalHealth;

        transform.localScale = transform.localScale * 1.2f;
    }

    protected bool ShouldEnterBattleMode()
    {
        if (IsPlayerInAgrresionRange() && !inBattleMode)
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

    public virtual void GetHit(int _damage)
    {
        EnterBattleMode();
        health.ReduceHealth(_damage);

        if (health.ShouldDie())
        {
            Die();
        }
    }

    public virtual void Die()
    {
        dropController.DropItems();

        anim.enabled = false;
        agent.isStopped = true;
        agent.enabled = false;
        ragdoll.RagdollActive(true);

        MissionObject_HuntTarget huntTarget = GetComponent<MissionObject_HuntTarget>();
        huntTarget?.InvokeOnTargetKilled();
    }

    public virtual void BulletImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        if (health.ShouldDie())
            StartCoroutine(BulletImpactCoroutine(force, hitPoint, rb));
    }
    private IEnumerator BulletImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public void FaceTarget(Vector3 target, float turnSpeed = 0)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngels = transform.rotation.eulerAngles;

        if (turnSpeed == 0)
            turnSpeed = this.turnSpeed;

        float yRotation = Mathf.LerpAngle(currentEulerAngels.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngels.x, yRotation, currentEulerAngels.z);
    }



    #region Animation events
    public void ActivateManualMovement(bool manualMovement) => this.manualMovement = manualMovement;
    public bool ManualMovementActive() => manualMovement;

    public void ActivateManualRotation(bool manualRotation) => this.manualRotation = manualRotation;
    public bool ManualRotationActive() => manualRotation;
    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();



    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }

    #endregion

    #region Patrol logic
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentPatrolIndex];

        currentPatrolIndex++;

        if (currentPatrolIndex >= patrolPoints.Length)
            currentPatrolIndex = 0;

        return destination;
    }
    private void InitializePatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolPoints.Length];

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsPosition[i] = patrolPoints[i].position;
            patrolPoints[i].gameObject.SetActive(false);
        }
    }

    #endregion

    public bool IsPlayerInAgrresionRange() => Vector3.Distance(transform.position, player.position) < aggresionRange;
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }
}
