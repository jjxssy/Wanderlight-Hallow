using UnityEngine;
using UnityEngine.EventSystems;

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
        if (dragged == null || dragged.ItemData == null) return;

        Debug.Log($"Drop: {dragged.ItemData.ItemName} from {dragged.SlotIndex} → inventory {slotIndex}");

        if (dragged.IsFromInventory() && dragged.SlotIndex != slotIndex)
        {
            inventoryManager.SwapInventoryItems(dragged.SlotIndex, slotIndex);
        }
        else
        {
            inventoryManager.AssignInventoryItem(slotIndex, dragged.ItemData);

            if (dragged.IsFromQuickslot())
            {
                inventoryManager.ClearQuickslot(dragged.SlotIndex);
            }
        }
    }
}
