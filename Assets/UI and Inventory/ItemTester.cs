using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Item testItem;
    public Item testItem2;

    void Start()
    {
        InventoryManager.instance.AddItem(testItem);
        InventoryManager.instance.AddItem(testItem2);
    }
}
