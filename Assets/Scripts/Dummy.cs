using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//这个是个靶子，可以进行武器的数值测试
public class Dummy : MonoBehaviour, IDamageble
{
    public int currentHealth;
    public int maxHealth = 100;

    [Header("Material")]
    public MeshRenderer mesh;
    public Material whiteMat;
    public Material redMat;
    [Space]
    public float refreshCooldown;
    private float lastTimeDamaged;

    private void Update()
    {
        if (Time.time > lastTimeDamaged + refreshCooldown)
            Refresh();
    }

    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        currentHealth = maxHealth;
        mesh.sharedMaterial = whiteMat;
    }

    public void TakeDamage(int _damage)
    {
        lastTimeDamaged = Time.time;
        currentHealth -= _damage;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        mesh.sharedMaterial = redMat;
    }

}
