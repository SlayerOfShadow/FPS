using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    public GameObject associatedItem;
    [SerializeField] string itemName;
    [TextAreaAttribute] string itemDescription;
    [SerializeField] InventoryActions[] inventoryActions;

    #region InventoryMovementsVariables
    Vector3 offset;
    public Vector3 startPosition;
    RectTransform rectTransform;
    public Transform[] cells;
    public int nbRows;
    public int nbColumns;
    PointerEventData currentEvent = new PointerEventData(EventSystem.current);
    #endregion

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void DisplayActions()
    {
        foreach (GameObject button in GameManager.Instance.player.playerInventory.actionsButtons)
        {
            button.SetActive(false);
        }
        foreach (InventoryActions action in inventoryActions)
        {
            switch (action)
            {
                case InventoryActions.equip:
                    GameManager.Instance.player.playerInventory.actionsButtons[0].SetActive(true);
                    break;
                case InventoryActions.use:
                    GameManager.Instance.player.playerInventory.actionsButtons[1].SetActive(true);
                    break;
                case InventoryActions.drop:
                    GameManager.Instance.player.playerInventory.actionsButtons[2].SetActive(true);
                    break;
            }
        }
        GameManager.Instance.player.playerInventory.itemActionsPanel.SetActive(true);
    }

    enum InventoryActions
    {
        equip,
        use,
        drop
    }

    #region InventoryMovements
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.useDragThreshold = false;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameManager.Instance.player.playerInventory.itemActionsPanel.transform.position = Input.mousePosition;
            GameManager.Instance.player.playerInventory.itemActionsPanel.SetActive(false);
            DisplayActions();
        }
        rectTransform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            currentEvent = eventData;
            startPosition = transform.position;
            offset = transform.position - Input.mousePosition;
            GameManager.Instance.player.playerInventory.itemActionsPanel.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Input.mousePosition + offset;
            GameManager.Instance.player.playerInventory.moveItem(gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.player.playerInventory.snapItem(gameObject);
        }
    }

    void OnDisable()
    {
        if (currentEvent.pointerDrag)
        {
            currentEvent.pointerDrag = null;
            foreach (InventoryCell cell in GameManager.Instance.player.playerInventory.previouslyOccupiedCells[gameObject])
            {
                cell.cellState = CellState.occupied;
            }
        }
    }
    #endregion
}