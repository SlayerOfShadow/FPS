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
                Weapon itemToEquipWeapon = itemToEquip.GetComponent<Weapon>();
                itemToEquip.transform.SetParent(weaponHolder);
                itemToEquip.transform.localPosition = itemToEquipWeapon.weaponBasePosition;
                itemToEquip.transform.localRotation = itemToEquipWeapon.weaponBaseRotation;
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
        Transform inventoryItemToUnequip = inventoryIconsHandlerTransform.GetChild(inventoryIconsHandlerTransform.childCount - 1);
        InventoryItem inventoryItem = inventoryItemToUnequip.GetComponent<InventoryItem>();
        GameObject itemToUnequip = inventoryItem.associatedItem;
        inventoryItem.inventoryActions[inventoryItem.occupiedEquipmentSlot] = true;
        inventoryItem.inventoryActions[4] = false;
        player.playerEquipment.equipment[inventoryItem.occupiedEquipmentSlot] = null;
        inventoryItem.occupiedEquipmentSlot = -1;
        if (!drag) player.playerInventory.RemoveEquipment(inventoryItem);
        itemToUnequip.transform.SetParent(map);
        if (itemToUnequip.activeSelf) player.playerEquipment.playerArms.SetActive(false);
        if (player.playerEquipment.weaponHeld == itemToUnequip.GetComponent<Weapon>()) player.playerEquipment.weaponHeld = null;
        itemToUnequip.SetActive(false);
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
            if (itemDropped.GetComponent<Weapon>() == player.playerEquipment.weaponHeld)
            {
                player.playerEquipment.weaponMovementsObject.SetActive(false);
                player.playerEquipment.playerArms.SetActive(false);
                player.playerEquipment.weaponHeld = null;
                player.playerEquipment.muzzleFlash = null;
                player.playerEquipment.audioSource = null;
            }
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
