using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{

    private PlayerControls control;
    private Rigidbody rb;
    private float moveInput;
    private float steerInput;

    public float speed;

    public float turnSensetivity;

    [Header("Car setting")]
    [SerializeField] private Transform centerOfMass;

    [Header("Engine settings")]
    public float currentSpeed;
    [Range(7, 12)]
    public float maxSpeed;
    [Range(0.5f, 5)]
    public float acclerationSpeed;
    [Range(1500, 4500)]
    public float motorForce = 1500f;

    [Header("Brakes Settings")]
    [Range(4, 10)]
    public float brakeSensetivity = 5;
    [Range(4000, 6000)]
    public float brakePower = 5000;
    private bool isBraking;


    private Car_Wheel[] wheels;

    private void Start()
    {
        control = ControlsManager.instance.controls;

        ControlsManager.instance.SwitchToCarControls();

        wheels = GetComponentsInChildren<Car_Wheel>();

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;

        AssignInputEvents();
    }

    private void Update()
    {
        speed = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        ApplySpeedLimit();
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
    }

    private void ApplyBrakes()
    {
        float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime;
        float currentBrakeTorque = isBraking ? newBrakeTorque : 0;

        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
                wheel.cd.brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyAnimationToWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            //这里用于实现轮子自身的转动
            wheel.cd.GetWorldPose(out position, out rotation);

            if (wheel.model != null)
            {
                wheel.model.transform.position = position;
                wheel.model.transform.rotation = rotation;
            }
        }
    }

    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                //下面控制车辆的转向
                float targetSteerAngle = steerInput * turnSensetivity;
                wheel.cd.steerAngle = Mathf.Lerp(wheel.cd.steerAngle, targetSteerAngle, 0.5f);
            }
        }
    }

    private void ApplyDrive()
    {
        currentSpeed = moveInput * acclerationSpeed * Time.deltaTime;

        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (wheel.axelType == AxelType.Front)
            {
                //这里控制车辆的前进
                wheel.cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private void AssignInputEvents()
    {
        control.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            moveInput = input.y;
            steerInput = input.x;
        };

        control.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        control.Car.Brake.performed += ctx => isBraking = true;
        control.Car.Brake.canceled += ctx => isBraking = false;
    }
}
