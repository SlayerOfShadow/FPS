using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    Vector3 offset;
    public Vector3 startPosition;
    RectTransform rectTransform;
    public Transform[] cells;
    public int nbRows;
    public int nbColumns;
    PointerEventData currentEvent = new PointerEventData(EventSystem.current);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
        eventData.useDragThreshold = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentEvent = eventData;
        startPosition = transform.position;
        offset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + offset;
        GameManager.Instance.player.playerInventory.moveItem(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.Instance.player.playerInventory.snapItem(gameObject);
    }

    void OnDisable()
    {
        if (currentEvent.pointerDrag)
        {
            currentEvent.pointerDrag = null;
            foreach (InventoryCell cell in GameManager.Instance.player.playerInventory.previouslyOccupiedCells[gameObject])
            {
                cell.cellState = state.occupied;
            }
        }
    }
}