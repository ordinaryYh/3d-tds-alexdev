using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject_Key : MonoBehaviour
{
    private GameObject player;
    //这里采用订阅事件的方式来实现
    public static event Action OnKeyPickedUp;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player)
            return;

        OnKeyPickedUp?.Invoke();
        Destroy(gameObject);
    }
}
