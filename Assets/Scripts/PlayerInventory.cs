using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Player player;
    ItemDrag itemDragged;
    Transform[] itemCells;
    [SerializeField] InventoryCell[] inventoryCells;
    [SerializeField] RectTransform[] inventoryCellsTransform;
    [SerializeField] int cellSize;
    [SerializeField] int inventoryColumns;
    public List<InventoryCell> previewCells = new List<InventoryCell>();
    public Dictionary<GameObject, List<InventoryCell>> previouslyOccupiedCells = new Dictionary<GameObject, List<InventoryCell>>();

    void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && player.canOpenInventory)
        {
            player.inventoryOpen = !player.inventoryOpen;
            if (!player.inventoryOpen && itemDragged)
            {
                snapItem(itemDragged.gameObject);
            }
            player.canInteract = !player.inventoryOpen;
            Cursor.lockState = player.inventoryOpen == true ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = player.inventoryOpen;
            player.playerInventoryPanel.SetActive(player.inventoryOpen);
        }

        if (itemDragged)
        {
            foreach (InventoryCell cell in previewCells)
            {
                cell.cellState = state.preview;
            }
            if (previewCells.Count != itemDragged.cells.Length)
            {
                foreach (InventoryCell cell in previewCells)
                {
                    cell.cellState = state.none;
                }
            }
        }
    }

    public void addItem(GameObject item, GameObject prefab)
    {
        int itemNbColumn = item.GetComponent<ItemDrag>().nbColumns;
        int itemNbRows = item.GetComponent<ItemDrag>().nbRows;
        bool enoughCells = false;

        for (int i = 0; i < inventoryCells.Length; i++)
        {
            if (inventoryCells[i].cellState != state.occupied)
            {
                if (i + (itemNbColumn - 1) + ((itemNbRows - 1) * inventoryColumns) > inventoryCells.Length-1)
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
                            if (inventoryCells[i + j + k].cellState == state.occupied)
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
        item = Instantiate(item, player.inventoryIconsHandler.transform.position, Quaternion.identity, player.inventoryIconsHandler.transform);
        prefab.SetActive(false);
        previouslyOccupiedCells[item] = new List<InventoryCell>(previewCells);
        snapItem(item);
    }

    public void moveItem(GameObject item)
    {
        if (previouslyOccupiedCells.ContainsKey(item))
        {
            foreach (InventoryCell cell in previouslyOccupiedCells[item])
            {
                cell.cellState = state.none;
            }
        }

        if (itemDragged == null)
        {
            itemDragged = item.GetComponent<ItemDrag>();
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
                        if (inventoryCell.cellState != state.occupied)
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
                        inventoryCell.cellState = state.none;
                    }
                }
            }
            index++;
        }
    }

    public void snapItem(GameObject item)
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
        if (count == item.GetComponent<ItemDrag>().cells.Length)
        {
            Rect rect = item.GetComponent<RectTransform>().rect;
            x /= count;
            y /= count;
            Vector3 meanPosition = new Vector3(x - rect.width * 0.5f, y + rect.height * 0.5f, item.transform.position.z);
            item.transform.localPosition = meanPosition;
            foreach (InventoryCell cell in previewCells)
            {
                cell.cellState = state.occupied;
                cell.ChangeColor();
            }
            previouslyOccupiedCells[item] = new List<InventoryCell>(previewCells);
        }
        else
        {
            item.transform.position = item.GetComponent<ItemDrag>().startPosition;
            foreach (InventoryCell cell in previouslyOccupiedCells[item])
            {
                cell.cellState = state.occupied;
                cell.ChangeColor();
            }
        }
        previewCells.Clear();
        itemDragged = null;
    }
}