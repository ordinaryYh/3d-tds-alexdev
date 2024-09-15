using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> interactables;

    private Interactable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.controls.Character.Interaction.performed += context => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        closestInteractable?.Interaction();
    }

    public void UpdateClosestInteractble()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;

        float closestDistance = float.MaxValue;

        foreach (var item in interactables)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = item;
            }
        }

        closestInteractable?.HighlightActive(true);
    }
}
