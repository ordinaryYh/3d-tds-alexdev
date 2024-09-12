public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}


[System.Serializable] //在编辑器可以查看这个类
public class Weapon
{
    public WeaponType weaponType;

    public int bulletsInMagazine;
    public int magazineCapacity;
    public int totleReserveAmmo;

    public bool CanShoot()
    {
        return HaveEnougeBullets();
    }

    private bool HaveEnougeBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
            return true;
        }

        return false;
    }

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
}
