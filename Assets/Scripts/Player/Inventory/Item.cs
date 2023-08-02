using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [SerializeField] GameObject inventoryItem;

    public override void interactAction()
    {
        GameManager.Instance.player.playerInventory.addItem(inventoryItem, gameObject);
    }
}