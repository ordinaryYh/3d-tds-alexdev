using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private bool isDead = false;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void ReduceHealth(int _damage)
    {
        currentHealth -= _damage;
    }

    public virtual void IncreaseHealth()
    {
        currentHealth++;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool ShouldDie()
    {
        if (isDead == true)
            return false;

        if (currentHealth <= 0)
        {
            isDead = true;
            return true;
        }

        return false;
    }


}

