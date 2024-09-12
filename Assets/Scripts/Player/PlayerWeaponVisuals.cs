using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;



public class PlayerWeaponVisuals : MonoBehaviour
{
    private Player player;
    private Animator anim;
    private bool isGrabbingWeapon;

    [SerializeField] private WeaponModel[] weaponModels;


    [Header("Rig")]
    [SerializeField] private float rigWeightIncreaseRate;
    private bool shouldIncrease_RigWeight;

    [Header("Left hand IK")]
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIK_Traget;
    private bool shouldIncrease_LeftHandIKWeight;
    private Rig rig;



    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rig = GetComponentInChildren<Rig>();
        //输入true，代表即使被关闭也能获取到，否则获取不到
        weaponModels = GetComponentsInChildren<WeaponModel>(true);
        player = GetComponent<Player>();
    }

    private void Update()
    {
        CheckWeaponSwitch();
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public WeaponModel CurrentWeaponModel()
    {
        WeaponModel weaponModel = null;

        WeaponType weaponType = player.weapon.CurrentWeapon().weaponType;

        for (int i = 0; i < weaponModels.Length; i++)
        {
            if (weaponModels[i].weaponType == weaponType)
                weaponModel = weaponModels[i];
        }

        return weaponModel;
    }

    public void PlayReloadAnimation()
    {
        if (isGrabbingWeapon)
            return;

        //这里要停止rig，不然的话装弹的时候，手部、枪、头部都不会有动作
        //因为rig上设置了这些部位的动作
        ReduceRigWeight();
        anim.SetTrigger("Reload");
    }

    private void UpdateLeftHandIKWeight()
    {
        if (shouldIncrease_LeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;

            if (leftHandIK.weight >= 1)
                shouldIncrease_LeftHandIKWeight = false;
        }
    }

    private void UpdateRigWeight()
    {
        if (shouldIncrease_RigWeight)
        {
            rig.weight += rigWeightIncreaseRate * Time.deltaTime;

            if (rig.weight >= 1)
                shouldIncrease_RigWeight = false;
        }
    }

    private void ReduceRigWeight()
    {
        rig.weight = 0.15f;
    }

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        leftHandIK.weight = 0;
        ReduceRigWeight();
        anim.SetFloat("WeaponGrabType", (float)grabType);
        anim.SetTrigger("WeaponGrab");

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool _busy)
    {
        isGrabbingWeapon = _busy;
        anim.SetBool("BusyGrabbingWeapon", isGrabbingWeapon);
    }
    public void Maxmize_RigWeight() => shouldIncrease_RigWeight = true;
    public void Maxmize_LeftHandWeight() => shouldIncrease_LeftHandIKWeight = true;



    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOn();
            SwitchAnimationLayer(1);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOn();
            SwitchAnimationLayer(2);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOn();
            SwitchAnimationLayer(3);
            PlayWeaponGrabAnimation(GrabType.BackGrab);
        }
    }

    private void SwitchOn()
    {
        SwitchOffWeaponModels();

        CurrentWeaponModel().gameObject.SetActive(true);

        AttachLeftHand();
    }

    private void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = CurrentWeaponModel().holdPoint;

        leftHandIK_Traget.localPosition = targetTransform.localPosition;
        leftHandIK_Traget.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(layerIndex, 1);
    }
}
