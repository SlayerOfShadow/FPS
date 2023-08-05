using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    Player player;

    [Header("Inventory properties")]
    [SerializeField] GameObject playerInventoryPanel;
    public GameObject inventoryIconsHandler;
    [SerializeField] InventoryCell[] inventoryCells;
    [SerializeField] RectTransform[] inventoryCellsTransform;
    [SerializeField] int cellSize;
    [SerializeField] int inventoryColumns;
    [HideInInspector] public List<InventoryCell> previewCells = new List<InventoryCell>();
    [HideInInspector] public Dictionary<GameObject, List<InventoryCell>> previouslyOccupiedCells = new Dictionary<GameObject, List<InventoryCell>>();
    Transform[] itemCells;
    InventoryItem itemDragged;

    [Header("Menus")]
    public GameObject itemActionsPanel;
    public GameObject[] actionsButtons;
    public GameObject itemInfosPanel;
    public TextMeshProUGUI itemInfosPanelName;
    public TextMeshProUGUI itemInfosPanelDescription;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && player.inventoryOpen)
        {
            itemActionsPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.I) && player.canOpenInventory)
        {
            player.inventoryOpen = !player.inventoryOpen;
            if (!player.inventoryOpen && itemDragged)
            {
                SnapItem(itemDragged.gameObject);
            }
            player.canInteract = !player.inventoryOpen;
            Cursor.lockState = player.inventoryOpen == true ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = player.inventoryOpen;
            playerInventoryPanel.SetActive(player.inventoryOpen);
            itemActionsPanel.SetActive(false);
            itemInfosPanel.SetActive(false);
        }

        if (itemDragged)
        {
            foreach (InventoryCell cell in previewCells)
            {
                cell.cellState = CellState.preview;
            }
            if (previewCells.Count != itemDragged.cells.Length)
            {
                foreach (InventoryCell cell in previewCells)
                {
                    cell.cellState = CellState.none;
                }
            }
        }

        if (itemInfosPanel.activeSelf)
        {
            itemInfosPanel.transform.position = Input.mousePosition;
        }
    }

    public void AddItem(GameObject item, GameObject prefab)
    {
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();
        int itemNbColumn = inventoryItem.nbColumns;
        int itemNbRows = inventoryItem.nbRows;
        bool enoughCells = false;

        for (int i = 0; i < inventoryCells.Length; i++)
        {
            if (inventoryCells[i].cellState != CellState.occupied)
            {
                if (i + (itemNbColumn - 1) + ((itemNbRows - 1) * inventoryColumns) > inventoryCells.Length - 1)
                {
                    return;
                }
                else
                {
                    bool stopSearching = false;
                    for (int j = 0; j < itemNbColumn; j++)
                    {
                        for (int k = 0; k < itemNbRows; k++)
                        {
                            k = k * inventoryColumns;
                            if (inventoryCells[i + j + k].cellState == CellState.occupied)
                            {
                                previewCells.Clear();
                                stopSearching = true;
                                break;
                            }
                            else
                            {
                                previewCells.Add(inventoryCells[i + j + k]);
                                if (previewCells.Count == itemNbColumn * itemNbRows)
                                {
                                    enoughCells = true;
                                    break;
                                }
                            }
                            k = k / inventoryColumns;
                        }
                        if (stopSearching)
                        {
                            break;
                        }
                    }
                }

                if (enoughCells)
                {
                    enoughCells = false;
                    if (previewCells[0].transform.position.x + cellSize * (itemNbColumn - 1) == previewCells[previewCells.Count - 1].transform.position.x
                    && previewCells[0].transform.position.y - cellSize * (itemNbRows - 1) == previewCells[previewCells.Count - 1].transform.position.y)
                    {
                        break;
                    }
                    else
                    {
                        previewCells.Clear();
                    }
                }
            }
        }
        item = Instantiate(item, inventoryIconsHandler.transform.position, Quaternion.identity, inventoryIconsHandler.transform);
        item.GetComponent<InventoryItem>().associatedItem = prefab;
        prefab.SetActive(false);
        previouslyOccupiedCells[item] = new List<InventoryCell>(previewCells);
        SnapItem(item);
    }

    public void MoveItem(GameObject item)
    {
        if (previouslyOccupiedCells.ContainsKey(item))
        {
            foreach (InventoryCell cell in previouslyOccupiedCells[item])
            {
                cell.cellState = CellState.none;
            }
        }

        if (itemDragged == null)
        {
            itemDragged = item.GetComponent<InventoryItem>();
            itemCells = itemDragged.cells;
        }

        int index = 0;
        foreach (InventoryCell inventoryCell in inventoryCells)
        {
            foreach (Transform itemCell in itemCells)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(inventoryCellsTransform[index], itemCell.transform.position))
                {
                    if (!previewCells.Contains(inventoryCell))
                    {
                        if (inventoryCell.cellState != CellState.occupied)
                        {
                            previewCells.Add(inventoryCell);
                        }
                    }
                    break;
                }
                else
                {
                    if (previewCells.Contains(inventoryCell))
                    {
                        previewCells.Remove(inventoryCell);
                        inventoryCell.cellState = CellState.none;
                    }
                }
            }
            index++;
        }
    }

    public void SnapItem(GameObject item)
    {
        int count = 0;
        float x = 0;
        float y = 0;
        foreach (InventoryCell cell in previewCells)
        {
            count++;
            x += cell.transform.localPosition.x;
            y += cell.transform.localPosition.y;
        }
        if (count == item.GetComponent<InventoryItem>().cells.Length)
        {
            Rect rect = item.GetComponent<RectTransform>().rect;
            x /= count;
            y /= count;
            Vector3 meanPosition = new Vector3(x - rect.width * 0.5f, y + rect.height * 0.5f, item.transform.position.z);
            item.transform.localPosition = meanPosition;
            foreach (InventoryCell cell in previewCells)
            {
                cell.cellState = CellState.occupied;
                cell.ChangeColor();
            }
            previouslyOccupiedCells[item] = new List<InventoryCell>(previewCells);
        }
        else
        {
            item.transform.position = item.GetComponent<InventoryItem>().startPosition;
            foreach (InventoryCell cell in previouslyOccupiedCells[item])
            {
                cell.cellState = CellState.occupied;
                cell.ChangeColor();
            }
        }
        previewCells.Clear();
        itemDragged = null;
    }

    public void RemoveEquipment(InventoryItem item)
    {
        int itemNbColumn = item.nbColumns;
        int itemNbRows = item.nbRows;
        bool enoughCells = false;

        for (int i = 0; i < inventoryCells.Length; i++)
        {
            if (inventoryCells[i].cellState != CellState.occupied)
            {
                if (i + (itemNbColumn - 1) + ((itemNbRows - 1) * inventoryColumns) > inventoryCells.Length - 1)
                {
                    return;
                }
                else
                {
                    bool stopSearching = false;
                    for (int j = 0; j < itemNbColumn; j++)
                    {
                        for (int k = 0; k < itemNbRows; k++)
                        {
                            k = k * inventoryColumns;
                            if (inventoryCells[i + j + k].cellState == CellState.occupied)
                            {
                                previewCells.Clear();
                                stopSearching = true;
                                break;
                            }
                            else
                            {
                                previewCells.Add(inventoryCells[i + j + k]);
                                if (previewCells.Count == itemNbColumn * itemNbRows)
                                {
                                    enoughCells = true;
                                    break;
                                }
                            }
                            k = k / inventoryColumns;
                        }
                        if (stopSearching)
                        {
                            break;
                        }
                    }
                }

                if (enoughCells)
                {
                    enoughCells = false;
                    if (previewCells[0].transform.position.x + cellSize * (itemNbColumn - 1) == previewCells[previewCells.Count - 1].transform.position.x
                    && previewCells[0].transform.position.y - cellSize * (itemNbRows - 1) == previewCells[previewCells.Count - 1].transform.position.y)
                    {
                        break;
                    }
                    else
                    {
                        previewCells.Clear();
                    }
                }
            }
        }
        previouslyOccupiedCells[item.gameObject] = new List<InventoryCell>(previewCells);
        SnapItem(item.gameObject);
    }

    public void RemoveItem(GameObject item)
    {
        foreach (InventoryCell cell in previouslyOccupiedCells[item])
        {
            cell.cellState = CellState.none;
        }
        Destroy(item);
    }
}