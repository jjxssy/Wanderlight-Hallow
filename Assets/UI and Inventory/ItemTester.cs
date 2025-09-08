using UnityEngine;

public class ItemTester : MonoBehaviour
{
    public Item testItem;
    public Item testItem2;

    void Start()
    {
        InventoryManager.Instance.AddItem(testItem);
        InventoryManager.Instance.AddItem(testItem2);
    }
}
