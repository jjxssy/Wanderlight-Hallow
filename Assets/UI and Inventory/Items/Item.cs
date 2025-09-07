using UnityEngine;

public enum ItemType
{
    Default,
    Consumable,
    Equipment
}

// UPDATE: Add more specific types for our new slots.
public enum EquipmentType
{
    None,
    Helmet,
    Chestplate,
    Legs,
    Weapon,
    Accessory // For rings, amulets, etc.
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

    [Header("Item Stats")]
    [SerializeField] private int defenseModifier = 0;
    [SerializeField] private int strengthModifier = 0;
    [SerializeField] private int healthModifier = 0;
    [SerializeField] private int speedModifier = 0;

    // ... existing methods (GetItemName, GetDescription, etc.) remain the same ...
    public string GetItemName() => itemName;
    public string GetDescription() => description;
    public Sprite GetIcon() => icon;
    public ItemType GetItemType() => itemType;
    public EquipmentType GetEquipmentType() => equipmentType;
    public int GetDefenseModifier() => defenseModifier;
    public int GetStrengthModifier() => strengthModifier;
    public int GetHealthModifier() => healthModifier;
    public int GetSpeedModifier() => speedModifier;



    public virtual void Use()
    {
        // This is where you would call an EquipmentManager to handle equipping.
        if (itemType == ItemType.Equipment)
        {
            // We can add a call here later if needed.
            Debug.Log($"Equipping {itemName}");
        }
        else
        {
            FindFirstObjectByType<PlayerStats>().Heal(5);
        }
    }
}