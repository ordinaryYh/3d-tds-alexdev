using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();

        //这段代码的作用是，fire事件执行时，给定输入事件的上下文，函数为shoot
        player.controls.Character.Fire.performed += context => Shoot();
    }
    
    private void Shoot()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
