using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActions : MonoBehaviour
{
    public void EquipPrimaryWeapon()
    {
        Transform inventoryItemToEquip = GameManager.Instance.player.inventoryIconsHandler.transform.GetChild(GameManager.Instance.player.inventoryIconsHandler.transform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToEquip.GetComponent<InventoryItem>();
        GameObject itemEquipped = inventoryItem.associatedItem;
        inventoryItem.inventoryActions[0] = false;
        inventoryItem.inventoryActions[1] = true;
        GameManager.Instance.player.playerEquipment.primaryWeapon = itemEquipped;
    }

    public void Unequip()
    {
        Transform inventoryItemToUnequip = GameManager.Instance.player.inventoryIconsHandler.transform.GetChild(GameManager.Instance.player.inventoryIconsHandler.transform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToUnequip.GetComponent<InventoryItem>();
        GameObject itemUnequipped = inventoryItem.associatedItem;
        inventoryItem.inventoryActions[0] = true;
        inventoryItem.inventoryActions[1] = false;
        GameManager.Instance.player.playerEquipment.primaryWeapon = null;
    }

    public void Drop()
    {
        Transform inventoryItemToDrop = GameManager.Instance.player.inventoryIconsHandler.transform.GetChild(GameManager.Instance.player.inventoryIconsHandler.transform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToDrop.GetComponent<InventoryItem>();
        GameObject itemDropped = inventoryItem.associatedItem;
        if (GameManager.Instance.player.playerEquipment.primaryWeapon == itemDropped)
        {
            inventoryItem.inventoryActions[0] = true;
            inventoryItem.inventoryActions[1] = false;
            GameManager.Instance.player.playerEquipment.primaryWeapon = null;
        }

        itemDropped.transform.position = GameManager.Instance.player.playerCamera.transform.position + GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropDistance;
        itemDropped.transform.rotation = GameManager.Instance.player.playerCamera.transform.rotation * Quaternion.Euler(0, 90, 0);
        itemDropped.SetActive(true);
        Vector3 forceToAdd = GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropForce;
        itemDropped.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
        GameManager.Instance.player.playerInventory.RemoveItem(inventoryItemToDrop.gameObject);
    }
}
