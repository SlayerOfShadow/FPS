using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    public CellState cellState;
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
            case CellState.none:
                if (img.color != baseColor)
                img.color = baseColor;
                break;
            case CellState.preview:
                img.color = previewColor;
                break;
            case CellState.occupied:
                img.color = occupiedColor;
                break;
        }
    }
}

public enum CellState
{
    none,
    preview,
    occupied
}