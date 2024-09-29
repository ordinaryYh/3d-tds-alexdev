using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy;

    private float damageCooldown;
    private float lastTimeDamage;
    private int flameDamage;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        damageCooldown = enemy.flameDamageCooldown;
        flameDamage = enemy.flameDamage;
    }

    private void OnTriggerStay(Collider other)
    {
        if (enemy.flamethrowActive == false)
            return;

        if (Time.time - lastTimeDamage < damageCooldown)
            return;

        IDamageble damage = other.GetComponent<IDamageble>();

        if (damage != null)
        {
            damage.TakeDamage(flameDamage);
            lastTimeDamage = Time.time;
            damageCooldown = enemy.flameDamageCooldown;
        }
    }
}
