using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles dropping a draggable item into a quickslot.
/// Supports moving from inventory or swapping quickslots.
/// </summary>
public class QuickslotDrop : MonoBehaviour, IDropHandler
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

        Debug.Log($"Drop: {dragged.GetItemData().GetItemName()} from {dragged.GetSlotIndex()} → quickslot {slotIndex}");

        if (dragged.IsFromInventory())
        {
            inventoryManager.AssignItemToSlot(slotIndex, dragged.GetItemData());
            inventoryManager.ClearInventorySlot(dragged.GetSlotIndex());
        }
        else if (dragged.IsFromQuickslot() && dragged.GetSlotIndex() != slotIndex)
        {
            inventoryManager.SwapQuickslots(dragged.GetSlotIndex(), slotIndex);
        }
    }
}
