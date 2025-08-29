using UnityEngine;

// Attach this script to a prefab that represents an item on the ground.
[RequireComponent(typeof(SpriteRenderer))]
public class ItemWorld : MonoBehaviour
{
    // The actual item data this world object represents.
    [SerializeField] private Item itemData;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Call this after instantiating the prefab to set which item it is.
    public void SetItem(Item item)
    {
        this.itemData = item;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = item.GetIcon();
        }
    }

    public Item GetItem()
    {
        return itemData;
    }
}
