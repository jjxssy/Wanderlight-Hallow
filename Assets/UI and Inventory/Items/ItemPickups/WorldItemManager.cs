using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tracks item instances placed in the world, remembers which ones were destroyed (picked up),
/// and re-hides them after loads so they don’t respawn.
/// Use <see cref="RegisterItem(ItemWorld)"/> from each <see cref="ItemWorld"/> in <c>Awake</c>,
/// call <see cref="MarkAsDestroyed(string)"/> when an item is picked up, and
/// persist/restore via <see cref="GetDestroyedItemIdsForSave"/> / <see cref="LoadDestroyedItems(List{string})"/>.
/// </summary>
public class WorldItemManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance for global access (read-only).
    /// </summary>
    public static WorldItemManager Instance { get; private set; }

    /// <summary>
    /// All <see cref="ItemWorld"/> objects registered in the scene.
    /// </summary>
    private readonly List<ItemWorld> _registeredItems = new();

    /// <summary>
    /// Unique IDs of items that have been destroyed (picked up) and should remain hidden.
    /// </summary>
    private List<string> _destroyedItemIds = new();

    /// <summary>
    /// True once saved data has been loaded via <see cref="LoadDestroyedItems(List{string})"/>.
    /// </summary>
    private bool _hasLoaded = false;

    /// <summary>
    /// Ensures a single instance of <see cref="WorldItemManager"/> exists.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Registers a world item so it can be managed (hidden if previously destroyed).
    /// Should be called by <see cref="ItemWorld"/> during its <c>Awake</c>.
    /// </summary>
    /// <param name="item">The world item to register.</param>
    public void RegisterItem(ItemWorld item)
    {
        if (!_registeredItems.Contains(item))
        {
            _registeredItems.Add(item);

            // If save data already loaded and this item was previously destroyed, hide it now.
            if (_hasLoaded && _destroyedItemIds.Contains(item.GetSaveID()))
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Marks an item (by its unique save ID) as destroyed so it won’t respawn next load.
    /// </summary>
    /// <param name="id">Unique ID of the item instance.</param>
    public void MarkAsDestroyed(string id)
    {
        if (!_destroyedItemIds.Contains(id))
        {
            _destroyedItemIds.Add(id);
        }
    }

    /// <summary>
    /// Returns a copy of all destroyed item IDs for saving.
    /// </summary>
    /// <returns>New list containing destroyed item IDs.</returns>
    public List<string> GetDestroyedItemIdsForSave()
    {
        return new List<string>(_destroyedItemIds);
    }

    /// <summary>
    /// Loads destroyed item IDs from save data and schedules syncing with registered items.
    /// </summary>
    /// <param name="ids">List of destroyed item IDs from a save (can be null).</param>
    public void LoadDestroyedItems(List<string> ids)
    {
        _destroyedItemIds = ids ?? new List<string>();
        _hasLoaded = true;

        // Defer hiding until end of frame so all ItemWorld.Awake() have run and registered.
        StartCoroutine(ProcessLoadedItems());
    }

    /// <summary>
    /// After one frame (so all registrations are complete), hides any registered items
    /// whose IDs are present in the loaded destroyed list.
    /// </summary>
    private IEnumerator ProcessLoadedItems()
    {
        yield return new WaitForEndOfFrame();

        foreach (var item in _registeredItems)
        {
            if (item != null && _destroyedItemIds.Contains(item.GetSaveID()))
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
