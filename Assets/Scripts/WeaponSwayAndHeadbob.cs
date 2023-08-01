using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayAndHeadbob : MonoBehaviour
{
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
    [SerializeField] Vector3 walkBobLimit = Vector3.one * 0.02f;
    [SerializeField] Vector3 runBobLimit = Vector3.one * 0.04f;
    [SerializeField] Vector3 crouchBobLimit = Vector3.one * 0.01f;
    [SerializeField] Vector3 standingBobLimit = Vector3.one * 0.01f;
    Vector3 bobLimit;
    [SerializeField] float walkBobFrequency = 4f;
    [SerializeField] float runBobFrequency = 6f;
    [SerializeField] float crouchBobFrequency = 2f;
    float bobFrequency;
    [SerializeField] float jumpBob = 0.01f;
    [SerializeField] float jumpBobLimit = 0.05f;

    float speedCurve;
    float sinCurve { get => Mathf.Sin(speedCurve); }
    float cosCurve { get => Mathf.Cos(speedCurve); }
    Vector3 bobPosition;
    Vector3 bobRotation;

    [Header("Bob Rotation")]
    public Vector3 movingBobRotationMultiplier;
    public Vector3 standingBobRotationMultiplier;
    Vector3 bobRotationMultiplier;
    Vector2 lookInputs;

    [Header("Smooth")]
    public float smoothPosition = 10f;
    public float smoothRotation = 12f;

    void Update()
    {
        bobFrequency = GameManager.Instance.player.isRunning ? runBobFrequency
                    : GameManager.Instance.player.isCrouching ? crouchBobFrequency
                    : walkBobFrequency;

        bobLimit = GameManager.Instance.player.isRunning ? runBobLimit
                    : GameManager.Instance.player.isCrouching ? crouchBobLimit
                    : GameManager.Instance.player.isMoving ? walkBobLimit
                    : standingBobLimit;

        bobRotationMultiplier = GameManager.Instance.player.isMoving ? movingBobRotationMultiplier
                    : standingBobRotationMultiplier;

        lookInputs = !GameManager.Instance.player.inventoryOpen ? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) : Vector2.zero;

        Sway();
        SwayRotation();

        BobOffset();
        BobRotation();

        CompositePositionRotation();
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
        speedCurve += Time.deltaTime * (GameManager.Instance.player.characterController.isGrounded ? GameManager.Instance.player.rawMoveInputs.magnitude * bobFrequency : 1f) + 0.01f;
        speedCurve = speedCurve % (2 * Mathf.PI);
        bobPosition.x = (cosCurve * bobLimit.x * (GameManager.Instance.player.characterController.isGrounded ? 1 : 0)) - (GameManager.Instance.player.rawMoveInputs.x * travelLimit.x);
        bobPosition.y = GameManager.Instance.player.isJumping ? Mathf.Clamp(GameManager.Instance.player.characterController.velocity.y * -jumpBob, -jumpBobLimit, jumpBobLimit) : (sinCurve * bobLimit.y) - (GameManager.Instance.player.rawMoveInputs.y * travelLimit.y);
        bobPosition.z = -(GameManager.Instance.player.rawMoveInputs.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobRotation.x = (GameManager.Instance.player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.x * (Mathf.Sin(2 * speedCurve)) : bobRotationMultiplier.x * (Mathf.Sin(2 * speedCurve) * 0.5f));
        bobRotation.y = (GameManager.Instance.player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.y * cosCurve : 0);
        bobRotation.z = (GameManager.Instance.player.rawMoveInputs != Vector2.zero ? bobRotationMultiplier.z * cosCurve * GameManager.Instance.player.rawMoveInputs.x : 0);
    }
}