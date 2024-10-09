using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{

    private PlayerControls control;
    private float moveInput;
    private float steerInput;

    private bool isBraking;

    private void Start()
    {
        control = ControlsManager.instance.controls;

        ControlsManager.instance.SwitchToCarControls();

        AssignInputEvents();
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
