using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorSetter : MonoBehaviour
{
    private ColorVariables m_ColorVars => GameConfig.Instance.Movement.ColorVars;

    private float m_Brightness = 1;

    [SerializeField] private Light2D m_Light;
    // [SerializeField] private Material m_PlayerMat;

    private void Update()
    {
        var delta = PlayerMovement.Instance.Velocity * m_ColorVars.BrightenSpeed - m_ColorVars.DarkenSpeed;
        m_Brightness                  += delta * Time.deltaTime;
        m_Brightness                  =  Mathf.Clamp01(m_Brightness);
        m_Light.pointLightOuterRadius =  m_ColorVars.BrightnessCurve.Evaluate(m_Brightness);
        // MK.Toon.Properties.brightness.SetValue(m_PlayerMat, m_ColorVars.BrightnessCurve.Evaluate(m_Brightness));
    }

    private void OnApplicationQuit()
    {
        // MK.Toon.Properties.brightness.SetValue(m_PlayerMat, 1);
    }
}