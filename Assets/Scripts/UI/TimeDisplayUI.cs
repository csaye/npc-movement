using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeDisplayUI : MonoBehaviour
{

    private TMPro.TextMeshProUGUI textField;

    private TimeSpan currentGameTime;

    void Start()
    {
        if (textField == null)
        {
            textField = GetComponent<TMPro.TextMeshProUGUI>();
        }
    }

    void Update()
    {
        TimeSpan gameTime = TimeSystem.gameTime;

        if (gameTime != currentGameTime)
        {
            textField.text = gameTime.ToString("hh':'mm");
            currentGameTime = gameTime;
        }
    }
}
