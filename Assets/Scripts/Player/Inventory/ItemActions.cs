using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActions : MonoBehaviour
{
    public void Equip()
    {
        print("Equip");
    }

    public void Use()
    {
        print("Use");
    }

    public void Drop()
    {
        Transform inventoryItemToDrop = GameManager.Instance.player.inventoryIconsHandler.transform.GetChild(GameManager.Instance.player.inventoryIconsHandler.transform.childCount - 1);
        GameObject itemDropped = inventoryItemToDrop.GetComponent<InventoryItem>().associatedItem;
        itemDropped.transform.position = GameManager.Instance.player.playerCamera.transform.position + GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropDistance;
        itemDropped.SetActive(true);
        Vector3 forceToAdd = GameManager.Instance.player.playerCamera.transform.forward * GameManager.Instance.player.dropForce;
        itemDropped.GetComponent<Rigidbody>().AddForce(forceToAdd, ForceMode.Impulse);
        GameManager.Instance.player.playerInventory.RemoveItem(inventoryItemToDrop.gameObject);
    }
}
