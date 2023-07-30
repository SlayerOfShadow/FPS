using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadbob : MonoBehaviour
{
    Player player;
    [SerializeField] Vector2 walkAmount = new Vector2(0.02f, 0.01f);
    [SerializeField] Vector2 runAmount = new Vector2(0.04f, 0.02f);
    Vector2 amount;
    [SerializeField] float walkFrequency = 12f;
    [SerializeField] float runFrequency = 16f;
    float frequency;
    [SerializeField] float smooth = 20f;
    Vector3 startPosition;
    Vector3 headbobPosition;

    void Start()
    {
        player = GameManager.Instance.player;
        startPosition = transform.localPosition;
    }

    void Update()
    {
        frequency = player.isRunning ? runFrequency : walkFrequency;
        amount = player.isRunning ? runAmount : walkAmount;
        CheckHeadbobTrigger();
        StopHeadbob();
    }

    void CheckHeadbobTrigger()
    {
        if (player.isMoving)
        {
            StartHeadbob();
        }
    }

    Vector3 StartHeadbob()
    {
        headbobPosition = Vector3.zero;
        headbobPosition.y += Mathf.Lerp(headbobPosition.y, Mathf.Sin(Time.time * frequency) * amount.x, smooth * Time.deltaTime);
        headbobPosition.x += Mathf.Lerp(headbobPosition.x, Mathf.Cos(Time.time * frequency * 0.5f) * amount.y, smooth * Time.deltaTime);
        transform.localPosition += headbobPosition;
        return headbobPosition;
    }

    void StopHeadbob()
    {
        if (transform.localPosition == startPosition)
		{
			return;
		}
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, 1 * Time.deltaTime);
    }
}
