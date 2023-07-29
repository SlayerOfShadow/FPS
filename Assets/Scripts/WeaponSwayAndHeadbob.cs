using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayAndHeadbob : MonoBehaviour
{
    Player player;
    [SerializeField] CharacterController character_controller;

    [Header("Sway")]
    public float step = 0.01f;
    public float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    public float rotationStep = 4f;
    public float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;
    Vector2 look_inputs;

    void Start()
    {
        player = GameManager.Instance.player; 
    }

    void Update()
    {
        look_inputs.x = !GameManager.Instance.player.inventory_open ? Input.GetAxis("Mouse X") : 0;
        look_inputs.y = !GameManager.Instance.player.inventory_open ? Input.GetAxis("Mouse Y") : 0;
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    void Sway()
    {
        Vector3 invertLook = look_inputs * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = look_inputs * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (character_controller.isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * bobExaggeration : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (character_controller.isGrounded ? 1 : 0)) - (player.move_inputs.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(player.move_inputs.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = (player.move_inputs != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (player.move_inputs != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (player.move_inputs != Vector2.zero ? multiplier.z * curveCos * player.move_inputs.x : 0);
    }
}