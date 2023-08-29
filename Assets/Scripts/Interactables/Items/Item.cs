using UnityEngine;

public class Item : Interactable
{
    Player player;
    
    [SerializeField] GameObject inventoryItem;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    public override void InteractAction()
    {
        player.playerInventory.AddItem(inventoryItem, gameObject);
    }
}