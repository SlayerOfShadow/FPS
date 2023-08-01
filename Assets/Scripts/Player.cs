using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    public Camera playerCamera;
    public CharacterController characterController;
    public TMP_Text interactionText;
    public GameObject playerInventoryPanel;

    [Header("Movements")]
    public float currentSpeed;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float smoothMoveSpeed = 0.01f;
    public float jumpPower = 6f;
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float smoothCrouch = 1f;
    public float gravity = 15f;
    public float lookSpeed = 1f;
    public float lookXLimit = 90f;
    public float interactionRange = 5f;

    [Header("Inventory")]
    public PlayerInventory playerInventory;
    public bool inventoryOpen = false;
    public GameObject inventoryIconsHandler;

    [Header("Actions")]
    public bool canInteract = true;
    public bool canOpenInventory = true;
    public bool canMove = true;
    public bool canRun = true;
    public bool canJump = false;
    public bool canCrouch = true;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isJumping = true;
    public bool isTryingToCrouch = false;
    public bool isCrouching = false;

    [Header("Inputs")]
    public Vector2 moveInputs;
    public Vector2 rawMoveInputs;

    void Update()
    {
        currentSpeed = isRunning ? runSpeed
                    : isCrouching ? crouchSpeed
                    : walkSpeed;

        // Jump
        canJump = characterController.isGrounded;
        isJumping = !canJump;

        // Move
        canMove = !isJumping;
        if (canMove)
        {
            moveInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            rawMoveInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }   
        isMoving = rawMoveInputs.magnitude > 0;

        // Run
        canRun = rawMoveInputs.y > 0 && isMoving && !isCrouching && characterController.height >= standingHeight - 0.15f;
        if (canRun)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
        }
        else
        {
            isRunning = false;
        }

        // Crouch
        canCrouch = !isJumping;
        if (canCrouch)
        {
            isTryingToCrouch = Input.GetKey(KeyCode.X);
            if (isTryingToCrouch && isRunning)
            {
                isRunning = false;
            }
        }
        
        // Interact
        canInteract = !inventoryOpen;
    }
}