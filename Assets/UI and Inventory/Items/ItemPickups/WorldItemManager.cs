using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tracks world item instances, remembers which were picked up,
/// and re-hides them after loads so they don’t respawn.
/// </summary>
public sealed class WorldItemManager : MonoBehaviour
{
    /// <summary>Singleton (read-only).</summary>
    public static WorldItemManager Instance { get; private set; }

    private readonly List<ItemWorld> _registeredItems = new();
    private List<string> _destroyedItemIds = new();
    private bool _hasLoaded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterItem(ItemWorld item)
    {
        if (!_registeredItems.Contains(item))
        {
            _registeredItems.Add(item);

            if (_hasLoaded && _destroyedItemIds.Contains(item.GetSaveID()))
                item.gameObject.SetActive(false);
        }
    }

    public void MarkAsDestroyed(string id)
    {
        if (!_destroyedItemIds.Contains(id))
            _destroyedItemIds.Add(id);
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

    private IEnumerator ProcessLoadedItems()
    {
        yield return new WaitForEndOfFrame();

        foreach (var item in _registeredItems)
        {
            if (item != null && _destroyedItemIds.Contains(item.GetSaveID()))
                item.gameObject.SetActive(false);
        }
    }
}
