using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    private Enemy_Melee enemy;
    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Melee>();
    }

    public void ReduceDurability()
    {
        durability--;

        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0);  //这里启用另外一个chase的动画
            Destroy(gameObject);
        }
    }
}
