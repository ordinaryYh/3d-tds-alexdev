using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamageble
{
    private Car_Controller car_controller;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;


    [Header("Explosion Info")]
    [SerializeField] private int explosionDamage = 350;
    [Space]
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private Transform explosionPoint;
    [SerializeField] private ParticleSystem fireFx;
    [SerializeField] private ParticleSystem explosionFx;
    [Space]
    [SerializeField] private float explisionDelay = 3;
    [SerializeField] private float explosionForce = 7;
    [SerializeField] private float explosionUpwardsModifier = 2;


    private void Start()
    {
        currentHealth = maxHealth;
        car_controller = GetComponent<Car_Controller>();
    }

    private void Update()
    {
        if (fireFx.gameObject.activeSelf)
            fireFx.transform.rotation = Quaternion.identity;
    }

    public void UpdateCarHealthUI()
    {
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            BreakTheCar();
    }

    private void BreakTheCar()
    {
        carBroken = true;
        car_controller.BreakTheCar();

        fireFx.gameObject.SetActive(true);
        StartCoroutine(ExplisionCo(explisionDelay));
    }

    public void TakeDamage(int _damage)
    {
        ReduceHealth(_damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplisionCo(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        explosionFx.gameObject.SetActive(true);
        car_controller.rb.
            AddExplosionForce(explosionForce, explosionPoint.position,
            explosionRadius, explosionUpwardsModifier, ForceMode.Impulse);

        Explode();
    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius);

        foreach (var cd in colliders)
        {
            IDamageble damage = cd.gameObject.GetComponent<IDamageble>();

            if (damage != null)
            {
                GameObject root = cd.transform.root.gameObject;

                if (uniqueEntities.Add(root) == false)
                    continue;

                damage.TakeDamage(explosionDamage);

                cd.GetComponentInChildren<Rigidbody>().
                    AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsModifier, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmoz()
    {
        Gizmos.DrawWireSphere(explosionPoint.position + Vector3.forward * 1.5f, explosionRadius);
    }
}
