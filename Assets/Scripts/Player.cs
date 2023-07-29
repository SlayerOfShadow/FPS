using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Camera player_camera;
    public GameObject interaction_panel;
    public TMP_Text interaction_text;

    [Header("Stats")]
    public float walk_speed = 3f;
    public float run_speed = 6f;
    public float smooth_move_speed = 0.01f;
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
    public bool can_interact = true;
    public bool can_open_inventory = true;
    public bool is_running = false;
    public bool is_jumping = false;

    [Header("Inputs")]
    public Vector2 move_inputs;
    public Vector2 look_inputs;

    void Update()
    {
        move_inputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        look_inputs = inventory_open ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized : Vector2.zero;
        is_running = Input.GetKey(KeyCode.LeftShift);
    }
}
