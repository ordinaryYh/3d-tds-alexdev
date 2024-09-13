using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisuals visualController;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        visualController = GetComponentInParent<PlayerWeaponVisuals>();
        weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadIsOver()
    {
        visualController.Maxmize_RigWeight();
        weaponController.CurrentWeapon().ReloadBullets();

        weaponController.SetWeaponReady(true);
    }

    public void ReturnRig()
    {
        visualController.Maxmize_RigWeight();
        visualController.Maxmize_LeftHandWeight();
    }

    public void WeaponEquipmingIsOver()
    {
        weaponController.SetWeaponReady(true);
    }

    public void SwitchOnWeaponModel() => visualController.SwithcOnCurrentWeaponModel();
}
