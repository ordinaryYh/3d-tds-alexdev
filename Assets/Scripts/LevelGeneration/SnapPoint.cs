using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SnapPoint : MonoBehaviour
{
    public SnapPointType pointType;

    private void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        MeshRenderer mesh = GetComponent<MeshRenderer>();

        if (boxCollider != null)
            boxCollider.enabled = false;

        if (mesh != null)
            mesh.enabled = false;
    }

    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
