using UnityEngine;

/// <summary>
/// Handles picking up items from the world and adding them to the player’s inventory.
/// Attach this script to your player character (must have a 2D trigger collider).
/// </summary>
public sealed class PlayerItemPickup : MonoBehaviour
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
            if (InventoryManager.Instance.AddItem(itemWorld.GetItemData()))
            {
                // Register this pickup with the WorldItemManager so it won't respawn after saving/loading.
                WorldItemManager.Instance.MarkAsDestroyed(itemWorld.GetSaveID());

                // Remove the item from the world.
                Destroy(other.gameObject);
            }
        }
    }
}
