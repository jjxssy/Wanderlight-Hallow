using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private int slotIndex;

    private void Awake()
    {
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (dragged == null || dragged.ItemData == null) return;

        // Swap if dragging between inventory slots
        if (dragged.SlotIndex >= 0 && dragged.SlotIndex < inventoryManager.InventorySlotCount)
        {
            if (dragged.SlotIndex != slotIndex)
                inventoryManager.SwapInventoryItems(dragged.SlotIndex, slotIndex);
        }
        else
        {
            // If dragging from quickslot or other source
            inventoryManager.AssignInventoryItem(slotIndex, dragged.ItemData);
        }
    }
}
