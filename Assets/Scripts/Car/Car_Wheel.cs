using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxelType axelType;
    public WheelCollider cd { get; private set; }
    public TrailRenderer trail { get; private set; }
    public GameObject model;

    private float defaultSidesStiffness;

    private void Awake()
    {
        cd = GetComponent<WheelCollider>();
        trail = GetComponentInChildren<TrailRenderer>();

        trail.emitting = false;

        if (model == null)
            model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaultStiffness(float _newValue)
    {
        defaultSidesStiffness = _newValue;
        RestoreDefaultStiffness();
    }

    public void RestoreDefaultStiffness()
    {
        WheelFrictionCurve sideWarFriction = cd.sidewaysFriction;

        sideWarFriction.stiffness = defaultSidesStiffness;
        cd.sidewaysFriction = sideWarFriction;
    }
}
