using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Quickslots")]
    [SerializeField] private List<Button> slotButtons;
    [SerializeField] private List<DraggableItem> slotIcons;
    [SerializeField] private Sprite defaultSlotSprite;
    [SerializeField] private List<Item> items = new List<Item>(5);

    [Header("Inventory")]
    [SerializeField] private List<DraggableItem> inventoryIcons;
    [SerializeField] private List<Item> inventoryItems;

    [Header("Keybinds (optional)")]
    [SerializeField] private KeyBindingsManager keyBindingsManager;

    public int QuickslotCount => slotButtons.Count;
    public int InventorySlotCount => inventoryIcons.Count;

    private void Awake()
    {
        while (items.Count < slotButtons.Count)
            items.Add(null);

        while (inventoryItems.Count < inventoryIcons.Count)
            inventoryItems.Add(null);
    }

    private void Start()
    {
        for (int i = 0; i < slotButtons.Count; i++)
        {
            int index = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(index));
            slotIcons[i].SetItem(items[i], index);
            slotIcons[i].SetInventoryManager(this);
        }

        for (int i = 0; i < inventoryIcons.Count; i++)
        {
            inventoryIcons[i].SetItem(inventoryItems[i], i);
            inventoryIcons[i].SetInventoryManager(this);
        }
    }

    private void Update()
    {
        if (keyBindingsManager == null) return;

        for (int i = 0; i < slotButtons.Count; i++)
        {
            string actionName = $"QUICKSLOT {i + 1}";
            KeyCode key = keyBindingsManager.GetKey(actionName);
            if (Input.GetKeyDown(key))
            {
                OnSlotSelected(i);
            }
        }
    }

    public void OnSlotSelected(int index)
    {
        if (index < 0 || index >= items.Count) return;

        Item item = items[index];
        if (item == null)
        {
            Debug.Log($"Slot {index + 1} is empty.");
            return;
        }

        if (item.Type == ItemType.Consumable)
        {
            Debug.Log($"Consumed {item.ItemName} from quickslot {index + 1}");
            ClearQuickslot(index);
        }
        else
        {
            Debug.Log($"Equipped {item.ItemName} from quickslot {index + 1}");
        }
    }

    public void AssignItemToSlot(int index, Item item)
    {
        if (index < 0 || index >= items.Count) return;

        items[index] = item;

        slotIcons[index].SetItem(item, index);
        slotIcons[index].SetInventoryManager(this);
    }

    public void ClearQuickslot(int index)
    {
        if (index < 0 || index >= items.Count) return;

        items[index] = null;
        slotButtons[index].image.sprite = defaultSlotSprite;
        slotIcons[index].SetItem(null, index);
    }

    public void AssignInventoryItem(int index, Item item)
    {
        if (index < 0 || index >= inventoryItems.Count) return;

        inventoryItems[index] = item;
        inventoryIcons[index].SetItem(item, index);
        inventoryIcons[index].SetInventoryManager(this);
    }

    public void SwapInventoryItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= inventoryItems.Count || indexB >= inventoryItems.Count) return;

        (inventoryItems[indexA], inventoryItems[indexB]) = (inventoryItems[indexB], inventoryItems[indexA]);

        inventoryIcons[indexA].SetItem(inventoryItems[indexA], indexA);
        inventoryIcons[indexB].SetItem(inventoryItems[indexB], indexB);

        inventoryIcons[indexA].SetInventoryManager(this);
        inventoryIcons[indexB].SetInventoryManager(this);
    }
    
    public void ClearInventorySlot(int index)
{
    if (index < 0 || index >= inventoryItems.Count) return;

    inventoryItems[index] = null;
    inventoryIcons[index].SetItem(null, index);
}

public void SwapQuickslots(int indexA, int indexB)
{
    if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
        return;

    (items[indexA], items[indexB]) = (items[indexB], items[indexA]);

    slotIcons[indexA].SetItem(items[indexA], indexA);
    slotIcons[indexB].SetItem(items[indexB], indexB);

    slotIcons[indexA].SetInventoryManager(this);
    slotIcons[indexB].SetInventoryManager(this);
}


}
