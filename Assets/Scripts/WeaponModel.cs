using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrabType { SideGrab, BackGrab };
public enum HoldType { CommonHold = 1, LowHold, HighHold } //这个切换武器的动画的种类

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public GrabType grabType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;
}
