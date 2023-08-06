using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    Player player;

    [SerializeField] int slotNumber;
    [SerializeField] Color baseColor;
    [SerializeField] Color hoverColor;
    Image img;
    RectTransform rectTransform;
    RectTransform itemDraggedRectTransform;

    void Start()
    {
        player = GameManager.Instance.player;
        img = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player.playerInventory.itemDragged)
        {
            if (!itemDraggedRectTransform) itemDraggedRectTransform = player.playerInventory.itemDragged.GetComponent<RectTransform>();
            Vector3 itemPos = itemDraggedRectTransform.position + 0.5f * new Vector3(itemDraggedRectTransform.rect.width, -itemDraggedRectTransform.rect.height, 0);
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, itemPos))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (player.playerInventory.itemDragged.inventoryActions[i] && i == slotNumber && !player.playerEquipment.equipment[i])
                    {
                        img.color = hoverColor;
                        break;
                    }
                    else
                    {
                        img.color = baseColor;
                    }
                }
            }
            else
            {
                img.color = baseColor;
            }
        }
        else
        {
            img.color = baseColor;
            itemDraggedRectTransform = null;
        }
    }
}
