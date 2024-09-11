using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;

    private Rigidbody rb => GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        CreateImpactFX(collision);
        Destroy(gameObject);
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
