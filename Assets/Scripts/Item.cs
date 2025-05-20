using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType type;
    [SerializeField] private int amount;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public ItemType Type => type;
    public int Amount => amount;
}

public enum ItemType
{
    Consumable,
    Tool
}
