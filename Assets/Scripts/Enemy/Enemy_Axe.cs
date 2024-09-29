using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;


    private Vector3 direction;
    private Transform player;
    private float flySpeed;
    private float rotationSpeed;
    private float timer = 1;

    private int damage;

    public void AxeSetup(float flySpeed, Transform player, float timer, int _damage)
    {
        rotationSpeed = 1600;

        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
        this.damage = _damage;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;


        transform.forward = rb.velocity;
    }

    //fixedupdate和update的区别在于，fixedupdate每秒运行的次数固定
    //而update每秒运行的次数可能不一致，每帧的时间都不同
    private void FixedUpdate()
    {
        //放在这里面可以让运动更加流畅
        rb.velocity = direction.normalized * flySpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageble damage = collision.gameObject.GetComponent<IDamageble>();
        damage?.TakeDamage(this.damage);

        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);
        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }

}
