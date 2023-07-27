using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TMP_Text fps_text;
    float polling_time = 1f;
    float time;
    int frame_count;
    int frame_rate;

    void Update()
    {
        time += Time.deltaTime;
        frame_count++;
        if (time >= polling_time)
        {
            frame_rate = Mathf.RoundToInt(frame_count / time);
            fps_text.text = frame_rate.ToString() + " FPS";
            time -= polling_time;
            frame_count = 0;
        }
    }
}
