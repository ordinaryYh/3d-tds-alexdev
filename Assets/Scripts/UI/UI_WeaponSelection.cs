using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn;
    public UI_SelectedWeaponWindow[] selectedWeapon;

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float disapearaingSpeed = 0.25f;
    private float currentWarningAlpha;
    private float targetWarningAlpha;

    private void Start()
    {
        selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disapearaingSpeed;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn);
            UI.instance.StartLevelGeneration();
        }
        else
            ShowWarningMessage("请至少选择一把武器");
    }

    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;

    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> seletedData = new List<Weapon_Data>();

        foreach (var weapon in selectedWeapon)
        {
            if (weapon.weaponData != null)
                seletedData.Add(weapon.weaponData);
        }

        return seletedData;
    }

    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].IsEmpty() == true)
                return selectedWeapon[i];
        }

        return null;
    }

    public UI_SelectedWeaponWindow FindSlotWithWeaponOfType(Weapon_Data _weaponData)
    {
        for (int i = 0; i < selectedWeapon.Length; i++)
        {
            if (selectedWeapon[i].weaponData == _weaponData)
                return selectedWeapon[i];
        }

        return null;
    }

    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white;
        warningText.text = message;

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0;
    }

}
