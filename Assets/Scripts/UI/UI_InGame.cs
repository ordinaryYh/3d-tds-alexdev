using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthBar;

    [Header("Weapon")]
    [SerializeField] private UI_WeaponSlot[] weaponSlots_UI;

    [Header("Missions")]
    [SerializeField] private GameObject missionToolTipParent;
    [SerializeField] private GameObject missionHelpTooltip;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionDetails;
    private bool tooltipActive = true;

    private void Awake()
    {
        weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
    }

    public void SwitchMissionToolTip()
    {
        tooltipActive = !tooltipActive;
        missionToolTipParent.SetActive(tooltipActive);
        missionHelpTooltip.SetActive(!tooltipActive);
    }

    public void UpdateMissionInfo(string _missionText, string _missionDetails = "")
    {
        this.missionDetails.text = _missionDetails;
        this.missionText.text = _missionText;
    }

    public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon _currentWeapon)
    {
        for (int i = 0; i < weaponSlots_UI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                bool isActiveWeapon = weaponSlots[i] == _currentWeapon ? true : false;
                weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isActiveWeapon);
            }
            else
            {
                weaponSlots_UI[i].UpdateWeaponSlot(null, false);
            }
        }
    }

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
