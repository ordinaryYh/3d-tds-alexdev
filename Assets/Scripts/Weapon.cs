using UnityEngine;

public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}

public enum ShootType
{
    Single,
    Auto
}


[System.Serializable] //在编辑器可以查看这个类
public class Weapon
{
    public WeaponType weaponType;

    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totleReserveAmmo;

    [Space]
    public ShootType shootType;
    [Range(1, 3)]
    public float reloadSpeed = 1;  //装弹动画的速度
    [Range(1, 3)]
    public float equipmentSpeed = 1;  //装备武器的动画速度
    [Range(2, 12)]
    public float gunDistance = 4;
    [Range(3, 8)]
    public float cameraDistance = 6;

    [Header("Burst fire")]
    public bool burstAvalible;
    public bool burstActive;
    public int burstBulletPerShot;
    public float burstFireRate;
    public float burstFireDelay = 0.1f;

    [Header("Shooting spesifics")]
    public int bulletPerShot = 3;
    public float defaultFireRate;
    public float fireRate = 1; //1秒可以射出多少子弹
    private float lastShootTime;

    [Header("Spread")]
    public float baseSpread = 1;
    private float currentSpred;
    public float maximumSpread = 3;

    public float spreadIncreaseRate = 0.15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;

    private void Awake()
    {
        currentSpred = baseSpread;
    }

    #region  Spread Methods
    //设置后坐力
    public Vector3 ApplySpread(Vector3 originalDirection)
    {
        UpdateSpread();

        float randomizedValue = Random.Range(-currentSpred, currentSpred);

        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

        return spreadRotation * originalDirection;
    }

    private void UpdateSpread()
    {
        if (Time.time > lastSpreadUpdateTime + spreadCooldown)
            currentSpred = baseSpread;
        else
            IncreaseSpread();

        lastSpreadUpdateTime = Time.time;
    }

    private void IncreaseSpread()
    {
        currentSpred = Mathf.Clamp(currentSpred + spreadIncreaseRate, baseSpread, maximumSpread);
    }

    #endregion

    #region  Burst Methods

    public bool BurstActivated()
    {
        if (weaponType == WeaponType.Shotgun)
        {
            burstFireDelay = 0;
            return true;
        }

        return burstActive;
    }

    public void ToggleBurst()
    {
        if (burstAvalible == false)
            return;

        burstActive = !burstActive;

        if (burstActive)
        {
            bulletPerShot = burstBulletPerShot;
            fireRate = burstFireRate;
        }
        else
        {
            bulletPerShot = 1;
            fireRate = defaultFireRate;
        }
    }



    #endregion


    public bool CanShoot() => HaveEnougeBullets() && ReadyToFire();

    private bool ReadyToFire()
    {
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    #region  Reload methods

    public bool CanReload()
    {
        if (bulletsInMagazine >= magazineCapacity)
            return false;

        if (totleReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void ReloadBullets()
    {
        //装弹的时候，会丢失当前的子弹
        int bulletToReload = magazineCapacity;

        if (bulletToReload > totleReserveAmmo)
            bulletToReload = totleReserveAmmo;

        totleReserveAmmo -= bulletToReload;
        bulletsInMagazine = bulletToReload;

        if (totleReserveAmmo < 0)
            totleReserveAmmo = 0;
    }

    private bool HaveEnougeBullets() => bulletsInMagazine > 0;

    #endregion


}


