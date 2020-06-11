using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] public static TimeSpan gameTime;

    [SerializeField] private float elapsedTime;

    private float cycleMinutes = 1;
    private float cycleSpeed;

    void Start()
    {
        cycleSpeed = 1440.0f / cycleMinutes;
    }

    void Update()
    {
        UpdateGameTime();
    }

    void UpdateGameTime()
    {
        if (!PauseSystem.paused)
        {
            elapsedTime += Time.deltaTime;

            gameTime = TimeSpan.FromSeconds(elapsedTime * cycleSpeed);

            if (gameTime.TotalHours >= 24)
            {
                elapsedTime = 0;
                gameTime = TimeSpan.FromSeconds(0);
            }
        }
    }
}
