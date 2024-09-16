using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Interactable : MonoBehaviour
{
    protected MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;

    private void Start()
    {
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();

        defaultMaterial = mesh.sharedMaterial;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            HighlightActive(true);
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newMesh)
    {
        mesh = newMesh;
        defaultMaterial = newMesh.sharedMaterial;
    }

    public virtual void Interaction()
    {
        Debug.Log("Interacted with+ " + gameObject.name);
    }

    public void HighlightActive(bool _active)
    {
        if (_active)
            mesh.material = highlightMaterial;
        else
            mesh.material = defaultMaterial;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;


        playerInteraction.GetInteracbles().Add(this);
        playerInteraction.UpdateClosestInteractble();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();

        if (playerInteraction == null)
            return;


        playerInteraction.GetInteracbles().Remove(this);
        playerInteraction.UpdateClosestInteractble();
    }
}
