using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSetter : MonoBehaviour
{
    private static readonly int s_LightPos          = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange        = Shader.PropertyToID("_LightRange");
    private static readonly int s_VisibilityFalloff = Shader.PropertyToID("_VisibilityFalloff");

    private LightVariables m_LightVars => GameConfig.Instance.Movement.LightVars;

    private float m_Brightness = 1;

    [SerializeField] private Material m_LightEffectedMat;

    private void Update()
    {
        calculateBrightness();
        setLight();
    }

    private void calculateBrightness()
    {
        var delta = PlayerMovement.Instance.Velocity * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);
    }

    private void setLight()
    {
        m_LightEffectedMat.SetVector(s_LightPos, transform.position);
        m_LightEffectedMat.SetFloat(s_LightRange,        m_LightVars.BrightnessCurve.Evaluate(m_Brightness));
        m_LightEffectedMat.SetFloat(s_VisibilityFalloff, m_LightVars.VisibilityFalloff);
    }

    private void OnApplicationQuit()
    {
        m_Brightness       = 1;
        transform.position = Vector3.zero;
        setLight();
    }
}