using UnityEngine;

// Attach this script to your player character.
public class PlayerItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with has an ItemWorld component.
        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            // Try to add the item to the inventory.
            bool wasPickedUp = InventoryManager.instance.AddItem(itemWorld.GetItem());

            // If the item was successfully added, destroy the world object.
            if (wasPickedUp)
            {
                Destroy(other.gameObject);
            }
            // If the inventory is full, wasPickedUp will be false, and the item
            // will remain on the ground.
        }
    }
}
