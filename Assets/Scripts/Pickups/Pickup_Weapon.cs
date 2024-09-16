using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Weapon : Interactable
{
    [SerializeField] private Weapon_Data weapon_Data;
    [SerializeField] private Weapon weapon;

    //添加这个列表是为了，可以进行皮肤的选择
    //如果当前物品是pistol，那么就会选择pistol的皮肤
    [SerializeField] private BackupWeaponModel[] models;

    private bool oldWeapon = false;

    private void Start()
    {
        if (oldWeapon == false)
            weapon = new Weapon(weapon_Data);

        SetupGameObject();
    }

    public void SetupPickupWeapon(Weapon _weapon, Transform transform)
    {
        oldWeapon = true;
        this.weapon = _weapon;
        this.weapon_Data = _weapon.weaponData;
        this.transform.position = transform.position + new Vector3(0, 0.75f, 0);
    }

    public void SetupGameObject()
    {
        gameObject.name = "Pickup_Weapon - " + weapon_Data.weaponType.ToString();
        SetupWeaponModel();
    }

    [ContextMenu("Update Item Model")]
    public void SetupWeaponModel()
    {
        foreach (var model in models)
        {
            model.gameObject.SetActive(false);

            if (model.weaponType == weapon_Data.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshAndMaterial(model.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        weaponController.PickupWeapon(weapon);

        ObjectPool.instance.ReturnToPool(gameObject);
    }
}
