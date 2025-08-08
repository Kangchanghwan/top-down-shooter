using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.controllers.Character.Interaction.performed += _ => InteractWithClosest();
    }

    public void InteractWithClosest()
    {
        closestInteractable?.Interaction();
        interactables.Remove(closestInteractable);
        
        UpdateClosestInteractable();
    }
    
    public void UpdateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);
        closestInteractable = null;
        
        float closeDistance = float.MaxValue;

        foreach (var interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (closeDistance > distance)
            {
                closeDistance = distance;
                closestInteractable = interactable;
            }
        }
        closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteractables() => interactables;
}
