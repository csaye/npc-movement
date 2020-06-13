using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSystem : MonoBehaviour
{
    public static bool paused;

    [Header("References")]
    public InputSystem inputSystem;
    
    private RawImage image;

    void Awake()
    {
        inputSystem = new InputSystem();
        inputSystem.Menu.Escape.performed += ctx => TriggerPause();
    }

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<RawImage>();
        }
    }

    void TriggerPause()
    {
        paused = !paused;
        image.enabled = paused;
    }

    void OnEnable()
    {
        inputSystem.Enable();
    }

    void OnDisable()
    {
        inputSystem.Disable();
    }
}
