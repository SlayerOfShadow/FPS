using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    Player player;
    LayerMask layer_mask;
    Ray ray;
    RaycastHit hit;
    RectTransform interaction_panel_transform;
    Vector3 interaction_text_position;
    Interactable interactable;
    Interactable cached_interactable;

    void Start()
    {
        player = GameManager.Instance.player;
        layer_mask = LayerMask.GetMask("Interactable");
        interaction_panel_transform = player.interaction_panel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!player.can_interact)
        {
            player.interaction_panel.SetActive(false);
            return;
        }

        ray = player.player_camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (Physics.Raycast(ray, out hit, player.interaction_range, layer_mask))
        {
            interactable = hit.transform.GetComponent<Interactable>();
            if (cached_interactable != interactable)
            {
                cached_interactable = interactable;
            }
            interaction_text_position = player.player_camera.WorldToScreenPoint(hit.transform.position);
            player.interaction_panel.transform.position = interaction_text_position;
            player.interaction_text.text = cached_interactable.interact_text;
            player.interaction_panel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(interaction_panel_transform);
            Canvas.ForceUpdateCanvases();

            if (Input.GetKeyDown(KeyCode.F))
            {
                cached_interactable.interact_action();
            }
        }
        else
        {
            player.interaction_panel.SetActive(false);
        }
    }
}
