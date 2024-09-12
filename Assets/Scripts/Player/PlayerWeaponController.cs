using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;
    //这个是子弹默认击中后的效果，之后如果速度增加，质量就会减少
    //子弹击中的效果永远不会变
    private const float REFERENCE_BULLET_SPEED = 20;


    [SerializeField] private Weapon currentWeapon;


    [Header("Bullet details")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;


    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]
    [SerializeField] private int maxSlots = 2;
    [SerializeField] private List<Weapon> weaponSlots;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon.bulletsInMagazine = currentWeapon.totleReserveAmmo;
    }

    #region  Slots managment - Pick\Equip\Drop

    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
    }

    public void PickupWeapon(Weapon _newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("No slots avaliable");
            return;
        }

        weaponSlots.Add(_newWeapon);
    }

    private void DropWeapon()
    {
        if (weaponSlots.Count <= 1)
            return;

        weaponSlots.Remove(currentWeapon);

        currentWeapon = weaponSlots[0];
    }

    #endregion

    private void Shoot()
    {
        if (currentWeapon.CanShoot() == false)
            return;

        GameObject newBullet =
            Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        //这段代码的作用是控制子弹的击中效果，速度增加，那么质量就会减少
        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rb.velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;

        //下面两句代码是为了保证弹道准确，不会因为角色的旋转而发生偏差
        // weaponHolder.LookAt(aim);
        // gunPoint.LookAt(aim);

        return direction;
    }

    public Weapon CurrentWeapon() => currentWeapon;

    public Transform GunPoint() => gunPoint;

    #region  Input Event
    private void AssignInputEvents()
    {
        PlayerControlls controls = player.controls;

        //这段代码的作用是，fire事件执行时，给定输入事件的上下文，函数为shoot
        controls.Character.Fire.performed += context => Shoot();

        //下面是装备武器的事件
        controls.Character.EquipSlot1.performed += context => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += context => EquipWeapon(1);
        controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.Character.Reload.performed += context =>
        {
            if (currentWeapon.CanReload())
            {
                player.weaponVisuals.PlayReloadAnimation();
            }
        };
    }
    #endregion

}
