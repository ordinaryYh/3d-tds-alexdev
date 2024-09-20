using System.Collections;
using System.Collections.Generic;
using Cinemachine.Editor;
using UnityEngine;
using UnityEngine.AI;

public class MoveState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Vector3 destination;

    public MoveState_Melee(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = (Enemy_Melee)enemyBase;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.speed = enemy.moveSpeed;

        this.destination = enemyBase.GetPatrolDestination();
        enemyBase.agent.SetDestination(this.destination);
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log("I exit move state");
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(GetNextPathPoint());

        if (enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.05f)
            stateMachine.ChangeState(enemy.idleState);
    }

}
