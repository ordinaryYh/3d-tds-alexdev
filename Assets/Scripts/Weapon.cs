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
    #region  Regular mode variables
    public WeaponType weaponType;
    public ShootType shootType;
    public int bulletPerShot { get; private set; }

    private float defaultFireRate;
    public float fireRate; //1秒可以射出多少子弹
    private float lastShootTime;
    #endregion

    #region  Burst  mode variable
    private bool burstAvalible;
    public bool burstActive;
    private int burstBulletPerShot;
    private float burstFireRate;
    public float burstFireDelay { get; private set; }
    #endregion

    [Header("Magazine details")]
    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totleReserveAmmo;

    #region  Weapon generic info variables
    public float reloadSpeed { get; private set; }  //装弹动画的速度
    public float equipmentSpeed { get; private set; }  //装备武器的动画速度
    public float gunDistance { get; private set; }
    public float cameraDistance { get; private set; }
    #endregion

    #region  Weapon spread variables
    [Header("Spread")]
    private float baseSpread = 1;
    private float currentSpred;
    private float maxSpread = 3;
    private float spreadIncreaseRate = 0.15f;

    private float lastSpreadUpdateTime;
    private float spreadCooldown = 1;
    #endregion

    public Weapon(Weapon_Data weapon_Data)
    {
        bulletsInMagazine = weapon_Data.bulletsInMagazine;
        magazineCapacity = weapon_Data.magazineCapacity;
        totleReserveAmmo = weapon_Data.totleReserveAmmo;

        fireRate = weapon_Data.fireRate;
        weaponType = weapon_Data.weaponType;
        shootType = weapon_Data.shootType;
        bulletPerShot = weapon_Data.bulletPerShot;

        burstAvalible = weapon_Data.burstAvalible;
        burstActive = weapon_Data.burstActive;
        burstBulletPerShot = weapon_Data.burstBulletPerShot;
        burstFireRate = weapon_Data.burstFireRate;
        burstFireDelay = weapon_Data.burstFireDelay;

        baseSpread = weapon_Data.baseSpread;
        maxSpread = weapon_Data.maxSpread;

        spreadIncreaseRate = weapon_Data.spreadIncreaseRate;

        reloadSpeed = weapon_Data.reloadSpeed;
        equipmentSpeed = weapon_Data.equipmentSpeed;
        gunDistance = weapon_Data.gunDistance;
        cameraDistance = weapon_Data.cameraDistance;

        defaultFireRate = fireRate;
    }

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
        currentSpred = Mathf.Clamp(currentSpred + spreadIncreaseRate, baseSpread, maxSpread);
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


