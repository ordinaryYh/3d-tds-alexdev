using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControlls controls;
    private CharacterController characterController;
    private Animator anim;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private float speed;
    private float verticalVelocity;
    private Vector3 movementDirection;
    private bool isRunning;

    [Header("Aim Infor")]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    private Vector3 lookingDirection;


    private Vector2 moveInput;
    private Vector2 aimInput;


    private void Start()
    {
        player = GetComponent<Player>();

        characterController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        ApplyMovement();
        AimTowardsMouse();
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        //这里要使用向量的点积
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        anim.SetFloat("xVelocity", xVelocity, 0.05f, Time.deltaTime);
        anim.SetFloat("zVelocity", zVelocity, 0.05f, Time.deltaTime);
        anim.SetBool("isRunning", isRunning && movementDirection.magnitude > 0);
    }

    private void AimTowardsMouse()
    {
        //这个函数的作用，就是让玩家移至朝向鼠标的位置
        //camera实际上有一个视野是一个四棱锥，称为视锥体
        //在屏幕上点击的位置虽然是个二维坐标，实际上就是这个视椎体的底部上面的区域
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            lookingDirection = hitInfo.point - transform.position;
            lookingDirection.y = 0;
            lookingDirection.Normalize();

            transform.forward = lookingDirection;

            aim.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
        }

    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();

        //如果存在输入
        if (movementDirection.magnitude > 0)
        {
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded == false)
        {
            verticalVelocity -= 9.81f * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
            verticalVelocity = -0.5f;
    }


    private void AssignInputEvents()
    {
        controls = player.controls;

        //下面的所有代码都是新的inputSystem的内容
        //下面的代码作用就是movement执行和取消时，对事件的上下文赋值
        //当按下wasd时，给与vector2的值，松开时，就是vector2.zero
        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context => moveInput = Vector2.zero;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;


        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };
        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };

    }


}
