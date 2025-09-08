using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject database that stores all available items in the game.
/// Provides lookup functionality to find items by name.
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Header("All Items")]
    [Tooltip("List of all items available in the game.")]
    [SerializeField] private List<Item> allItems = new();

    /// <summary>
    /// Retrieves an item from the database by its name.
    /// </summary>
    /// <param name="name">The name of the item to search for.</param>
    /// <returns>
    /// The <see cref="Item"/> with the matching name, or null if no match is found.
    /// </returns>
    public Item GetItemByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        return allItems.Find(item => item.GetItemName() == name);
    }
}
