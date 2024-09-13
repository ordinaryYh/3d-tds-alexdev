using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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

    public void BulletSetup(float _flyDistance)
    {
        bulletDisable = false;
        cd.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = _flyDistance + 0.5f;
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
            ObjectPool.instance.ReturnBullet(gameObject);

    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        //使用对象池进行管理
        ObjectPool.instance.ReturnBullet(gameObject);
    }

    private void CreateImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            //下面的代码是只在第一次接触的点，触发fx特效
            ContactPoint contact = collision.contacts[0];
            GameObject newImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));

            Destroy(newImpactFX, 1);
        }
    }
}
