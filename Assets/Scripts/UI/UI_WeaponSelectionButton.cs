using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponSelectionButton : UI_Button
{
    public UI_WeaponSelection weaponSelectionUI;

    [SerializeField] private Weapon_Data weaponData;
    [SerializeField] private Image weaponIcon;

    private UI_SelectedWeaponWindow emptySlot;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Weapon: " + weaponData.weaponType;
    }

    public override void Start()
    {
        base.Start();

        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
        weaponIcon.sprite = weaponData.weaponIcon;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        weaponIcon.color = Color.yellow;

        emptySlot = weaponSelectionUI.FindEmptySlot();
        emptySlot?.UpdateSlotInfo(weaponData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        weaponIcon.color = Color.white;

        emptySlot?.UpdateSlotInfo(null);
        emptySlot = null;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        weaponIcon.color = Color.white;

        //这个判断的作用是如果没有空的槽位了，并且没有装备这个武器，那么就直接返回
        bool noMoreEmptySlots = weaponSelectionUI.FindEmptySlot() == null;
        bool noThisWeaponInSlots = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData) == null;
        if (noMoreEmptySlots == true && noThisWeaponInSlots == true)
        {
            weaponSelectionUI.ShowWarningMessage("没有更多的武器槽位了");
            return;
        }

        UI_SelectedWeaponWindow slotBusyWithThisWeapon = weaponSelectionUI.FindSlotWithWeaponOfType(weaponData);
        //这个判断的作用是，如果已经装备了一把武器，那么再点击一次就会取消
        if (slotBusyWithThisWeapon != null)
        {
            slotBusyWithThisWeapon.SetWeaponSlot(null);
        }
        else
        {
            emptySlot = weaponSelectionUI.FindEmptySlot();
            emptySlot.SetWeaponSlot(weaponData);
        }


        emptySlot = null;
    }
}
