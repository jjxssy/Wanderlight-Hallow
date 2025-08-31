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
            if (InventoryManager.instance.AddItem(itemWorld.GetItemData()))
            {
                // ADD THIS LINE:
                WorldItemManager.instance.MarkAsDestroyed(itemWorld.GetSaveID());

                Destroy(other.gameObject);
            }
        }
    }
}
