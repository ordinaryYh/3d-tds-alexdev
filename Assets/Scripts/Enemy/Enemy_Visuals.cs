using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;


public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }

public class Enemy_Visuals : MonoBehaviour
{

    [Header("Weapon visuals")]
    [SerializeField] private Enemy_WeaponModel[] weaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;
    public GameObject currentWeaponModel { get; private set; }

    [Header("Corruption visuals")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        weaponModels = GetComponentsInChildren<Enemy_WeaponModel>(true);
        CollectCorruptionCrystals();
    }

    private void Start()
    {

    }

    public void EnableWeaponTrail(bool enable)
    {
        Enemy_WeaponModel currentWeapon = currentWeaponModel.GetComponent<Enemy_WeaponModel>();
        currentWeapon.EneableTrailEffect(enable);
    }


    public void SetupWeaponType(Enemy_MeleeWeaponType type)
        => this.weaponType = type;

    public void SetupLook()
    {
        SetupRandomColor();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    //这段代码的作用是让enemy身上的水晶随机显示部分
    private void SetupRandomCorruption()
    {
        List<int> avaliableIndexs = new List<int>();

        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            avaliableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);

        }

        for (int i = 0; i < corruptionAmount; i++)
        {
            if (avaliableIndexs.Count == 0)
                break;


            int randomIndex = Random.Range(0, avaliableIndexs.Count);
            int objectIndex = avaliableIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            Debug.Log("open crystal");
            avaliableIndexs.RemoveAt(randomIndex);
        }
    }

    private void SetupRandomWeapon()
    {
        foreach (var weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<Enemy_WeaponModel> filteredWeaponModels = new List<Enemy_WeaponModel>();

        foreach (var weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == this.weaponType)
                filteredWeaponModels.Add(weaponModel);
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        currentWeaponModel = filteredWeaponModels[randomIndex].gameObject;
        currentWeaponModel.SetActive(true);

        OverrideAnimatorController();
    }

    private void OverrideAnimatorController()
    {
        AnimatorOverrideController overrideController =
                    currentWeaponModel.GetComponent<Enemy_WeaponModel>().overrideController;

        if (overrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
        }
    }

    private void SetupRandomColor()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);

        newMat.mainTexture = colorTextures[randomIndex];

        skinnedMeshRenderer.material = newMat;
    }

    private void CollectCorruptionCrystals()
    {
        Enemy_CoruptionCrystal[] crystalComponents = GetComponentsInChildren<Enemy_CoruptionCrystal>(true);
        corruptionCrystals = new GameObject[crystalComponents.Length];

        for (int i = 0; i < corruptionCrystals.Length; i++)
        {
            corruptionCrystals[i] = crystalComponents[i].gameObject;
        }
    }

}
