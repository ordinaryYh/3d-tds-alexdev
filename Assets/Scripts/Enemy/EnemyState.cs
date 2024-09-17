using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;

    protected string animBoolName;

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        //anim.setBool(animBoolName, true);
        Debug.Log(animBoolName);
    }

    public virtual void Exit()
    {
        //anim.setBool(animBoolName, false);
    }

    public virtual void Update()
    {

    }

}
