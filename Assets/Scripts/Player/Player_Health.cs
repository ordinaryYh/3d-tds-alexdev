using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : HealthController
{
    private Player player;

    public bool isDead { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
    }

    public override void ReduceHealth(int _damage)
    {
        base.ReduceHealth(_damage);

        if (ShouldDie())
            Die();

        UI.instance.inGameUI.UpdateHealthUI(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead)
            return;

        Debug.Log("player is killed " + Time.time);
        isDead = true;
        player.ragdoll.RagdollActive(true);
        player.anim.enabled = false;

        GameManager.instance.GameOver();
    }
}
