using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActions : MonoBehaviour
{
    Player player;

    [SerializeField] Transform inventoryIconsHandlerTransform;
    public RectTransform[] equipmentSlots;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    public void Equip(int slot)
    {
        Transform inventoryItemToEquip = inventoryIconsHandlerTransform.GetChild(inventoryIconsHandlerTransform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToEquip.GetComponent<InventoryItem>();
        GameObject itemToEquip = inventoryItem.associatedItem;
        if (inventoryItem.occupiedEquipmentSlot > -1) // In case player switches between weapons slots
        {
            inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
            player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
        }
        else
        {
            foreach (InventoryCell cell in player.playerInventory.previouslyOccupiedCells[inventoryItem.gameObject])
            {
                cell.cellState = CellState.none;
            }
            player.playerInventory.previouslyOccupiedCells[inventoryItem.gameObject].Clear();
        }
        inventoryItem.occupiedEquipmentSlot = slot;
        inventoryItem.inventoryActions[slot] = false;
        inventoryItem.inventoryActions[4] = true;
        player.playerEquipment.equipment[slot] = itemToEquip;
        inventoryItem.transform.position = equipmentSlots[slot].position + 0.5f * new Vector3(-inventoryItem.GetComponent<RectTransform>().rect.width, inventoryItem.GetComponent<RectTransform>().rect.height, 0);
    }

    public void Unequip(bool drag)
    {
        Transform inventoryItemToEquip = inventoryIconsHandlerTransform.GetChild(inventoryIconsHandlerTransform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToEquip.GetComponent<InventoryItem>();
        GameObject itemToEquip = inventoryItem.associatedItem;
        inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
        inventoryItem.inventoryActions[4] = false;
        player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
        inventoryItem.occupiedEquipmentSlot = -1;
        if (!drag) player.playerInventory.RemoveEquipment(inventoryItem);
    }

    public void Use()
    {
        print("Use");
    }

    public void Drop()
    {
        Transform inventoryItemToDrop = inventoryIconsHandlerTransform.GetChild(player.playerInventory.inventoryIconsHandler.transform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToDrop.GetComponent<InventoryItem>();
        GameObject itemDropped = inventoryItem.associatedItem;
        if (inventoryItem.occupiedEquipmentSlot > -1)
        {
            inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
            inventoryItem.inventoryActions[4] = false;
            player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
            inventoryItem.occupiedEquipmentSlot = -1;
        }

        itemDropped.transform.position = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.dropDistance;
        itemDropped.transform.rotation = player.playerCamera.transform.rotation * Quaternion.Euler(0, 90, 0);
        itemDropped.SetActive(true);
        Vector3 forceToAdd = player.playerCamera.transform.forward * player.dropForce;
        itemDropped.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
        player.playerInventory.RemoveItem(inventoryItemToDrop.gameObject);
    }
}
