using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamageble
{
    private Car_Controller car_controller;

    public int maxHealth;
    public int currentHealth;

    private bool carBroken;

    private void Start()
    {
        currentHealth = maxHealth;
        car_controller = GetComponent<Car_Controller>();
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
    }

    public void TakeDamage(int _damage)
    {
        ReduceHealth(_damage);
        UpdateCarHealthUI();
    }


}
