using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Item testItem;
    public InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager.AssignInventoryItem(0, testItem); // First inventory slot
        inventoryManager.AssignItemToSlot(0, testItem);    // First quickslot
    }
}
