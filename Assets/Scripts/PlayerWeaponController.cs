using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;

    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform aim;

    private void Start()
    {
        player = GetComponent<Player>();

        //这段代码的作用是，fire事件执行时，给定输入事件的上下文，函数为shoot
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject newBullet =
            Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(gunPoint.forward));

        newBullet.GetComponent<Rigidbody>().velocity = BulletDirection() * bulletSpeed;

        Destroy(newBullet, 10);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 BulletDirection()
    {
        Vector3 direction = (aim.position - gunPoint.position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;

        //下面两句代码是为了保证弹道准确，不会因为角色的旋转而发生偏差
        weaponHolder.LookAt(aim);
        gunPoint.LookAt(aim);

        return direction;
    }

    public Transform GunPoint() => gunPoint;
}
