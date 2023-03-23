using System;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : MonoBehaviour
{
    public static List<Transform> ActiveLightObjects = new();

    protected virtual void OnEnable()
    {
        ActiveLightObjects.Add(transform);
    }

    protected virtual void OnDisable()
    {
        ActiveLightObjects.Remove(transform);
    }
}