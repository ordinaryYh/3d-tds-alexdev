using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    //这个是子弹默认击中后的效果，之后如果速度增加，质量就会减少
    //子弹击中的效果永远不会变
    private const float REFERENCE_BULLET_SPEED = 20;

    [SerializeField] private Weapon_Data defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] private float bulletImpacForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;


    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private List<Weapon> weaponSlots; //一共只有两把武器

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        Invoke(nameof(EquipStartingWeapon), 0.1f); //在开局的时候装备武器1
    }

    private void Update()
    {
        if (isShooting)
            Shoot();
    }

    //下面是三连发模式
    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletPerShot)
                SetWeaponReady(true);
        }
    }

    #region  Slots managment - Pick\Equip\Drop\Ready

    private void EquipStartingWeapon()
    {
        weaponSlots[0] = new Weapon(defaultWeaponData);

        EquipWeapon(0);
    }

    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
            return;

        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquiAnimation();

        CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);

        Debug.Log("当前武器为: " + CurrentWeapon().weaponType.ToString());
    }

    //捡起武器总共有三种情况
    public void PickupWeapon(Weapon newWeapon)
    {

        //如果当前有这把武器，那么就加子弹
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totleReserveAmmo += newWeapon.totleReserveAmmo;
            return;
        }

        //如果没有这把武器，并且武器槽数量过多，那么就会替换当前武器
        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        //如果没有这把武器，并且武器槽有空，那么就添加这把武器
        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();
    }

    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
            return;

        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready) => weaponReady = ready;
    public bool WeaponReady() => weaponReady;

    #endregion

    private void Shoot()
    {
        if (WeaponReady() == false)
            return;

        if (currentWeapon.CanShoot() == false)
            return;

        player.weaponVisuals.PlayerFireAnimation();

        //每次按下左键才会设置为true，如果是单发模式，那么每次射击后都变为false即可
        //这样需要按一次才会射击一次
        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            return;
        }

        FireSingleBullet();

        TriggerEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;

        //使用对象池来创建子弹
        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab);

        newBullet.transform.position = GunPoint().position;
        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(currentWeapon.gunDistance, bulletImpacForce);


        Vector3 bulletDirection = currentWeapon.ApplySpread(BulletDirection());

        //这段代码的作用是控制子弹的击中效果，速度增加，那么质量就会减少
        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rb.velocity = bulletDirection * bulletSpeed;
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(WeaponType _weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponType == _weaponType)
                return weapon;
        }

        return null;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();

            if (enemy != null)
                enemy.ActivateDodgeRoll();
        }
    }

    #region  Input Event
    private void AssignInputEvents()
    {
        PlayerControlls controls = player.controls;

        //这段代码的作用是，fire事件执行时，给定输入事件的上下文，函数为shoot
        controls.Character.Fire.performed += context => isShooting = true;
        controls.Character.Fire.canceled += context => isShooting = false;

        //下面是装备武器的事件
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += context => EquipWeapon(2);
        controls.Character.EquipSlot4.performed += context => EquipWeapon(3);
        controls.Character.EquipSlot5.performed += context => EquipWeapon(4);

        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.ToogleWeaponMode.performed += context => currentWeapon.ToggleBurst();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady() == true)
            {
                Reload();
            }
        };
    }

    #endregion


}
