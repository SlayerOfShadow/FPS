using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    Player player;

    [Header("Item properties")]
    [SerializeField] string itemName;
    [TextAreaAttribute][SerializeField] string itemDescription;
    [Tooltip("0 = EquipPrimary | 1 = EquipSecondary | 2 = EquipArmor | 3 = EquipHelmet | 4 = Unequip | 5 = Use | 6 = Drop")]
    public bool[] inventoryActions = new bool[7];
    [HideInInspector] public GameObject associatedItem;
    [HideInInspector] public int occupiedEquipmentSlot = -1;

    [Header("Item hover")]
    public Transform[] cells;
    [HideInInspector] public Vector3 startPosition;
    public int nbRows;
    public int nbColumns;
    Vector3 offset;
    RectTransform rectTransform;
    PointerEventData currentEvent = new PointerEventData(EventSystem.current);
    
    void Start()
    {
        player = GameManager.Instance.player;
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!currentEvent.pointerDrag)
        {
            player.playerInventory.itemInfosPanelName.text = itemName;
            player.playerInventory.itemInfosPanelDescription.text = itemDescription;
            player.playerInventory.itemInfosPanel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(player.playerInventory.itemInfosPanel.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!currentEvent.pointerDrag)
        {
            player.playerInventory.itemInfosPanel.SetActive(false);
        }
    }

    void DisplayActions()
    {
        for (int i = 0; i < inventoryActions.Length; i++)
        {
            if (inventoryActions[i])
            {
                player.playerInventory.actionsButtons[i].SetActive(true);
                if (i < 4)
                {
                    player.playerInventory.actionsButtons[i].GetComponent<Button>().interactable = !player.playerEquipment.equipment[i];
                    player.playerInventory.actionsButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color
                    = player.playerEquipment.equipment[i] ? Color.gray : Color.white;
                }
            }
            else
            {
                player.playerInventory.actionsButtons[i].SetActive(false);
            }
        }
        player.playerInventory.itemActionsPanel.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.useDragThreshold = false;
        }
        else if (eventData.button == PointerEventData.InputButton.Right && !currentEvent.pointerDrag)
        {
            player.playerInventory.itemActionsPanel.transform.position = Input.mousePosition + new Vector3(-1f, 1f, 0);
            player.playerInventory.itemActionsPanel.SetActive(false);
            player.playerInventory.itemInfosPanel.SetActive(false);
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
            player.playerInventory.itemActionsPanel.SetActive(false);
            player.playerInventory.itemInfosPanel.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Input.mousePosition + offset;
            player.playerInventory.MoveItem(gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (player.playerInventory.previewCells.Count > 0)
            {
                player.playerInventory.itemInfosPanel.SetActive(true);
            }
            else
            {
                player.playerInventory.itemInfosPanel.SetActive(false);
            }
            player.playerInventory.SnapItem(gameObject);
        }
    }

    void OnDisable()
    {
        if (currentEvent.pointerDrag)
        {
            currentEvent.pointerDrag = null;
            foreach (InventoryCell cell in player.playerInventory.previouslyOccupiedCells[gameObject])
            {
                cell.cellState = CellState.occupied;
            }
        }
    }
}