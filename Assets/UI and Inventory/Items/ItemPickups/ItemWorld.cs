using UnityEngine;

/// <summary>
/// Represents a physical item instance that exists in the game world.
/// Each item is linked to an <see cref="Item"/> ScriptableObject for its data,
/// and is assigned a unique ID for saving/loading persistence.
/// </summary>
public class ItemWorld : MonoBehaviour
{
    /// <summary>
    /// The ScriptableObject that defines this item’s data (icon, stats, etc.).
    /// </summary>
    [SerializeField] private Item itemData;

    // --- NEW ---
    [Tooltip("A unique ID for this specific item instance in the world. Generate one from the context menu.")]
    /// <summary>
    /// A unique ID for this specific item instance in the world.
    /// Used for saving/loading. Can be generated via the context menu.
    /// </summary>
    [SerializeField] private string itemInstanceId;

    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Unity Awake: caches SpriteRenderer, assigns item icon, 
    /// and registers this item with the <see cref="WorldItemManager"/>.
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.GetIcon();
        }
        // Register this item with the manager when it's created
        WorldItemManager.instance?.RegisterItem(this);
    }
    /// <summary>
    /// Gets the ScriptableObject data for this item.
    /// </summary>
    public Item GetItemData() => itemData;

    /// <summary>
    /// Gets the unique save ID for this item instance.
    /// </summary>
    public string GetSaveID() => itemInstanceId;

    /// <summary>
    /// Generates a new unique GUID for <see cref="itemInstanceId"/>.
    /// This can be run from the Inspector context menu.
    /// </summary>
    [ContextMenu("Generate GUID for itemInstanceId")]
    private void GenerateGuid()
    {
        itemInstanceId = System.Guid.NewGuid().ToString();
    }
}
