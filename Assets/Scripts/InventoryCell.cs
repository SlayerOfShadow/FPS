using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    public state cellState;
    [SerializeField] Color baseColor;
    [SerializeField] Color previewColor;
    [SerializeField] Color occupiedColor;
    [SerializeField] Image img;

    void Update()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        switch (cellState)
        {
            case state.none:
                img.color = baseColor;
                break;
            case state.preview:
                img.color = previewColor;
                break;
            case state.occupied:
                img.color = occupiedColor;
                break;
        }
    }
}

public enum state
{
    none,
    preview,
    occupied
}