using UnityEngine;

public class WeaponMovements : MonoBehaviour
{
    Player player;

    [Header("Sway position")]
    [SerializeField] float swayPositionSpeed = 0.01f;
    [SerializeField] float maxSwayDistance = 0.06f;
    Vector3 swayPosition;

    [Header("Sway rotation")]
    [SerializeField] float swayRotationSpeed = 4f;
    [SerializeField] float maxSwayRotation = 5f;
    Vector3 swayRotation;

    [Header("Bob position")]
    [SerializeField] Vector3 movingTravelLimit = Vector3.one * 0.02f;
    [SerializeField] Vector3 aimingTravelLimit = Vector3.one * 0.01f;
    Vector3 travelLimit;
    [SerializeField] Vector3 standingBobLimit = Vector3.one * 0.01f;
    [SerializeField] Vector3 walkBobLimit = Vector3.one * 0.02f;
    [SerializeField] Vector3 crouchBobLimit = Vector3.one * 0.01f;
    [SerializeField] Vector3 runBobLimit = Vector3.one * 0.03f;
    [SerializeField] Vector3 aimBobLimit = Vector3.one * 0.005f;
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
    [SerializeField] Vector3 standingBobRotationMultiplier;
    [SerializeField] Vector3 movingBobRotationMultiplier;
    [SerializeField] Vector3 aimingBobRotationMultiplier;
    Vector3 bobRotationMultiplier;
    Vector2 lookInputs;

    [Header("Smooth Sway & Bob")]
    [SerializeField] float smoothPosition = 10f;
    [SerializeField] float smoothRotation = 12f;

    [Header("Running position & rotation")]
    [SerializeField] Transform weaponHolder;
    [SerializeField] Vector3 weaponHolderRunPosition;
    [SerializeField] Quaternion weaponHolderRunRotation;
    [SerializeField] float smoothMovements = 5f;
    Vector3 weaponHolderStartPosition;
    Quaternion weaponHolderStartRotation;

    [Header("Recoil")]
    [SerializeField] Transform cameraHolder;
    Vector3 cameraHolderStartPosition;
    Quaternion cameraHolderStartRotation;
    bool recoil = false;
    float upRecoil, sideRecoil, kickBack;


    void Start()
    {
        player = GameManager.Instance.player;
        weaponHolderStartPosition = weaponHolder.localPosition;
        weaponHolderStartRotation = weaponHolder.localRotation;
        cameraHolderStartPosition = cameraHolder.localPosition;
        cameraHolderStartRotation = cameraHolder.localRotation;
    }

    void Update()
    {
        travelLimit = player.isAiming ? aimingTravelLimit : movingTravelLimit;

        bobFrequency = player.isCrouching ? crouchBobMultiplier
                    : player.isRunning ? runBobMultiplier
                    : walkBobMultiplier;

        bobLimit = player.isAiming ? aimBobLimit
                    : player.isCrouching ? crouchBobLimit
                    : player.isRunning ? runBobLimit
                    : player.isMoving ? walkBobLimit
                    : standingBobLimit;

        bobRotationMultiplier = player.isAiming ? aimingBobRotationMultiplier
                    : player.isMoving ? movingBobRotationMultiplier
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
        Vector3 targetPosition = weaponHolderStartPosition;
        Quaternion targetRotation = weaponHolderStartRotation;

        Vector3 cameraTargetPosition = cameraHolderStartPosition;
        Quaternion cameraTargetRotation = cameraHolderStartRotation;

        if (player.isAiming)
        {
            targetPosition = player.playerEquipment.weaponHeld.weaponAimPosition;
            targetRotation = player.playerEquipment.weaponHeld.weaponAimRotation;
        }
        else if (player.isRunning)
        {
            targetPosition = weaponHolderRunPosition;
            targetRotation = weaponHolderRunRotation;
        }

        if (recoil)
        {
            Vector3 newPosition = new Vector3(0, 0, kickBack);
            Quaternion newRotation = Quaternion.Euler(-upRecoil, Random.Range(-sideRecoil, sideRecoil), 0);
            targetPosition -= newPosition;
            targetRotation *= newRotation;
            cameraTargetPosition -= newPosition;
            cameraTargetRotation *= newRotation;
            recoil = false;
        }

        weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, targetPosition, smoothMovements * Time.deltaTime);
        weaponHolder.localRotation = Quaternion.Slerp(weaponHolder.localRotation, targetRotation, smoothMovements * Time.deltaTime);
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, cameraTargetPosition, smoothMovements * 0.25f * Time.deltaTime);
        cameraHolder.localRotation = Quaternion.Slerp(cameraHolder.localRotation, cameraTargetRotation, smoothMovements * 0.25f * Time.deltaTime);
    }

    public void Recoil(float up, float side, float kick)
    {
        recoil = true;
        upRecoil = up;
        sideRecoil = side;
        kickBack = kick;
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