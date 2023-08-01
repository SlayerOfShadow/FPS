using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.zero;
    float xRotation = 0;
    Vector2 movements;
    Vector2 inputs;
    Vector3 initialCameraPosition;
    float currentHeight;
    bool IsCrouching => GameManager.Instance.player.standingHeight - currentHeight > .1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initialCameraPosition = GameManager.Instance.player.playerCamera.transform.localPosition;
        currentHeight = GameManager.Instance.player.standingHeight;
    }

    void Update()
    {
        MoveAndJump();
        Crouch();
        if (!GameManager.Instance.player.inventoryOpen) RotateCamera();
    }

    void MoveAndJump()
    {
        // Move
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        if (!GameManager.Instance.player.isJumping)
        {
            inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            movements = inputs * GameManager.Instance.player.currentSpeed;
        }

        float yMovementDirection = moveDirection.y;
        Vector3 targetMoveDirection = (forward * movements.y) + (right * movements.x);
        moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref moveDirection, GameManager.Instance.player.smoothMoveSpeed);

        // Jump
        bool isTryingToJump = false;
        if (Input.GetButton("Jump") && GameManager.Instance.player.canJump)
        {
            moveDirection.y = GameManager.Instance.player.jumpPower;
            isTryingToJump = true;
        }
        else
        {
            moveDirection.y = yMovementDirection;
        }

        if (GameManager.Instance.player.isJumping)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, currentHeight * 0.5f, 0);
            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.1f))
            {
                moveDirection.y = -0.2f;
            }
            moveDirection.y -= GameManager.Instance.player.gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = 0 + (isTryingToJump ? GameManager.Instance.player.jumpPower : -3.0f);
        }

        // Final
        GameManager.Instance.player.characterController.Move(moveDirection * Time.deltaTime);
    }

    void Crouch()
    {
        GameManager.Instance.player.isCrouching = IsCrouching;

        float targetHeight = GameManager.Instance.player.isTryingToCrouch ? GameManager.Instance.player.crouchHeight : GameManager.Instance.player.standingHeight;

        if (IsCrouching && !GameManager.Instance.player.isTryingToCrouch)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, currentHeight * 0.5f, 0);
            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.2f))
            {
                float distanceToCeiling = hit.point.y - castOrigin.y;
                targetHeight = Mathf.Max(currentHeight + distanceToCeiling - 0.1f, GameManager.Instance.player.crouchHeight);
            }
        }

        if (!Mathf.Approximately(targetHeight, currentHeight))
        {
            float crouchDelta = Time.deltaTime * GameManager.Instance.player.smoothCrouch;
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchDelta);

            Vector3 halfHeightDifference = new Vector3(0, (GameManager.Instance.player.standingHeight - currentHeight) * 0.5f, 0);
            Vector3 newCameraPosition = initialCameraPosition - halfHeightDifference;

            GameManager.Instance.player.playerCamera.transform.localPosition = newCameraPosition;
            GameManager.Instance.player.characterController.height = currentHeight;
        }
    }

    void RotateCamera()
    {
        xRotation += -Input.GetAxis("Mouse Y") * GameManager.Instance.player.lookSpeed;
        xRotation = Mathf.Clamp(xRotation, -GameManager.Instance.player.lookXLimit, GameManager.Instance.player.lookXLimit);
        GameManager.Instance.player.playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * GameManager.Instance.player.lookSpeed, 0);
    }
}