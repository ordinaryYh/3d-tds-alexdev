using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponModel : MonoBehaviour
{

    public WeaponType weaponType;
    public EquipType equipAnimationType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;

    [Header("Audio Source")]
    public AudioSource fireSFX;
    public AudioSource reloadSFX;

}
