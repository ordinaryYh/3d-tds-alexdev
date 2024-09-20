using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public float attackIndex; //这个代表播放的动画的index
    [Range(1, 2)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class Enemy_Melee : Enemy
{
    #region  states
    public IdleState_Melee idleState { get; private set; }
    public MoveState_Melee moveState { get; private set; }
    public RecoveryState_Melee recoveryState { get; private set; }
    public ChaseState_Melee chaseState { get; private set; }
    public AttackState_Melee attackState { get; private set; }
    public DeadState_Melee deadState { get; private set; }
    public AbilityState_Melee abilityState { get; private set; }
    #endregion


    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    public Transform shieldTransform;
    public float dodgeCooldown;
    private float lastTimeDodge = -10;


    [Header("Axe Throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float aimTimer;
    public float axeThrowCooldown;
    public Transform axeStartPoint;
    private float lastTimeAxeThrown;


    [Header("Attack data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle"); //idle并不重要，最终都要使用ragdoll
        abilityState = new AbilityState_Melee(this, stateMachine, "Axe Throw");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        InitializeSpeciality();
    }


    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        stateMachine.ChangeState(recoveryState);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        moveSpeed = moveSpeed * 0.6f;
        pulledWeapon.gameObject.SetActive(false);
    }

    private void InitializeSpeciality()
    {
        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }
    }

    public override void GetHit()
    {
        base.GetHit();
        if (healthPoints <= 0)
            stateMachine.ChangeState(deadState);
    }

    public void PullWeapon()
    {
        hiddenWeapon.gameObject.SetActive(false);
        pulledWeapon.gameObject.SetActive(true);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }

    public bool PlayerInAttackRnage() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    public void ActivateDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge)
        {
            return;
        }

        if (stateMachine.currentState != chaseState)
        {
            return;
        }

        if (Vector3.Distance(transform.position, player.position) < 2f)
        {
            return;
        }

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge roll");

        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            anim.SetTrigger("Dodge");
            lastTimeDodge = Time.time;
        }

    }

    public bool CanThrowAxe()
    {
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > lastTimeAxeThrown + axeThrowCooldown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }

        return false;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0;
    }
}
