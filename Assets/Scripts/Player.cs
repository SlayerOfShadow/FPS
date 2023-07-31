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
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float smoothMoveSpeed = 0.01f;
    public float jumpPower = 6f;
    public float normalHeight = 2f;
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
    public bool isCrouching = false;

    [Header("Inputs")]
    public Vector2 moveInputs;
    public Vector2 rawMoveInputs;
    public Vector2 lookInputs;

    void Update()
    {
        if (canMove)
        {
            moveInputs = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            rawMoveInputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }
        lookInputs = inventoryOpen ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).normalized : Vector2.zero;

        if (canRun)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
        }
        isCrouching = Input.GetKey(KeyCode.X);
        isMoving = moveInputs.magnitude > 0 ? true : false;
        isJumping = !characterController.isGrounded;

        canInteract = !inventoryOpen;
        canMove = characterController.isGrounded;
        canJump = characterController.isGrounded;
        canCrouch = characterController.isGrounded;
        canRun = isMoving && !isCrouching;
    }
}
