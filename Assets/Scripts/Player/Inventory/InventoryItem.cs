using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler
{
    [Tooltip("0 = EquipPrimary | 1 = EquipSecondary | 2 = EquipArmor | 3 = EquipHelmet | 4 = Unequip | 5 = Use | 6 = Drop")]
    public bool[] inventoryActions = new bool[7];
    public int occupiedEquipmentSlot = -1;
    public GameObject associatedItem;
    public int nbRows;
    public int nbColumns;
    Vector3 offset;
    public Vector3 startPosition;
    RectTransform rectTransform;
    public Transform[] cells;
    PointerEventData currentEvent = new PointerEventData(EventSystem.current);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void DisplayActions()
    {
        for (int i = 0; i < inventoryActions.Length; i++)
        {
            if (inventoryActions[i])
            {
                GameManager.Instance.player.playerInventory.actionsButtons[i].SetActive(true);
                if (i == 0 || i == 1 || i == 2 || i == 3)
                {
                    GameManager.Instance.player.playerInventory.actionsButtons[i].GetComponent<Button>().interactable = !GameManager.Instance.player.playerEquipment.equipment[i];
                    GameManager.Instance.player.playerInventory.actionsButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color 
                    = GameManager.Instance.player.playerEquipment.equipment[i] ? Color.gray : Color.white;
                }
            }
            else
            {
                GameManager.Instance.player.playerInventory.actionsButtons[i].SetActive(false);
            }
        }
        GameManager.Instance.player.playerInventory.itemActionsPanel.SetActive(true);
    }

    public enum InventoryActions
    {
        equipPrimary,
        unequip,
        drop
    }

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
            GameManager.Instance.player.playerInventory.MoveItem(gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.player.playerInventory.SnapItem(gameObject);
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
}