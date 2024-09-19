using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateMnaualMovement(true);
    public void StopManulMovement() => enemy.ActivateMnaualMovement(false);

    public void StartManualRotation() =>enemy.ActivateMnaualRotation(true);
    public void StopManualRotation() =>enemy.ActivateMnaualRotation(false);

    public void AbilityEvent()=> enemy.GetComponent<Enemy_Melee>().TriggerAbility();
}
