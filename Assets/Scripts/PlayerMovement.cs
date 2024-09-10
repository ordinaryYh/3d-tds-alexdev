using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
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

    public Vector2 moveInput { get; private set; }
    private Vector3 movementDirection;
    private bool isRunning;



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
        AnimatorControllers();
    }

    private void AnimatorControllers()
    {
        //这里要使用向量的点积,以此来获取在自身x和z轴上的分量，判断是往左走还是右走
        //然后给动画赋值，就可以正常实现了
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        anim.SetFloat("xVelocity", xVelocity, 0.05f, Time.deltaTime);
        anim.SetFloat("zVelocity", zVelocity, 0.05f, Time.deltaTime);
        anim.SetBool("isRunning", isRunning && movementDirection.magnitude > 0);
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
