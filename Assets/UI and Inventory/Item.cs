using UnityEngine;

public enum ItemType { Consumable, Tool, KeyItem }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public ItemType Type;
}
