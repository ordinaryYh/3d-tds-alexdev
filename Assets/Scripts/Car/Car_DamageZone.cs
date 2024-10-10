using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController;

    [SerializeField] private float minSpeedToDamage = 1.5f;

    [SerializeField] private int carDamage;
    [SerializeField] private float impactForce = 250;
    [SerializeField] private float upwardsMultiplier = 3;

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>();
    }

    private void ApplyForce(Rigidbody _rb)
    {
        _rb.isKinematic = false;
        _rb.AddExplosionForce(impactForce, transform.position, 3, upwardsMultiplier, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController.rb.velocity.magnitude < minSpeedToDamage)
            return;

        IDamageble damage = other.GetComponent<IDamageble>();
        if (damage == null)
            return;

        damage.TakeDamage(carDamage);

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
            ApplyForce(rb);
    }
}
