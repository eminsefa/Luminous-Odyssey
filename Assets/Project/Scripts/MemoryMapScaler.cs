using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MemoryMapScaler : MonoBehaviour
{
    public Camera mainCamera;

    private void Awake()
    {
        setScale();
    }

    private void setScale()
    {
        float screenHeight = mainCamera.orthographicSize * 2.0f;
        float screenWidth  = screenHeight                * mainCamera.aspect;

        transform.localScale = new Vector3(screenWidth, screenHeight, 1.0f);
    }
}