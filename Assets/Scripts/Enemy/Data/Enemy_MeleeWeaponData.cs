using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Melee Weapon Data")]
public class Enemy_MeleeWeaponData : ScriptableObject
{
    public List<MeleeAttackData> attackDatas;
    public float turnspeed = 10;
}
