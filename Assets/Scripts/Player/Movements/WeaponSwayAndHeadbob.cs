using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayAndHeadbob : MonoBehaviour
{
    Player player;

    [Header("Sway position")]
    public float swayPositionSpeed = 0.01f;
    public float maxSwayDistance = 0.06f;
    Vector3 swayPosition;

    [Header("Sway rotation")]
    public float swayRotationSpeed = 4f;
    public float maxSwayRotation = 5f;
    Vector3 swayRotation;

    [Header("Bob position")]
    public Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] Vector3 standingBobLimit = Vector3.one * 0.01f;
    [SerializeField] Vector3 walkBobLimit = Vector3.one * 0.02f;
    [SerializeField] Vector3 crouchBobLimit = Vector3.one * 0.01f;
    [SerializeField] Vector3 runBobLimit = Vector3.one * 0.03f;
    Vector3 bobLimit;
    [SerializeField] float baseBobFrequency = 0.01f;
    [SerializeField] float walkBobMultiplier = 4f;
    [SerializeField] float crouchBobMultiplier = 2f;
    [SerializeField] float runBobMultiplier = 6f;
    float bobFrequency;
    [SerializeField] float jumpBob = 0.01f;
    [SerializeField] float jumpBobLimit = 0.05f;

    float speedCurve;
    float sinCurve { get => Mathf.Sin(speedCurve); }
    float cosCurve { get => Mathf.Cos(speedCurve); }
    Vector3 bobPosition;
    Vector3 bobRotation;

    [Header("Bob rotation")]
    public Vector3 standingBobRotationMultiplier;
    public Vector3 movingBobRotationMultiplier;
    Vector3 bobRotationMultiplier;
    Vector2 lookInputs;

    [Header("Smooth Sway & Bob")]
    public float smoothPosition = 10f;
    public float smoothRotation = 12f;

    [Header("Running position & rotation")]
    [SerializeField] Transform weaponHolder;
    [SerializeField] Vector3 weaponHolderRunningPosition;
    [SerializeField] Quaternion weaponHolderRunningRotation;
    [SerializeField] float smoothRunningPosition = 5f;
    [SerializeField] float smoothRunningRotation = 5f;
    Vector3 weaponHolderStartPosition;
    Quaternion weaponHolderStartRotation;

    void Start()
    {
        player = GameManager.Instance.player;
        weaponHolderStartPosition = weaponHolder.localPosition;
        weaponHolderStartRotation = weaponHolder.localRotation;
    }

    void Update()
    {
        bobFrequency = player.isCrouching ? crouchBobMultiplier 
                    : player.isRunning ? runBobMultiplier
                    : walkBobMultiplier;

        bobLimit = player.isCrouching ? crouchBobLimit
                    : player.isRunning ? runBobLimit
                    : player.isMoving ? walkBobLimit
                    : standingBobLimit;

        bobRotationMultiplier = player.isMoving ? movingBobRotationMultiplier
                    : standingBobRotationMultiplier;

        lookInputs = !player.inventoryOpen ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;

        WeaponHolderTransform();

        Sway();
        SwayRotation();

        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    void WeaponHolderTransform()
    {
        if (player.isRunning)
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, weaponHolderRunningPosition, Time.deltaTime * smoothRunningPosition);
            weaponHolder.localRotation = Quaternion.Slerp(weaponHolder.localRotation, weaponHolderRunningRotation, Time.deltaTime * smoothRunningRotation);
        }
        else if (!player.isJumping)
        {
            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, weaponHolderStartPosition, Time.deltaTime * smoothRunningPosition);
            weaponHolder.localRotation = Quaternion.Slerp(weaponHolder.localRotation, weaponHolderStartRotation, Time.deltaTime * smoothRunningRotation);
        }
    }

    void Sway()
    {
        Vector3 invertLookPosition;
        invertLookPosition = lookInputs * -swayPositionSpeed;
        invertLookPosition.x = Mathf.Clamp(invertLookPosition.x, -maxSwayDistance, maxSwayDistance);
        invertLookPosition.y = Mathf.Clamp(invertLookPosition.y, -maxSwayDistance, maxSwayDistance);
        swayPosition = invertLookPosition;
    }

    void SwayRotation()
    {
        Vector2 invertLookRotation;
        invertLookRotation = lookInputs * -swayRotationSpeed;
        invertLookRotation.x = Mathf.Clamp(invertLookRotation.x, -maxSwayRotation, maxSwayRotation);
        invertLookRotation.y = Mathf.Clamp(-invertLookRotation.y, -maxSwayRotation, maxSwayRotation);
        swayRotation = new Vector3(invertLookRotation.y, invertLookRotation.x, invertLookRotation.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition + bobPosition, Time.deltaTime * smoothPosition);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayRotation) * Quaternion.Euler(bobRotation), Time.deltaTime * smoothRotation);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (player.characterController.isGrounded ? player.rawMoveInputs.magnitude * bobFrequency : 1f) + baseBobFrequency;
        speedCurve = speedCurve % (2 * Mathf.PI);
        bobPosition.x = (cosCurve * bobLimit.x * (player.characterController.isGrounded ? 1 : 0)) - (player.rawMoveInputs.x * travelLimit.x);
        bobPosition.y = player.isJumping ? Mathf.Clamp(player.characterController.velocity.y * -jumpBob, -jumpBobLimit, jumpBobLimit) : (sinCurve * bobLimit.y) - (player.rawMoveInputs.y * travelLimit.y);
        bobPosition.z = -(player.rawMoveInputs.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobRotation.x = (player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.x * (Mathf.Sin(2 * speedCurve)) : bobRotationMultiplier.x * (Mathf.Sin(2 * speedCurve) * 0.5f));
        bobRotation.y = (player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.y * cosCurve : 0);
        bobRotation.z = (player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.z * cosCurve * player.rawMoveInputs.x : 0);
    }
}