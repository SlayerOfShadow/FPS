using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player player;
    Vector3 moveDirection = Vector3.zero;
    float xRotation = 0;
    Vector3 forward;
    Vector3 right;
    float yMovementDirection;
    Vector3 targetMoveDirection;
    Vector2 movements;
    float yVelocity = 0.0f;

    void Start()
    {
        player = GameManager.Instance.player;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MovePlayer();
        if (!player.inventoryOpen)
        {
            RotateCamera();
        }
    }

    void MovePlayer()
    {
        if (player.isCrouching)
        {
            player.characterController.height = Mathf.SmoothDamp(player.characterController.height, player.crouchHeight, ref yVelocity, player.smoothCrouch);
        }
        else
        {
            player.characterController.height = Mathf.SmoothDamp(player.characterController.height, player.normalHeight, ref yVelocity, player.smoothCrouch);;
        }

        if (!player.isJumping)
        {
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            movements = player.isRunning ? player.rawMoveInputs * player.runSpeed
                        : player.isCrouching ? player.rawMoveInputs * player.crouchSpeed
                        : player.rawMoveInputs * player.walkSpeed;
        }
        yMovementDirection = moveDirection.y;
        targetMoveDirection = (forward * movements.y) + (right * movements.x);
        moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref moveDirection, player.smoothMoveSpeed);

        moveDirection.y = Input.GetButton("Jump") && player.canJump ? player.jumpPower : yMovementDirection;

        if (player.isJumping)
        {
            moveDirection.y -= player.gravity * Time.deltaTime;
        }
        else
        {
            player.isJumping = false;
        }

        player.characterController.Move(moveDirection * Time.deltaTime);
    }

    void RotateCamera()
    {
        xRotation += -Input.GetAxis("Mouse Y") * player.lookSpeed;
        xRotation = Mathf.Clamp(xRotation, -player.lookXLimit, player.lookXLimit);
        player.playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * player.lookSpeed, 0);
    }
}
