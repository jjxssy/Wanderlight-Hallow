using UnityEngine;

/// <summary>
/// Defines the type of item.
/// </summary>
public enum ItemType
{
    Consumable, // Used once (e.g., potion)
    Tool,       // Equipable item that affects stats while equipped
    KeyItem     // Special items for quests or events
}

/// <summary>
/// Defines which player stat the item affects.
/// </summary>
public enum AffectedStat
{
    None,
    Health,
    Mana,
    Strength,
    Defense,
    Speed
}

/// <summary>
/// ScriptableObject representing an item in the inventory.
/// Supports temporary or permanent effects on specific player stats.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    // Item info
    [SerializeField] private string itemName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType type;

    // Effect properties
    [Header("Effect Settings")]
    [SerializeField] private AffectedStat statToAffect;
    [SerializeField] private int amount = 0;
    [SerializeField] private bool isTemporary = false;
    [SerializeField] private float duration = 0f;

    // ▶ Simple getters and setters

    public string GetItemName() { return itemName; }
    public void SetItemName(string value) { itemName = value; }

    public string GetDescription() { return description; }
    public void SetDescription(string value) { description = value; }

    public Sprite GetIcon() { return icon; }
    public void SetIcon(Sprite value) { icon = value; }

    public ItemType GetItemType() { return type; }
    public void SetItemType(ItemType value) { type = value; }

    public AffectedStat GetStatToAffect() { return statToAffect; }
    public void SetStatToAffect(AffectedStat value) { statToAffect = value; }

    public int GetAmount() { return amount; }
    public void SetAmount(int value) { amount = value; }

    public bool GetIsTemporary() { return isTemporary; }
    public void SetIsTemporary(bool value) { isTemporary = value; }

    public float GetDuration() { return duration; }
    public void SetDuration(float value) { duration = value; }
}
