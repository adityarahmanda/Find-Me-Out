using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDFPS : MonoBehaviour
{
    public float updateInterval = 0.5f;

    private float accum = 0;
    private int frames = 0;
    private float timeLeft;

    public TextMeshProUGUI fpsText;

    void Start()
    {
        if(fpsText == null)
        {
            Debug.Log("HUD FPS needs Text");
            enabled = false;
            return;
        }
        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;

        if(timeLeft <= 0)
        {
            float fps = accum/frames;
            fpsText.text = "FPS : " + fps.ToString("F0");

            timeLeft = updateInterval;
            accum = 0;
            frames = 0;
        }
    }
}
