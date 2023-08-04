using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    Player player;

    [SerializeField] TMP_Text interactionText;
    [SerializeField] GameObject interactionPanel;
    RectTransform interactionPanelRectTransform;
    LayerMask layerMask;
    Interactable cachedInteractable;

    void Start()
    {
        player = GameManager.Instance.player;
        interactionPanelRectTransform = interactionPanel.GetComponent<RectTransform>();
        layerMask = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        if (!player.canInteract)
        {
            interactionPanel.SetActive(false);
            return;
        }

        Ray ray = player.playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, player.interactionRange, layerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (cachedInteractable != interactable)
            {
                interactionPanel.SetActive(false);
                cachedInteractable = interactable;
            }
            Vector3 interactionTextPosition = player.playerCamera.WorldToScreenPoint(hit.transform.position);
            interactionPanel.transform.position = interactionTextPosition;
            interactionText.text = cachedInteractable.interactText;
            interactionPanel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(interactionPanelRectTransform);
            Canvas.ForceUpdateCanvases();

            if (Input.GetKeyDown(KeyCode.F))
            {
                cachedInteractable.InteractAction();
            }
        }
        else
        {
            interactionPanel.SetActive(false);
        }
    }
}