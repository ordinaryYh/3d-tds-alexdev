using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    private float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private TrailRenderer trailRenderer;


    [SerializeField] private GameObject bulletImpactFX;


    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisabled;

    private LayerMask allyLayerMask;

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(LayerMask _allyLayer, int _bulletDamage, float flyDistance = 100, float impactForce = 100)
    {
        this.bulletDamage = _bulletDamage;
        this.impactForce = impactForce;
        this.allyLayerMask = _allyLayer;

        bulletDisabled = false;
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear();
        trailRenderer.time = .25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + .5f; // magic number .5f is a length of tip of the laser ( Check method UpdateAimVisuals() on PlayerAim script) ;
    }

    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }
    protected void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }
    protected void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; // magic number 2 is choosen trhou testing
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (FriendlyFireEnable() == false)
        {
            //这句代码需要查阅unity资料才能完全理解
            //视频有文章，视频是14-004
            //layer值实际上在unity中是保存在二进制中
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10);
                return;
            }
        }

        CreateImpactFx();
        ReturnBulletToPool();

        IDamageble damage = collision.gameObject.GetComponent<IDamageble>();
        damage?.TakeDamage(bulletDamage);

        ApplyBulletImpactToEnemy(collision);
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;
            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody);
        }
    }

    protected void ReturnBulletToPool(float delay = 0) => ObjectPool.instance.ReturnObject(gameObject, delay);


    protected void CreateImpactFx()
    {
        GameObject newFx = Instantiate(bulletImpactFX);
        newFx.transform.position = transform.position;

        Destroy(newFx, 1);

        //GameObject newImpactFx = ObjectPool.instance.GetObject(bulletImpactFX, transform);
        //ObjectPool.instance.ReturnObject(newImpactFx, 1);
    }

    private bool FriendlyFireEnable() => GameManager.instance.friendlyFire;
}
