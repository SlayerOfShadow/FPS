using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadbob : MonoBehaviour
{
    Player player;
    [SerializeField] float x_amount = 0.01f;
    [SerializeField] float y_amount = 0.01f;
    [SerializeField] float frequency = 7.5f;
    [SerializeField] float smooth = 30f;
    Vector3 start_pos;
	Vector2 inputs;
	float inputs_magnitude;

    void Start()
    {
        player = GameManager.Instance.player;
        start_pos = transform.localPosition;
    }

    void Update()
    {
        check_headbob_trigger();
        stop_headbob();
    }

    void check_headbob_trigger()
    {
        if (player.move_inputs.magnitude > 0)
        {
            start_headbob();
        }
    }

    Vector3 start_headbob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * x_amount, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency * 0.5f) * y_amount, smooth * Time.deltaTime);
        transform.localPosition += pos;
        return pos;
    }

    void stop_headbob()
    {
        if (transform.localPosition == start_pos)
		{
			return;
		}
        transform.localPosition = Vector3.Lerp(transform.localPosition, start_pos, 1 * Time.deltaTime);
    }
}
