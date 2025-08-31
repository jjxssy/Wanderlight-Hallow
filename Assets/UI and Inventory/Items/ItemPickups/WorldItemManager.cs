using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldItemManager : MonoBehaviour
{
    public static WorldItemManager instance;

    private List<ItemWorld> _registeredItems = new List<ItemWorld>();
    private List<string> _destroyedItemIds = new List<string>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterItem(ItemWorld item)
    {
        if (!_registeredItems.Contains(item))
        {
            _registeredItems.Add(item);
        }
    }

    public void MarkAsDestroyed(string id)
    {
        if (!_destroyedItemIds.Contains(id))
        {
            _destroyedItemIds.Add(id);
        }
    }

    // Called by the Save System
    public List<string> GetDestroyedItemIdsForSave()
    {
        return new List<string>(_destroyedItemIds);
    }

    // Called by the Save System
    public void LoadDestroyedItems(List<string> ids)
    {
        _destroyedItemIds = ids ?? new List<string>();

        // Find and disable all items that were picked up in the save file
        foreach (var item in _registeredItems)
        {
            if (_destroyedItemIds.Contains(item.GetSaveID()))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
