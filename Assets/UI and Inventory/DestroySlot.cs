using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Special inventory slot that acts as a trash bin.
/// When an item is dropped here, it is permanently destroyed.
/// </summary>
public class DestroySlot : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// Called automatically by Unity when something is dropped on this slot.
    /// If the dropped item is valid, it is removed from the inventory.
    /// </summary>
    /// <param name="eventData">Pointer event data provided by Unity.</param>
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot sourceSlot = InventorySlot.DraggedSlot;

        if (sourceSlot != null && sourceSlot.HeldItem != null)
        {
            Debug.Log($"Destroyed item: {sourceSlot.HeldItem.GetItemName()}");

            InventoryManager.Instance.DestroyItem(sourceSlot.SlotIndex);
        }
    }
}
