using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;

    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;
    private BoxCollider cd;


    [SerializeField] private GameObject bulletImpactFX;



    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisable;

    private void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void BulletSetup(float _flyDistance, float _impactForce)
    {
        bulletDisable = false;
        cd.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = _flyDistance + 0.5f;
        this.impactForce = _impactForce;
    }

    private void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }

        //如果子弹距离过远，那么就移回到对象池
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && bulletDisable == false)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisable = true;
        }

        if (trailRenderer.time < 0)
            ObjectPool.instance.ReturnToPool(gameObject);

    }


    //这个和OnTriggerEnter的区别是
    //和OnTriggerEnter是冲突的，要求两个物体都不能勾选is Trigger，否则不会触发
    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        //使用对象池进行管理
        ObjectPool.instance.ReturnToPool(gameObject);

        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        EnemyShield shield = collision.gameObject.GetComponent<EnemyShield>();

        if (shield != null)
        {
            shield.ReduceDurability();
            return;
        }

        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.GetHit();
            enemy.HitImapct(force, collision.contacts[0].point, hitRigidbody);
        }

    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            //下面的代码是只在第一次接触的点，触发fx特效
            ContactPoint contact = collision.contacts[0];

            GameObject newImpactFX = ObjectPool.instance.GetObject(bulletImpactFX);
            newImpactFX.transform.position = contact.point;

            ObjectPool.instance.ReturnToPoolDelay(5, newImpactFX);
        }
    }
}
