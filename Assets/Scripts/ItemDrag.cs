using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    PlayerInventory player_inventory;
    Vector3 offset;
    public Vector3 start_position;
    RectTransform rect_transform;
    public Transform[] cells;
    public int nb_rows;
    public int nb_columns;
    PointerEventData current_event = new PointerEventData(EventSystem.current);

    void Start()
    {
        rect_transform = GetComponent<RectTransform>();
        player_inventory = GameManager.Instance.player.GetComponent<PlayerInventory>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rect_transform.SetAsLastSibling();
        eventData.useDragThreshold = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        current_event = eventData;
        start_position = transform.position;
        offset = transform.position - Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + offset;
        player_inventory.move_item(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        player_inventory.snap_item(gameObject);
    }

    void OnDisable()
    {
        if (current_event.pointerDrag)
        {
            current_event.pointerDrag = null;
            foreach (InventoryCell cell in player_inventory.previously_occupied_cells[gameObject])
            {
                cell.cell_state = state.occupied;
            }
        }
    }
}
