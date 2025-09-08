using UnityEngine;
/// <summary>
/// Handles picking up items from the world and adding them to the player’s inventory.
/// Attach this script to your player character (must have a 2D trigger collider).
/// </summary>
public class PlayerItemPickup : MonoBehaviour
{
    /// <summary>
    /// Triggered when the player enters a trigger collider.
    /// Checks for <see cref="ItemWorld"/> and adds the item to the inventory.
    /// </summary>
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
