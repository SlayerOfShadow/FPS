using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] GameObject interactionPanel;
    RectTransform interactionPanelRectTransform;
    LayerMask layerMask;
    Interactable cachedInteractable;

    void Start()
    {
        interactionPanelRectTransform = interactionPanel.GetComponent<RectTransform>();
        layerMask = LayerMask.GetMask("Interactable");
    }

    void Update()
    {
        if (!GameManager.Instance.player.canInteract)
        {
            interactionPanel.SetActive(false);
            return;
        }

        Ray ray = GameManager.Instance.player.playerCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, GameManager.Instance.player.interactionRange, layerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (cachedInteractable != interactable)
            {
                cachedInteractable = interactable;
            }
            Vector3 interactionTextPosition = GameManager.Instance.player.playerCamera.WorldToScreenPoint(hit.transform.position);
            interactionPanel.transform.position = interactionTextPosition;
            GameManager.Instance.player.interactionText.text = cachedInteractable.interactText;
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
