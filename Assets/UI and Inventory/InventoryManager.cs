using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Button> slotButtons;
    [SerializeField] private Sprite defaultSlotSprite;
    [SerializeField] private List<DraggableItem> slotIcons;
    [SerializeField] private List<Item> items = new List<Item>(5);

    private void Awake()
    {
        while (items.Count < 5)
            items.Add(null);
    }

    public void AssignItemToSlot(int index, Item item)
    {
        if (index < 0 || index >= items.Count) return;

        items[index] = item;
        slotButtons[index].image.sprite = item.Icon;
        slotIcons[index].SetItem(item, index);
    }

    public void SwapQuickslots(int indexA, int indexB)
    {
        (items[indexA], items[indexB]) = (items[indexB], items[indexA]);
        slotButtons[indexA].image.sprite = items[indexA] != null ? items[indexA].Icon : defaultSlotSprite;
        slotButtons[indexB].image.sprite = items[indexB] != null ? items[indexB].Icon : defaultSlotSprite;

        slotIcons[indexA].SetItem(items[indexA], indexA);
        slotIcons[indexB].SetItem(items[indexB], indexB);
    }
}
