using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    public state cell_state;
    Color next_color;
    [SerializeField] Color base_color;
    [SerializeField] Color preview_color;
    [SerializeField] Color occupied_color;
    [SerializeField] Color hover_color;
    [SerializeField] Image img;

    void Update()
    {
        change_color();
    }

    public void change_color()
    {
        switch (cell_state)
        {
            case state.none:
                img.color = base_color;
                break;
            case state.preview:
                img.color = preview_color;
                break;
            case state.occupied:
                img.color = occupied_color;
                break;
            case state.hover:
                img.color = hover_color;
                break;
        }
    }
}

public enum state
{
    none,
    preview,
    occupied,
    hover
}