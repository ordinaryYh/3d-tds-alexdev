using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryState_Melee : EnemyState
{
    private Enemy_Melee enemy;

    public RecoveryState_Melee(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = enemyBase as Enemy_Melee;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceTarget(enemy.player.position);

        if (triggerCalled)
        {

            if (enemy.CanThrowAxe())
            {
                stateMachine.ChangeState(enemy.abilityState);
                return;
            }
            else if (enemy.PlayerInAttackRnage())
            {
                stateMachine.ChangeState(enemy.attackState);
                return;
            }
            else
            {
                stateMachine.ChangeState(enemy.chaseState);
                return;
            }
        }
    }

}
