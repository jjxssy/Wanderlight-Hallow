using UnityEngine;

public enum ItemType
{
    Default,
    Consumable,
    Equipment
}

public enum EquipmentType
{
    None,
    Helmet,
    Chestplate,
    Leggings,
    Boots,
    Weapon,
    Shield
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Information")]
    [SerializeField] private string itemName = "New Item";
    [SerializeField][TextArea(3, 5)] private string description = "Item Description";
    [SerializeField] private Sprite icon = null;

    [Header("Item Type")]
    [SerializeField] private ItemType itemType = ItemType.Default;
    [SerializeField] private EquipmentType equipmentType = EquipmentType.None;

    public string GetItemName() => itemName;
    public string GetDescription() => description;

    public Sprite GetIcon() => icon;
    public ItemType GetItemType() => itemType;
    public EquipmentType GetEquipmentType() => equipmentType;
    public virtual void Use()
    {
        switch (itemType)
        {
            case ItemType.Consumable:
                Debug.Log($"Used consumable: {itemName}");
                // Add logic for health potions, mana, etc.
                break;
            case ItemType.Equipment:
                Debug.Log($"Equipped: {itemName}");
                // This would typically call an EquipmentManager to handle equipping it.
                break;
            default:
                Debug.Log($"Cannot use item of type Default: {itemName}");
                break;
        }
    }
}