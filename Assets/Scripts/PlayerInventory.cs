using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    Player player;
    ItemDrag item_dragged;
    Transform[] item_cells;
    [SerializeField] InventoryCell[] inventory_cells;
    [SerializeField] RectTransform[] inventory_cells_transform;
    [SerializeField] int inventory_columns;
    [SerializeField] int inventory_rows;
    public List<InventoryCell> preview_cells = new List<InventoryCell>();
    public Dictionary<GameObject, List<InventoryCell>> previously_occupied_cells = new Dictionary<GameObject, List<InventoryCell>>();

    void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && player.can_open_inventory)
        {
            player.inventory_open = !player.inventory_open;
            if (!player.inventory_open && item_dragged)
            {
                snap_item(item_dragged.gameObject);
            }
            player.can_interact = !player.inventory_open;
            Cursor.lockState = player.inventory_open == true ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = player.inventory_open;
            player.player_inventory.SetActive(player.inventory_open);
        }

        if (item_dragged)
        {
            foreach (InventoryCell cell in preview_cells)
            {
                cell.cell_state = state.preview;
            }
            if (preview_cells.Count != item_dragged.cells.Length)
            {
                foreach (InventoryCell cell in preview_cells)
                {
                    cell.cell_state = state.none;
                }
            }
        }
    }

    public void add_item(GameObject item, GameObject prefab)
    {
        int item_nb_column = item.GetComponent<ItemDrag>().nb_columns;
        int item_nb_rows = item.GetComponent<ItemDrag>().nb_rows;
        bool fitting = true;
        bool exit_loop = false;

        for (int i = 0; i < inventory_cells.Length; i++)
        {
            if (inventory_cells[i].cell_state != state.occupied)
            {
                for (int j = i; j < i + item_nb_column; j++)
                {
                    if (j + inventory_columns > inventory_cells.Length || j + inventory_rows > inventory_cells.Length)
                    {
                        return;
                    }
                    if (inventory_cells[j].cell_state == state.occupied || inventory_cells[j + inventory_columns].cell_state == state.occupied)
                    {
                        preview_cells.Clear();
                        break;
                    }
                    else
                    {
                        preview_cells.Add(inventory_cells[j]);
                        preview_cells.Add(inventory_cells[j + inventory_columns]);
                        if (preview_cells.Count == item_nb_column * item_nb_rows)
                        {
                            exit_loop = true;
                            break;
                        }
                    }
                }
                if (exit_loop)
                {

                    float initial_x = preview_cells[0].transform.position.x;
                    float initial_y = preview_cells[0].transform.position.y;

                    for (int k = 0; k < item_nb_rows; k++)
                    {
                        if (preview_cells[k].transform.position.x != initial_x)
                        {
                            fitting = false;
                            break;
                        }
                    }
                    if (fitting)
                    {
                        for (int l = 0; l < item_nb_column; l += item_nb_rows)
                        {
                            if (preview_cells[l].transform.position.y != initial_y)
                            {
                                fitting = false;
                                break;
                            }
                        }
                    }
                    if (fitting)
                    {
                        break;
                    }
                    else
                    {
                        preview_cells.Clear();
                        fitting = true;
                    }
                }
            }
        }
        item = Instantiate(item, player.inventory_icons_handler.transform.position, Quaternion.identity, player.inventory_icons_handler.transform);
        prefab.SetActive(false);
        previously_occupied_cells[item] = new List<InventoryCell>(preview_cells);
        snap_item(item);
    }

    public void move_item(GameObject item)
    {
        if (previously_occupied_cells.ContainsKey(item))
        {
            foreach (InventoryCell cell in previously_occupied_cells[item])
            {
                cell.cell_state = state.none;
            }
        }

        if (item_dragged == null)
        {
            item_dragged = item.GetComponent<ItemDrag>();
            item_cells = item_dragged.cells;
        }

        int index = 0;
        foreach (InventoryCell inventory_cell in inventory_cells)
        {
            foreach (Transform item_cell in item_cells)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(inventory_cells_transform[index], item_cell.transform.position))
                {
                    if (!preview_cells.Contains(inventory_cell))
                    {
                        if (inventory_cell.cell_state != state.occupied)
                        {
                            preview_cells.Add(inventory_cell);
                        }
                    }
                    break;
                }
                else
                {
                    if (preview_cells.Contains(inventory_cell))
                    {
                        preview_cells.Remove(inventory_cell);
                        inventory_cell.cell_state = state.none;
                    }
                }
            }
            index++;
        }
    }

    public void snap_item(GameObject item)
    {
        int count = 0;
        float x = 0;
        float y = 0;
        foreach (InventoryCell cell in preview_cells)
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
            Vector3 mean_position = new Vector3(x - rect.width * 0.5f, y + rect.height * 0.5f, item.transform.position.z);
            item.transform.localPosition = mean_position;
            foreach (InventoryCell cell in preview_cells)
            {
                cell.cell_state = state.occupied;
                cell.change_color();
            }
            previously_occupied_cells[item] = new List<InventoryCell>(preview_cells);
        }
        else
        {
            item.transform.position = item.GetComponent<ItemDrag>().start_position;
            foreach (InventoryCell cell in previously_occupied_cells[item])
            {
                cell.cell_state = state.occupied;
                cell.change_color();
            }
        }
        preview_cells.Clear();
        item_dragged = null;
    }
}