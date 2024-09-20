using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAxe : MonoBehaviour
{
    [SerializeField] private GameObject impactFX;
    [SerializeField] private Transform axeVisual;
    [SerializeField] private Rigidbody rb;


    private float flySpeed;
    private Transform player;
    private float rotationSpeed = 1600;
    private Vector3 direction;
    private float timer = 1;

    public void AxeSetup(float _flySpeed, Transform _player, float _timer)
    {
        this.flySpeed = _flySpeed;
        this.player = _player;
        this.timer = _timer;
    }

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position;

        rb.velocity = direction.normalized * flySpeed;

        transform.forward = rb.velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        Player player = other.GetComponent<Player>();

        if (bullet != null || player != null)
        {
            GameObject newFx = ObjectPool.instance.GetObject(impactFX);
            newFx.transform.position = transform.position;

            ObjectPool.instance.ReturnToPool(gameObject);
            ObjectPool.instance.ReturnToPoolDelay(1, newFx);
        }
    }

}
