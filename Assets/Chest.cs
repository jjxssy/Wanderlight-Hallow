using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{

    public bool IsOpened {  get; private set; }
    // saving id for the save chest
    public string ChestID {  get; private set; }
    public GameObject itemPrefab; // item that chest contains
    public Sprite openedSprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject); // unique id
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if(!CanInteract()) return;

        OpenChest();
    }

    public void OpenChest()
    {
        SetOpened(true);

        //DropItem
        if (itemPrefab != null)
        {
            GameObject dropedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
        }

    }

    public void SetOpened(bool opened)
    {
        if (IsOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
