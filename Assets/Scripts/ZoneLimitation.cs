using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLimitation : MonoBehaviour
{
    private ParticleSystem[] lines;
    private BoxCollider zoneCollider;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();
        ActivateWall(false);
    }

    private void ActivateWall(bool _activate)
    {
        foreach (var line in lines)
        {
            if (_activate)
            {
                line.Play();
            }
            else
            {
                line.Stop();
            }
        }

        zoneCollider.isTrigger = !_activate;
    }

    IEnumerator WallActivationCo()
    {
        ActivateWall(true);

        yield return new WaitForSeconds(1);

        ActivateWall(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WallActivationCo());
        Debug.Log("前面的区域不要进入");
    }


}
