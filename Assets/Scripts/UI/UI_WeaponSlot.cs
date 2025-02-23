using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSlot : MonoBehaviour
{
    public Image weaponIcon;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        weaponIcon = GetComponentInChildren<Image>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateWeaponSlot(Weapon _myWeapon, bool _activeWeapon)
    {
        if (_myWeapon == null)
        {
            weaponIcon.color = Color.clear;
            ammoText.text = "";
            return;
        }

        Color newColor = _activeWeapon ? Color.white : new Color(1, 1, 1, 0.35f);
        weaponIcon.color = newColor;
        weaponIcon.sprite = _myWeapon.weaponData.weaponIcon;

        ammoText.text = _myWeapon.bulletsInMagazine + "/" + _myWeapon.totalReserveAmmo;
        ammoText.color = Color.white;
    }
}
