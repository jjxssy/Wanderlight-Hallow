using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField] private Item itemData;

    // --- NEW ---
    [Tooltip("A unique ID for this specific item instance in the world. Generate one from the context menu.")]
    [SerializeField] private string itemInstanceId;

    private SpriteRenderer spriteRenderer;

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

    public Item GetItemData() => itemData;
    public string GetSaveID() => itemInstanceId;

    // This lets you generate a new ID from the Inspector
    [ContextMenu("Generate GUID for itemInstanceId")]
    private void GenerateGuid()
    {
        itemInstanceId = System.Guid.NewGuid().ToString();
    }
}
