using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_RagDoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    public void RagdollActive(bool active)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
    }

    //这个函数是死亡之后，禁用敌人身上的collider
    //这个如果想用，就在deadstate中进行使用即可
    public void ColliderActive(bool active)
    {
        foreach (Collider cd in ragdollColliders)
        {
            cd.enabled = false;
        }
    }
}
