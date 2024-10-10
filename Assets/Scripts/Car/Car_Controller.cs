using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    public bool carActive { get; private set; }
    private PlayerControls control;
    private float moveInput;
    private float steerInput;

    public float speed;

    [Range(30, 60)]
    [SerializeField] private float turnSensetivity = 30;

    [Header("Car setting")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350, 1000)]
    [SerializeField] private float carMass = 400;
    [Range(20, 80)]
    [SerializeField] private float wheelMass = 30;
    [Range(0.5f, 2)]
    [SerializeField] private float frontWheelTracktion = 1;
    [Range(0.5f, 2)]
    [SerializeField] private float backWheelTracktion = 1;

    [Header("Engine settings")]
    [SerializeField] private float currentSpeed;
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 12;
    [Range(0.5f, 10)]
    [SerializeField] private float acclerationSpeed = 2;
    [Range(1500, 6500)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Brakes Settings")]
    [Range(0, 10)]
    public float frontBrakeSensetivity = 5;
    [Range(0, 10)]
    public float backBrakeSensetivity = 5;
    [Range(4000, 6000)]
    public float brakePower = 5000;
    private bool isBraking;

    [Header("Drift settings")]
    [Range(0, 2)]
    [SerializeField] private float frontDriftFactor = 0.5f;
    [Range(0, 2)]
    [SerializeField] private float backDriftFactor = 0.5f;
    [SerializeField] private float driftDuration = 1;
    private float driftTimer;
    private bool isDrifting;


    private Car_Wheel[] wheels;

    private void Start()
    {
        wheels = GetComponentsInChildren<Car_Wheel>();
        rb = GetComponent<Rigidbody>();

        control = ControlsManager.instance.controls;
        //ControlsManager.instance.SwitchToCarControls();

        AssignInputEvents();

        SetupDefaultValues();

        ActivateCar(false);
    }

    private void SetupDefaultValues()
    {
        rb.centerOfMass = centerOfMass.localPosition;
        rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.cd.mass = wheelMass;

            if (wheel.axelType == AxelType.Front)
                wheel.SetDefaultStiffness(frontWheelTracktion);
            else if (wheel.axelType == AxelType.Back)
                wheel.SetDefaultStiffness(backWheelTracktion);
        }
    }

    private void Update()
    {
        if (carActive == false)
            return;

        speed = rb.velocity.magnitude;
        UI.instance.inGameUI.UpdateSpeedText(Mathf.RoundToInt(speed * 10) + "km/h");

        driftTimer -= Time.deltaTime;

        if (driftTimer < 0)
            isDrifting = false;
    }

    private void FixedUpdate()
    {
        if (carActive == false)
            return;

        ApplySpeedLimit();
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();

        if (isDrifting)
            ApplyDrift();
        else
            StopDrift();
    }

    private void ApplyBrakes()
    {
        foreach (var wheel in wheels)
        {
            bool frontBraks = wheel.axelType == AxelType.Front;
            float brakeSensetivity = frontBraks ? frontBrakeSensetivity : backBrakeSensetivity;

            float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0;

            wheel.cd.brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.axelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor;

            WheelFrictionCurve sidewarsFriction = wheel.cd.sidewaysFriction;

            sidewarsFriction.stiffness *= 1 - driftFactor;
            wheel.cd.sidewaysFriction = sidewarsFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffness();
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

    public void ActivateCar(bool activate)
    {
        carActive = activate;

        if (activate == false)
            rb.constraints = RigidbodyConstraints.FreezeAll;
        else
            rb.constraints = RigidbodyConstraints.None;
    }

    public void BreakTheCar()
    {
        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = 0.9f;
        backDriftFactor = 0.9f;

        //rb.constraints = RigidbodyConstraints.FreezeAll;
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
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.axelType == AxelType.Front)
                    //这里控制车辆的前进
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.axelType == AxelType.Back)
                    //这里控制车辆的前进
                    wheel.cd.motorTorque = motorTorqueValue;
            }
            else
            {
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

        control.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            isDrifting = true;
            driftTimer = driftDuration;
        };
        control.Car.Brake.canceled += ctx => isBraking = false;

        control.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfTheCar();
    }

    [ContextMenu("Focus camera and enable")]
    public void TextThisCar()
    {
        ActivateCar(true);
        CameraManager.instance.ChangeCameraTarget(transform, 12);
    }
}
