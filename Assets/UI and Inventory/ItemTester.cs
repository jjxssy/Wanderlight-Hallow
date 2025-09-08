using UnityEngine;

/// <summary>
/// Simple test script to automatically add items to the player's inventory
/// at the start of the scene. Used for debugging.
/// </summary>
public class ItemTester : MonoBehaviour
{
    [Header("Test Items")]
    [Tooltip("First item to add to the inventory at Start.")]
    [SerializeField] private Item testItem;

    [Tooltip("Second item to add to the inventory at Start.")]
    [SerializeField] private Item testItem2;

    /// <summary>
    /// Unity lifecycle method.
    /// Adds test items to the inventory on scene start.
    /// </summary>
    void Start()
    {
        InventoryManager.Instance.AddItem(testItem);
        InventoryManager.Instance.AddItem(testItem2);
    }
}
