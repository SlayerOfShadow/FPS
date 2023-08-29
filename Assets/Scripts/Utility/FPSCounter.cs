using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TMP_Text fpsText;
    [SerializeField] float refreshRate = 1f;
    float time;
    int frameCount;
    int frameRate;

    void Update()
    {
        time += Time.deltaTime;
        frameCount++;
        if (time >= refreshRate)
        {
            frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = frameRate.ToString() + " Fps";
            time -= refreshRate;
            frameCount = 0;
        }
    }
}