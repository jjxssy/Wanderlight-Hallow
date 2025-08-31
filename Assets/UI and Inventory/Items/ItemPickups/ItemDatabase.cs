using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems;

    public Item GetItemByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        return allItems.Find(item => item.GetItemName() == name);
    }
}
