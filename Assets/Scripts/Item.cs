using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    Player player;
    [SerializeField] GameObject itemIcon;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    public override void interactAction()
    {
        player.playerInventory.addItem(itemIcon, gameObject);
    }
}
