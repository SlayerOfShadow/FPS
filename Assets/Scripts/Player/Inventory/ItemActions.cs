using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActions : MonoBehaviour
{
    Player player;

    [SerializeField] Transform inventoryIconsHandlerTransform;
    [SerializeField] Transform weaponHolder;
    [SerializeField] Transform map;
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
            if (slot == 0 || slot == 1)
            {
                itemToEquip.transform.SetParent(weaponHolder);
                itemToEquip.transform.localPosition = Vector3.zero;
                itemToEquip.transform.localRotation = Quaternion.identity;
                SetLayerRecursively(itemToEquip, "Weapon");
            }
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
        itemToEquip.transform.SetParent(map);
        itemToEquip.SetActive(false);
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
            if (itemDropped.layer != LayerMask.NameToLayer("Interactable")) SetLayerRecursively(itemDropped, "Interactable");
        }
        itemDropped.transform.SetParent(map);
        itemDropped.transform.position = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.dropDistance;
        itemDropped.transform.rotation = player.playerCamera.transform.rotation * Quaternion.Euler(0, 90, 0);
        player.playerInventory.SetCollidersRigidbody(itemDropped, true);
        itemDropped.SetActive(true);
        Vector3 forceToAdd = player.playerCamera.transform.forward * player.dropForce;
        itemDropped.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
        player.playerInventory.RemoveItem(inventoryItemToDrop.gameObject);
    }

    void SetLayerRecursively(GameObject obj, string layer)
    {
        if (obj == null) return;

        obj.layer = LayerMask.NameToLayer(layer);

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
