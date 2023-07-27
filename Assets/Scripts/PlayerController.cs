using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player player;
    Vector3 move_direction = Vector3.zero;
    float rotation_x = 0;
    CharacterController character_controller;
    Vector3 forward;
    Vector3 right;
    bool is_running;
    float cur_speed_x;
    float cur_speed_y;
    float movement_direction_y;

    void Start()
    {
        player = GameManager.Instance.player;
        character_controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        move_player();
        if (!player.inventory_open)
        {
            rotate_camera();
        }
    }

    void move_player()
    {
        forward = transform.TransformDirection(Vector3.forward);
        right = transform.TransformDirection(Vector3.right);

        is_running = Input.GetKey(KeyCode.LeftShift);
        cur_speed_x = (is_running ? player.run_speed : player.walk_speed) * Input.GetAxis("Vertical");
        cur_speed_y = (is_running ? player.run_speed : player.walk_speed) * Input.GetAxis("Horizontal");
        movement_direction_y = move_direction.y;
        move_direction = (forward * cur_speed_x) + (right * cur_speed_y);

        if (Input.GetButton("Jump") && !player.inventory_open && character_controller.isGrounded)
        {
            move_direction.y = player.jump_power;
        }
        else
        {
            move_direction.y = movement_direction_y;
        }

        if (!character_controller.isGrounded)
        {
            move_direction.y -= player.gravity * Time.deltaTime;
        }

        character_controller.Move(move_direction * Time.deltaTime);
    }

    void rotate_camera()
    {
        rotation_x += -Input.GetAxis("Mouse Y") * player.look_speed;
        rotation_x = Mathf.Clamp(rotation_x, -player.look_x_limit, player.look_x_limit);
        player.player_camera.transform.localRotation = Quaternion.Euler(rotation_x, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * player.look_speed, 0);
    }
}
