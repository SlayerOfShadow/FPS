using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    Player player;
    PlayerInventory player_inventory;
    [SerializeField] GameObject item_icon;

    void Start()
    {
        player = GameManager.Instance.player;
        player_inventory = player.GetComponent<PlayerInventory>();
    }

    public override void interact_action()
    {
        player_inventory.add_item(item_icon, gameObject);
    }
}
