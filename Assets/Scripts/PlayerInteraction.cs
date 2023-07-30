using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    Player player;
    [SerializeField] GameObject interactionPanel;
    RectTransform interactionPanelRectTransform;
    LayerMask layerMask;
    Ray ray;
    RaycastHit hit;
    Vector3 interactionTextPosition;
    Interactable interactable;
    Interactable cachedInteractable;

    void Start()
    {
        player = GameManager.Instance.player;
        layerMask = LayerMask.GetMask("Interactable");
        interactionPanelRectTransform = interactionPanel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!player.canInteract)
        {
            interactionPanel.SetActive(false);
            return;
        }

        ray = player.playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (Physics.Raycast(ray, out hit, player.interactionRange, layerMask))
        {
            interactable = hit.transform.GetComponent<Interactable>();
            if (cachedInteractable != interactable)
            {
                cachedInteractable = interactable;
            }
            interactionTextPosition = player.playerCamera.WorldToScreenPoint(hit.transform.position);
            interactionPanel.transform.position = interactionTextPosition;
            player.interactionText.text = cachedInteractable.interactText;
            interactionPanel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(interactionPanelRectTransform);
            Canvas.ForceUpdateCanvases();

            if (Input.GetKeyDown(KeyCode.F))
            {
                cachedInteractable.interactAction();
            }
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }
}
