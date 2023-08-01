using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    [SerializeField] GameObject itemIcon;

    public override void interactAction()
    {
        GameManager.Instance.player.playerInventory.addItem(itemIcon, gameObject);
    }
}