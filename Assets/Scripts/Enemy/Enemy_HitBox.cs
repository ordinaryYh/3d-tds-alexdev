using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HitBox : HitBox
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
    }

    public override void TakeDamage(int _damage)
    {
        int newDamage = Mathf.RoundToInt(_damage * damageMultiplier);
        enemy.GetHit(newDamage);
    }
}
