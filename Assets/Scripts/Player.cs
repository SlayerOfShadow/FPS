using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Camera player_camera;

    [Header("Stats")]
    public float walk_speed = 3f;
    public float run_speed = 6f;
    public float jump_power = 6f;
    public float gravity = 15f;
    public float look_speed = 1f;
    public float look_x_limit = 90f;
    public float interaction_range = 5f;

    [Header("Inventory")]
    public GameObject player_inventory;
    public bool inventory_open = false;
    public GameObject inventory_icons_handler;

    [Header("Actions")]
    public GameObject interaction_panel;
    public TMP_Text interaction_text;
    public bool can_interact = true;
    public bool can_open_inventory = true;
}
