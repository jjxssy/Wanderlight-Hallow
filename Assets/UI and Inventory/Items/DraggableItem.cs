using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image iconImage;

    public Item ItemData { get; private set; }
    public int SlotIndex { get; private set; } = -1;

    private Canvas canvas;
    private Transform originalParent;

    public void SetItem(Item item, int slotIndex = -1)
    {
        ItemData = item;
        SlotIndex = slotIndex;
        iconImage.sprite = item.Icon;
        iconImage.enabled = true;
        gameObject.name = item.ItemName;
    }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(canvas.transform);
        iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        iconImage.raycastTarget = true;
    }
}
