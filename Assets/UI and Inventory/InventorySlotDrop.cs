using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles dropping a draggable item into an inventory slot.
/// Supports moving from quickslot to inventory and swapping inventory items.
/// </summary>
public class InventorySlotDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private int slotIndex;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (dragged == null || dragged.GetItemData() == null) return;

        Debug.Log($"Drop: {dragged.GetItemData().GetItemName()} from {dragged.GetSlotIndex()} → inventory {slotIndex}");

        if (dragged.IsFromInventory() && dragged.GetSlotIndex() != slotIndex)
        {
            // Swap within inventory
            inventoryManager.SwapInventoryItems(dragged.GetSlotIndex(), slotIndex);
        }
        else
        {
            // Assign item to inventory
            inventoryManager.AssignInventoryItem(slotIndex, dragged.GetItemData());

            // Clear quickslot if needed
            if (dragged.IsFromQuickslot())
            {
                inventoryManager.ClearQuickslot(dragged.GetSlotIndex());
            }
        }
    }
}
