using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Player player;

    Vector3 moveDirection = Vector3.zero;
    float xRotation = 0;
    Vector2 movements;
    Vector3 initialCameraPosition;
    float currentHeight;
    bool IsCrouching => player.standingHeight - currentHeight > .1f;
    Vector3 targetMoveDirection;

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
        #region Move
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        movements = player.rawMoveInputs * player.currentSpeed;

        float yMovementDirection = moveDirection.y;
        if (!player.isJumping) targetMoveDirection = (forward * movements.y) + (right * movements.x);
        moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref moveDirection, player.smoothMoveSpeed);
        #endregion

        #region Jump
        bool isTryingToJump = false;
        if (Input.GetButton("Jump") && player.canJump)
        {
            moveDirection.y = player.jumpPower;
            isTryingToJump = true;
        }
        else
        {
            moveDirection.y = yMovementDirection;
        }

        if (player.isJumping)
        {
            Vector3 castOrigin = transform.position + new Vector3(0, currentHeight * 0.5f, 0);
            if (Physics.Raycast(castOrigin, Vector3.up, out RaycastHit hit, 0.1f))
            {
                moveDirection.y = -0.2f;
            }
            moveDirection.y -= player.gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = 0 + (isTryingToJump ? player.jumpPower : -3.0f);
        }
        #endregion

        player.characterController.Move(moveDirection * Time.deltaTime);
    }

    void Crouch()
    {
        player.isCrouching = IsCrouching;

        float targetHeight = player.isTryingToCrouch ? player.crouchHeight : player.standingHeight;

        if (IsCrouching && !player.isTryingToCrouch)
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 collisionPoint = hit.point;
        body.AddForceAtPosition(pushDir * player.pushPower, collisionPoint, ForceMode.Impulse);
    }
}