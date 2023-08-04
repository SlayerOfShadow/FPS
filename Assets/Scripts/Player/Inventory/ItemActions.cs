using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActions : MonoBehaviour
{
    Transform inventoryIconsHandlerTransform;

    void Start()
    {
        inventoryIconsHandlerTransform = GameManager.Instance.player.inventoryIconsHandler.transform;
    }

    public void Equip(int slot)
    {
        Transform inventoryItemToEquip = inventoryIconsHandlerTransform.GetChild(inventoryIconsHandlerTransform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToEquip.GetComponent<InventoryItem>();
        GameObject itemToEquip = inventoryItem.associatedItem;
        if (inventoryItem.occupiedEquipmentSlot > -1)
        {
            inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
            GameManager.Instance.player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
        }
        inventoryItem.occupiedEquipmentSlot = slot;
        inventoryItem.inventoryActions[slot] = false;
        inventoryItem.inventoryActions[4] = true;
        GameManager.Instance.player.playerEquipment.equipment[slot] = itemToEquip;
    }

    public void Unequip()
    {
        Transform inventoryItemToEquip = inventoryIconsHandlerTransform.GetChild(inventoryIconsHandlerTransform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToEquip.GetComponent<InventoryItem>();
        GameObject itemToEquip = inventoryItem.associatedItem;
        inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
        inventoryItem.inventoryActions[4] = false;
        GameManager.Instance.player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
        inventoryItem.occupiedEquipmentSlot = -1;
    }

    public void Use()
    {
        print("Use");
    }

    public void Drop()
    {
        Transform inventoryItemToDrop = inventoryIconsHandlerTransform.GetChild(GameManager.Instance.player.inventoryIconsHandler.transform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToDrop.GetComponent<InventoryItem>();
        GameObject itemDropped = inventoryItem.associatedItem;
        if (inventoryItem.occupiedEquipmentSlot > -1)
        {
            inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
            inventoryItem.inventoryActions[4] = false;
            GameManager.Instance.player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
            inventoryItem.occupiedEquipmentSlot = -1;
        }

        itemDropped.transform.position = GameManager.Instance.player.playerCamera.transform.position + GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropDistance;
        itemDropped.transform.rotation = GameManager.Instance.player.playerCamera.transform.rotation * Quaternion.Euler(0, 90, 0);
        itemDropped.SetActive(true);
        Vector3 forceToAdd = GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropForce;
        itemDropped.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
        GameManager.Instance.player.playerInventory.RemoveItem(inventoryItemToDrop.gameObject);
    }
}
