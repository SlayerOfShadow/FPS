using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [SerializeField] GameObject inventoryItem;

    public override void InteractAction()
    {
        GameManager.Instance.player.playerInventory.AddItem(inventoryItem, gameObject);
    }
}