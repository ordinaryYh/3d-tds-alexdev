using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoBoxType { smallBox, bigBox }

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(0, 100)] public int minAmount;
    [Range(0, 100)] public int maxAmount;
}

public class Pickup_Ammo : Interactable
{
    [SerializeField] private AmmoBoxType boxType;



    [SerializeField] private List<AmmoData> smallBoxAmmo;
    [SerializeField] private List<AmmoData> bigBoxAmmo;

    [SerializeField] private GameObject[] boxModel;

    private void Start()
    {
        SetupBoxModel();
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModel.Length; i++)
        {
            boxModel[i].SetActive(false);

            if (i == ((int)boxType))
            {
                boxModel[i].SetActive(true);
                UpdateMeshAndMaterial(boxModel[i].GetComponent<MeshRenderer>());
            }
        }
    }

    public override void Interaction()
    {
        List<AmmoData> currentAmmoList = smallBoxAmmo;

        if (boxType == AmmoBoxType.bigBox)
            currentAmmoList = bigBoxAmmo;

        foreach (var ammo in currentAmmoList)
        {
            Weapon weapon = weaponController?.WeaponInSlots(ammo.weaponType);

            AddBulletsToWeapon(weapon, GetBulletsAmount(ammo));
        }
    }

    private int GetBulletsAmount(AmmoData ammoData)
    {
        float min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        float max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);

        int randomAmount = Random.Range(((int)min), ((int)max));
        return randomAmount;
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon == null)
            return;

        weapon.totleReserveAmmo += amount;
        ObjectPool.instance.ReturnToPool(gameObject);
    }
}
