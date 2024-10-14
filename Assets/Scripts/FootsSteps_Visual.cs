using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootsSteps_Visual : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private TrailRenderer LeftFoot;
    [SerializeField] private TrailRenderer rightFoot;

    [Range(0.001f, 0.3f)]
    [SerializeField] private float checkRadius = 0.05f;
    [Range(-0.15f, 0.15f)]
    [SerializeField] private float rayDistance = -0.05f;


    private void Update()
    {
        CheckFootStep(LeftFoot);
        CheckFootStep(rightFoot);
    }

    private void CheckFootStep(TrailRenderer foot)
    {
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        bool touchingGround = Physics.CheckSphere(checkPosition, checkRadius, whatIsGround);

        foot.emitting = touchingGround;
    }

    private void OnDrawGizmos()
    {
        DrawFootGimoz(LeftFoot.transform);
        DrawFootGimoz(rightFoot.transform);
    }

    private void DrawFootGimoz(Transform foot)
    {
        if (foot == null)
            return;

        Gizmos.color = Color.blue;
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        Gizmos.DrawWireSphere(checkPosition, checkRadius);
    }
}
