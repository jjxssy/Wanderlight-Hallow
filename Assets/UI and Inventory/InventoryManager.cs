using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI Panels")]
    [Tooltip("The parent object containing the 5 quickslot UI elements.")]
    [SerializeField] private Transform quickslotPanel;

    [Tooltip("The parent object containing the 30 main inventory UI elements.")]
    [SerializeField] private Transform mainInventoryPanel;

    [Header("Keybinds (Optional)")]
    [SerializeField] private KeyBindingsManager keyBindingsManager;

    // The single, unified list of all inventory slots.
    private List<InventorySlot> slots = new List<InventorySlot>();
    private const int QuickslotSize = 5;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        InitializeSlots();
    }

    private void Update()
    {
        HandleQuickslotInput();
    }

    private void InitializeSlots()
    {
        slots.Clear();

        // Important: Get quickslots first so they have indices 0-4
        if (quickslotPanel != null)
        {
            slots.AddRange(quickslotPanel.GetComponentsInChildren<InventorySlot>());
        }

        if (mainInventoryPanel != null)
        {
            slots.AddRange(mainInventoryPanel.GetComponentsInChildren<InventorySlot>());
        }

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Initialize(i);
        }

        Debug.Log($"Inventory initialized with {slots.Count} total slots.");
    }

    private void HandleQuickslotInput()
    {
        for (int i = 0; i < QuickslotSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Alpha1, Alpha2, etc.
            {
                UseItem(i);
            }
        }
    }

    public bool AddItem(Item itemToAdd)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.HeldItem == null)
            {
                slot.SetItem(itemToAdd);
                return true;
            }
        }

        Debug.LogWarning($"Inventory is full! Could not add {itemToAdd.GetItemName()}.");
        return false;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        Item itemInSlot = slots[slotIndex].HeldItem;
        if (itemInSlot != null)
        {
            itemInSlot.Use();

            if (itemInSlot.GetItemType() == ItemType.Consumable)
            {
                slots[slotIndex].ClearSlot();
            }
        }
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= slots.Count || indexB < 0 || indexB >= slots.Count) return;

        Item itemA = slots[indexA].HeldItem;
        Item itemB = slots[indexB].HeldItem;

        slots[indexA].SetItem(itemB);
        slots[indexB].SetItem(itemA);
    }
    public void DestroyItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        slots[slotIndex].ClearSlot();
    }
}
