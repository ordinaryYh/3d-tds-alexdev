using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMultiplier = 1;
    private Rigidbody rb;
    private float timer;
    private float impactPower;

    private LayerMask allyLayerMask;
    private bool canExplode = true;

    private void Awake() => rb = GetComponent<Rigidbody>();

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && canExplode == true)
            Explode();
    }

    private void Explode()
    {
        canExplode = false;

        PlayerExplosionFX();

        //这里要使用hash表，来避免重复元素
        //如果不使用，那么手雷爆炸时，player身上有多少个collider就会伤害多少次
        //这样的话，手雷的伤害过高，所以用hash表来避免这种情况。最终手雷只会伤害一次
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            if (IsTargetValid(hit) == false)
                continue;

            //root代表的是最上面的那个父节点，祖宗节点
            GameObject rootEntity = hit.transform.root.gameObject;
            //如果已经在hash表中了，那么就不再进行伤害
            if (uniqueEntities.Add(rootEntity) == false)
                continue;

            ApllyDamageTo(hit);
            ApllyPhysicalForceTo(hit);
        }
    }

    private void ApllyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
    }

    private static void ApllyDamageTo(Collider hit)
    {
        IDamageble damage = hit.GetComponent<IDamageble>();
        damage?.TakeDamage();
    }

    private void PlayerExplosionFX()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform);
        ObjectPool.instance.ReturnObject(newFx, 1);
        ObjectPool.instance.ReturnObject(gameObject);
    }

    public void SetupGrenade(LayerMask _allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower)
    {
        canExplode = true;

        this.allyLayerMask = _allyLayerMask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;
        this.impactPower = impactPower;
    }

    private bool IsTargetValid(Collider collider)
    {
        if (GameManager.instance.friendlyFire)
            return true;

        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
        {
            return false;
        }

        return true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;

        //  d=1/2 * g * t的平方 其中t是时间，g是重力加速度
        float velocityY =
            (direction.y - Physics.gravity.y * Mathf.Pow(timeToTarget, 2) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
