using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
    }

    public void ReloadIsOver()
    {
        visualController.Maxmize_RigWeight();

        //之后要添加补充子弹的功能
    }

    public void ReturnRig()
    {
        visualController.Maxmize_RigWeight();
        visualController.Maxmize_LeftHandWeight();
    }

    public void WeaponGrabIsOver()
    {
        visualController.SetBusyGrabbingWeaponTo(false);
    }
}
