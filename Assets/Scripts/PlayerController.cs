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
    Vector2 inputs;
    Vector3 initialCameraPosition;
    float currentHeight;
    bool IsCrouching => player.standingHeight - currentHeight > .1f;

    void Start()
    {
        player = GameManager.Instance.player;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCameraPosition = player.playerCamera.transform.localPosition;
        currentHeight = player.standingHeight;
    }

    void Update()
    {
        MoveAndJump();
        Crouch();
        if (!player.inventoryOpen) RotateCamera();
    }

    void MoveAndJump()
    {
        if (!player.isJumping)
        {
            inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            forward = transform.TransformDirection(Vector3.forward);
            right = transform.TransformDirection(Vector3.right);
            movements = player.isRunning ? inputs * player.runSpeed
                        : player.isCrouching ? inputs * player.crouchSpeed
                        : inputs * player.walkSpeed;
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

    void Crouch()
    {
        float targetHeight = player.isCrouching ? player.crouchHeight : player.standingHeight;

        if (IsCrouching && !player.isCrouching)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, currentHeight * 0.5f, 0);
            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
            {
                float distanceToCeiling = hit.point.y - castOrigin.y;
                targetHeight = Mathf.Max(currentHeight + distanceToCeiling - 0.1f, player.crouchHeight);
            }
        }

        if (!Mathf.Approximately(targetHeight, currentHeight))
        {
            float crouchDelta = Time.deltaTime * player.smoothCrouch;
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchDelta);

            Vector3 halfHeightDifference = new Vector3(0, (player.standingHeight - currentHeight) * 0.5f, 0);
            Vector3 newCameraPosition = initialCameraPosition - halfHeightDifference;

            player.playerCamera.transform.localPosition = newCameraPosition;
            player.characterController.height = currentHeight;
        }
    }

    void RotateCamera()
    {
        xRotation += -Input.GetAxis("Mouse Y") * player.lookSpeed;
        xRotation = Mathf.Clamp(xRotation, -player.lookXLimit, player.lookXLimit);
        player.playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * player.lookSpeed, 0);
    }
}
