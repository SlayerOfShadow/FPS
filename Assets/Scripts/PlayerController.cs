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
    float movement_direction_y;
    Vector3 target_move_direction;
    Vector2 movements;

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
        if (!player.is_jumping)
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            movements = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal")).normalized;
            movements = player.is_running ? movements * player.run_speed : movements * player.walk_speed;
        }
        movement_direction_y = move_direction.y;
        target_move_direction = (forward * movements.x) + (right * movements.y);
        move_direction = Vector3.SmoothDamp(move_direction, target_move_direction, ref move_direction, player.smooth_move_speed);
        
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
            player.is_jumping = true;
        }
        else
        {
            player.is_jumping = false;
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
