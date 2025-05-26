using UnityEngine;
using UnityEngine.EventSystems;

public class QuickslotDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private int slotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (dragged == null || dragged.ItemData == null) return;

        if (dragged.SlotIndex == -1)
        {
            // From item bank
            inventoryManager.AssignItemToSlot(slotIndex, dragged.ItemData);
        }
        else
        {
            // Rearranging slots
            inventoryManager.SwapQuickslots(dragged.SlotIndex, slotIndex);
        }
    }
}
