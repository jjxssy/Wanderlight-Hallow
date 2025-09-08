using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central inventory controller:
/// - Initializes quickslots + main inventory slots
/// - Adds/uses/swaps/destroys items
/// - Saves/loads item names to/from an array
/// </summary>
public sealed class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// Singleton accessor (read-only).
    /// </summary>
    public static InventoryManager Instance { get; private set; }

    [Header("UI Panels")]
    [Tooltip("The parent object containing the 5 quickslot UI elements.")]
    [SerializeField] private Transform quickslotPanel;

    [Tooltip("The parent object containing the 30 main inventory UI elements.")]
    [SerializeField] private Transform mainInventoryPanel;

    [Header("Keybinds (Optional)")]
    [SerializeField] private KeyBindingsManager keyBindingsManager;

    [Header("Database")]
    [SerializeField] private ItemDatabase itemDatabase;

    /// <summary>
    /// Unified list of all inventory slots (quickslots first).
    /// </summary>
    private List<InventorySlot> Slots { get; } = new();

    /// <summary>
    /// Number of quickslots at the start of the slots list.
    /// </summary>
    private const int QuickslotSize = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeSlots();
    }

    private void Update()
    {
        HandleQuickslotInput();
    }

    /// <summary>
    /// Finds all slot components under the configured panels and assigns indices.
    /// Quickslots are indexed 0..QuickslotSize-1.
    /// </summary>
    private void InitializeSlots()
    {
        Slots.Clear();

        if (quickslotPanel != null)
        {
            Slots.AddRange(quickslotPanel.GetComponentsInChildren<InventorySlot>());
        }

        if (mainInventoryPanel != null)
        {
            Slots.AddRange(mainInventoryPanel.GetComponentsInChildren<InventorySlot>());
        }

        for (int i = 0; i < Slots.Count; i++)
        {
            Slots[i].Initialize(i);
        }

        Debug.Log($"Inventory initialized with {Slots.Count} total slots.");
    }

    /// <summary>
    /// Checks number keys (Alpha1..Alpha5 by default) to use quickslot items.
    /// </summary>
    private void HandleQuickslotInput()
    {
        for (int i = 0; i < QuickslotSize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseItem(i);
            }
        }
    }

    public bool AddItem(Item itemToAdd)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.HeldItem == null)
            {
                slot.SetItem(itemToAdd);
                return true;
            }
        }

        Debug.LogWarning($"Inventory is full! Could not add {(itemToAdd != null ? itemToAdd.GetItemName() : "NULL")}.");
        return false;
    }

    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count) return;

        Item itemInSlot = Slots[slotIndex].HeldItem;
        if (itemInSlot != null)
        {
            itemInSlot.Use();

            if (itemInSlot.GetItemType() == ItemType.Consumable)
            {
                Slots[slotIndex].ClearSlot();
            }
        }
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= Slots.Count || indexB < 0 || indexB >= Slots.Count) return;

        Item itemA = Slots[indexA].HeldItem;
        Item itemB = Slots[indexB].HeldItem;

        Slots[indexA].SetItem(itemB);
        Slots[indexB].SetItem(itemA);
    }

    public void SwapItemWithSlot(int slotIndex, Item itemToPlace)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count) return;
        Slots[slotIndex].SetItem(itemToPlace);
    }

    public void DestroyItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= Slots.Count) return;

        Slots[slotIndex].ClearSlot();
    }

    public string[] GetInventoryDataForSave()
    {
        string[] itemNames = new string[Slots.Count];
        for (int i = 0; i < Slots.Count; i++)
        {
            itemNames[i] = Slots[i].HeldItem != null ? Slots[i].HeldItem.GetItemName() : null;
        }
        return itemNames;
    }

    public void LoadInventoryData(string[] itemNames)
    {
        if (itemNames == null || itemNames.Length != Slots.Count)
        {
            Debug.LogError("Failed to load inventory: Mismatched data size.");
            return;
        }

        for (int i = 0; i < Slots.Count; i++)
        {
            Item item = itemNames[i] != null ? itemDatabase.GetItemByName(itemNames[i]) : null;
            Slots[i].SetItem(item);
        }
    }
}
