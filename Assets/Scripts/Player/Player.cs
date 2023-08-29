using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public CharacterController characterController;

    [Header("Stats")]
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
    public float dropDistance = 1f;
    public float dropForce = 1f;

    [Header("Inventory")]
    [HideInInspector] public PlayerInventory playerInventory;
    [HideInInspector] public PlayerEquipment playerEquipment;

    [Header("States")]
    public bool inventoryOpen = false;
    public bool canMove = true;
    public bool canRun = true;
    public bool canJump = false;
    public bool canCrouch = false;
    public bool canOpenInventory = true;
    public bool canInteract = true;
    public bool canAim = true;
    public bool isMoving = false;
    public bool isRunning = false;
    public bool isJumping = true;
    public bool isTryingToCrouch = false;
    public bool isCrouching = false;
    public bool isAiming = false;

    [Header("Inputs")]
    public Vector2 rawMoveInputs;

    void Start()
    {
        playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        playerInventory = GetComponent<PlayerInventory>();
        playerEquipment = GetComponent<PlayerEquipment>();
    }

    void Update()
    {
        canJump = characterController.isGrounded;
        isJumping = !canJump;

        canMove = !isJumping;
        if (canMove) rawMoveInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        isMoving = rawMoveInputs.magnitude > 0;

        canRun = !isAiming && characterController.isGrounded && rawMoveInputs.y > 0 && !isCrouching && characterController.height >= standingHeight - 0.15f;
        isRunning = canRun && Input.GetKey(KeyCode.LeftShift);

        canCrouch = !isJumping;
        if (canCrouch) isTryingToCrouch = Input.GetKey(KeyCode.X);
        if (isTryingToCrouch && isRunning) isRunning = false;

        canAim = !inventoryOpen && !playerEquipment.isSwitching;
        isAiming = canAim && Input.GetMouseButton(1) && playerEquipment.playerArms.activeSelf;

        canInteract = !inventoryOpen;

        currentSpeed = isRunning ? runSpeed
                    : isCrouching ? crouchSpeed
                    : walkSpeed;
    }
}