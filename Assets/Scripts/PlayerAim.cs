using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Player player;
    private PlayerControlls controls;

    [Header("Aim Info")]
    [Range(0.5f, 1)]
    [SerializeField] private float minCameraDistance = 1.5f;
    [Range(1, 3f)]
    [SerializeField] private float maxCameraDistance = 4;
    [Range(3f, 5f)]
    [SerializeField] private float aimSensetivity = 5f;
    [Space]
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private float turnSpeed;
    private Vector2 aimInput;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        ApplyRotation();
        aim.position = Vector3.Lerp(aim.position, TargetAimPosition(), aimSensetivity * Time.deltaTime);
    }

    private void AssignInputEvents()
    {
        controls = player.controls;

        controls.Character.Aim.performed += context => aimInput = context.ReadValue<Vector2>();
        controls.Character.Aim.canceled += context => aimInput = Vector2.zero;
    }

    private void ApplyRotation()
    {
        Vector3 lookingDirection = this.GetMousePosition() - transform.position;
        lookingDirection.y = 0;
        lookingDirection.Normalize();

        //下面的函数是让玩家以自己定义的速度来进行旋转
        //lerp这个函数是让值以一定步长来增加,后面速度会越来越慢
        //这样可以让玩家旋转更平滑
        Quaternion targetRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 TargetAimPosition()
    {
        float actualMaxCameraDistance = player.movement.moveInput.y < -0.5f ? minCameraDistance : maxCameraDistance;

        Vector3 targetAimPosition = GetMousePosition();
        Vector3 aimDirection = (targetAimPosition - transform.position).normalized;

        float distanceToTargetPosition = Vector3.Distance(transform.position, targetAimPosition);
        float clampedDistance = Mathf.Clamp(distanceToTargetPosition, minCameraDistance, actualMaxCameraDistance);

        targetAimPosition = transform.position + aimDirection * clampedDistance;
        targetAimPosition.y = transform.position.y + 1;

        return targetAimPosition;
    }

    public Vector3 GetMousePosition()
    {
        //这个函数的作用，就是返回一个坐标
        //要想实现玩家朝向物体，就要用这种方式，从camera发出射线，然后射中物体，返回物体坐标
        //camera实际上有一个视野是一个四棱锥，称为视锥体
        //在屏幕上点击的位置虽然是个二维坐标，实际上就是这个视椎体的底部上面的区域
        Ray ray = Camera.main.ScreenPointToRay(aimInput);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }
}
