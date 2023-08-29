using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    Player player;

    [SerializeField] float walkAmount = 0.01f;
    [SerializeField] float runAmount = 0.03f;
    [SerializeField] float crouchAmount = 0.005f;
    float amount;
    [SerializeField] float walkFrequency = 12f;
    [SerializeField] float runFrequency = 14f;
    [SerializeField] float crouchFrequency = 8f;
    float frequency;
    [SerializeField] float smooth = 20f;
    [SerializeField] float returnSpeed = 10f;
    Vector3 startPosition;

    void Start()
    {
        player = GameManager.Instance.player;
        startPosition = transform.localPosition;
    }

    void Update()
    {
        frequency = player.isRunning ? runFrequency
                : player.isCrouching ? crouchFrequency
                : walkFrequency;

        amount = player.isRunning ? runAmount
                : player.isCrouching ? crouchAmount
                : walkAmount;

        CheckHeadbobTrigger();
        StopHeadbob();
    }

    void CheckHeadbobTrigger()
    {
        if (player.isMoving && !player.isJumping)
        {
            StartHeadbob();
        }
    }

    Vector3 StartHeadbob()
    {
        Vector3 headbobPosition = Vector3.zero;
        headbobPosition.y += Mathf.Lerp(headbobPosition.y, Mathf.Sin(Time.time * frequency) * amount, smooth * Time.deltaTime);
        transform.localPosition += headbobPosition;
        return headbobPosition;
    }

    void StopHeadbob()
    {
        if (transform.localPosition == startPosition)
        {
            return;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, returnSpeed * Time.deltaTime);
    }
}