using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCamera;
    public CharacterController characterController;

    [Header("Movements")]
    public float currentSpeed;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float jumpPower = 6f;
    public float gravity = 15f;
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float smoothMoveSpeed = 0.01f;
    public float smoothCrouch = 1f;
    public float lookSpeed = 1f;
    public float lookXLimit = 90f;
    public float interactionRange = 5f;
    public float pushPower = 1f;

    [Header("Inventory")]
    public PlayerInventory playerInventory;
    public bool inventoryOpen = false;
    public GameObject inventoryIconsHandler;

    [Header("States")]
    public bool canMove = true;
    public bool canRun = true;
    public bool canJump = false;
    public bool canCrouch = false;
    public bool canOpenInventory = true;
    public bool canInteract = true;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isJumping = true;
    public bool isTryingToCrouch = false;
    public bool isCrouching = false;

    [Header("Inputs")]
    public Vector2 rawMoveInputs;

    void Update()
    {
        currentSpeed = isRunning ? runSpeed
                    : isCrouching ? crouchSpeed
                    : walkSpeed;

        canJump = characterController.isGrounded;
        isJumping = !canJump;

        canMove = !isJumping;
        if (canMove) rawMoveInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        isMoving = rawMoveInputs.magnitude > 0;

        canRun = rawMoveInputs.y > 0 && isMoving && !isCrouching && characterController.height >= standingHeight - 0.15f;
        isRunning = canRun ? Input.GetKey(KeyCode.LeftShift) : false;

        canCrouch = !isJumping;
        if (canCrouch) isTryingToCrouch = Input.GetKey(KeyCode.X);
        if (isTryingToCrouch && isRunning) isRunning = false;

        canInteract = !inventoryOpen;
    }
}