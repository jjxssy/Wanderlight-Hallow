using UnityEngine;
/// <summary>
/// Detects nearby interactable objects and triggers their interaction
/// when the player presses the interact key (default: E).
/// Attach this script to a GameObject with a 2D trigger collider.
/// </summary>
public class interactionDetector : MonoBehaviour
{
    /// <summary>
    /// The interactable object currently within range, if any.
    /// </summary>
    private IInteractable interactableInRange=null;// Closet Interactable

    /// <summary>
    /// Calls <see cref="IInteractable.Interact"/> on the interactable in range, if present.
    /// </summary>
    public void onInteract()
    {
        interactableInRange?.Interact();
    }
    
    /// <summary>
    /// Checks for input and triggers interaction if the player presses the interact key.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E to interact
        {
            onInteract();
            
        }
    }
    /// <summary>
    /// When entering a trigger, checks if the collider has an <see cref="IInteractable"/> component.
    /// If it can be interacted with, stores it as the current interactable.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
            interactableInRange = interactable;
        
    }
    /// <summary>
    /// When exiting a trigger, clears the current interactable if it matches the one stored.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable==interactableInRange)
            interactableInRange = null;

    }
}
