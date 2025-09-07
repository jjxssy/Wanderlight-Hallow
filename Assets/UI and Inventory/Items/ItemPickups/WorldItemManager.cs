using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class WorldItemManager : MonoBehaviour
{
    public static WorldItemManager instance;

    private List<ItemWorld> _registeredItems = new List<ItemWorld>();
    private List<string> _destroyedItemIds = new List<string>();
    private bool _hasLoaded = false;
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
            if (_hasLoaded && _destroyedItemIds.Contains(item.GetSaveID()))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void MarkAsDestroyed(string id)
    {
        if (!_destroyedItemIds.Contains(id))
        {
            _destroyedItemIds.Add(id);
        }
    }

    public List<string> GetDestroyedItemIdsForSave()
    {
        return new List<string>(_destroyedItemIds);
    }

    public void LoadDestroyedItems(List<string> ids)
    {
        _destroyedItemIds = ids ?? new List<string>();
        _hasLoaded = true; 

        StartCoroutine(ProcessLoadedItems());
    }

    // This coroutine runs one frame after the data is loaded
    private IEnumerator ProcessLoadedItems()
    {
        // Wait for the end of the current frame. By the next frame, all Awake calls are guaranteed to be finished.
        yield return new WaitForEndOfFrame();

        // Now it's safe to loop through all registered items
        foreach (var item in _registeredItems)
        {
            if (item != null && _destroyedItemIds.Contains(item.GetSaveID()))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
