using UnityEngine;

public class interactionDetector : MonoBehaviour
{
    private IInteractable interactableInRange=null;// Closet Interactable
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // rb = GetComponent<Rigidbody2D>();
        // rb.freezeRotation = true;
    }
    public void onInteract()
    {
        interactableInRange?.interact();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E to interact
        {
            onInteract();
        }
    }

    private void OnTriggerEnter2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable.canInteract())
            interactableInRange = interactable;
        
    }
    private void OnTriggerExit2D(Collider2D collison)
    {
        if(collison.TryGetComponent(out IInteractable interactable) && interactable==interactableInRange)
            interactableInRange = null;

    }
}
