using UnityEngine;
/// <summary>
/// Detects nearby interactable objects and triggers their interaction
/// when the player presses the interact key (default: E).
/// Attach this to a GameObject with a 2D trigger collider.
/// </summary>
public class interactionDetector : MonoBehaviour
{
    /// <summary>
    /// The interactable object currently in range, if any.
    /// </summary>
    private IInteractable interactableInRange=null;// Closet Interactable
    
    /// <summary>
    /// Calls <see cref="IInteractable.Interact"/> on the interactable in range.
    /// </summary>
    public void onInteract()
    {
        interactableInRange?.Interact();
    }

    /// <summary>
    /// Checks for input and triggers interaction if available.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E to interact
        {
            onInteract();
            
        }
    }

    /// <summary>
    /// Stores the interactable object when entering a trigger zone.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
            interactableInRange = interactable;
        
    }

    /// <summary>
    /// Clears the stored interactable if the player leaves its trigger zone.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable==interactableInRange)
            interactableInRange = null;

    }
}
