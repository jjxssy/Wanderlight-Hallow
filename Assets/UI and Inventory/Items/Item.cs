using UnityEngine;

/// <summary>
/// Broad categories an item can belong to.
/// </summary>
public enum ItemType
{
    Default,
    Consumable,
    Equipment
}

/// <summary>
/// Specific equipment slots supported by the game.
/// </summary>
public enum EquipmentType
{
    None,
    Helmet,
    Chestplate,
    Legs,
    Weapon,
    Accessory // For rings, amulets, etc.
}

/// <summary>
/// ScriptableObject representing a single item definition:
/// - Display info (name, description, icon)
/// - Classification (item/equipment type)
/// - Stat modifiers
/// - Basic <see cref="Use"/> behavior
/// </summary>
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

  // === Accessors ===

    /// <summary>Gets the display name.</summary>
    public string GetItemName() => itemName;

    /// <summary>Gets the description text.</summary>
    public string GetDescription() => description;

    /// <summary>Gets the icon sprite.</summary>
    public Sprite GetIcon() => icon;

    /// <summary>Gets the broad item category.</summary>
    public ItemType GetItemType() => itemType;

     /// <summary>Gets the specific equipment slot (if applicable).</summary>
    public EquipmentType GetEquipmentType() => equipmentType;

     /// <summary>Gets the defense bonus.</summary>
    public int GetDefenseModifier() => defenseModifier;
        
    /// <summary>Gets the strength bonus.</summary>
    public int GetStrengthModifier() => strengthModifier;
    
    /// <summary>Gets the health bonus.</summary>
    public int GetHealthModifier() => healthModifier;

     /// <summary>Gets the speed bonus.</summary>
    public int GetSpeedModifier() => speedModifier;


    /// <summary>
    /// Performs the item's default use behavior.
    /// For equipment, this is where an equipment manager could be invoked.
    /// For non-equipment (e.g., consumables), a simple example heal is applied.
    /// </summary>
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