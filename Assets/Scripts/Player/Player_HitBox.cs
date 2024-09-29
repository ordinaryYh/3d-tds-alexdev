using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HitBox : HitBox
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponentInParent<Player>();
    }

    public override void TakeDamage()
    {
        base.TakeDamage();

        player.health.ReduceHealth();
    }
}
