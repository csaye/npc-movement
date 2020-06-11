using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeSystem : MonoBehaviour
{
    [SerializeField] public static TimeSpan gameTime;

    [SerializeField] private float elapsedTime;

    void Update()
    {
        UpdateGameTime();
    }

    void UpdateGameTime()
    {
        if (!PauseSystem.paused)
        {
            elapsedTime += Time.deltaTime;

            gameTime = TimeSpan.FromSeconds(elapsedTime * 120);

            if (gameTime.TotalHours >= 24)
            {
                elapsedTime = 0;
                gameTime = TimeSpan.FromSeconds(0);
            }
        }
    }
}
