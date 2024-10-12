using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Sounds : MonoBehaviour
{
    private Car_Controller car;
    [SerializeField] private AudioSource engineStart;
    [SerializeField] private AudioSource engineWork;
    [SerializeField] private AudioSource engineOff;

    private float minSpeed = 0;
    private float maxSpeed = 10;

    public float minPitch = 0.75f;
    public float maxPitch = 1.5f;

    private bool allowCarSounds;

    private void Start()
    {
        car = GetComponent<Car_Controller>();
        Invoke(nameof(AllowCarSounds), 1);
    }

    private void Update()
    {
        UpdateEngineSound();
    }

    private void UpdateEngineSound()
    {
        float currentSpeed = car.speed;
        float pitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / maxSpeed);

        engineWork.pitch = pitch;
    }

    public void ActivateCarSFX(bool activate)
    {
        if (allowCarSounds == false)
            return;

        if (activate)
        {
            engineStart.Play();
            engineWork.Play();
        }
        else
        {
            engineWork.Stop();
            engineOff.Play();
        }
    }

    private void AllowCarSounds() => allowCarSounds = true;

}
