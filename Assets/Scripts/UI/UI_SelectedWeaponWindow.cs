using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponInfo;

    private void Start()
    {
        UpdateSlotInfo(null);
    }

    public void SetWeaponSlot(Weapon_Data newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(weaponData);
    }

    public void UpdateSlotInfo(Weapon_Data _weaponData)
    {
        if (_weaponData == null)
        {
            weaponIcon.color = Color.white;
            weaponInfo.text = "Select a weapon...";
            return;
        }

        weaponIcon.color = Color.white;
        weaponIcon.sprite = _weaponData.weaponIcon;
        weaponInfo.text = _weaponData.weaponInfo;
    }
    public bool IsEmpty() => weaponData == null;
}
