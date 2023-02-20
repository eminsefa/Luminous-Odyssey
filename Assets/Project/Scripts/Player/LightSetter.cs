using System;
using System.Collections;
using System.Collections.Generic;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSetter : Singleton<LightSetter>
{
    private static readonly int s_LightPos          = Shader.PropertyToID("_LightPos");
    private static readonly int s_LightRange        = Shader.PropertyToID("_LightRange");
    private static readonly int s_VisibilityFalloff = Shader.PropertyToID("_VisibilityFalloff");

    private LightVariables m_LightVars => GameConfig.Instance.Movement.LightVars;

    private float    m_ScreenHeight;
    private float    m_Brightness = 1;
    public  Material LightReverseEffectedMat { get; set; }
    
    public  float    BrightnessFactor        => m_LightVars.BrightnessCurve.Evaluate(m_Brightness);

    [SerializeField] private P3dPaintSphere m_PaintSphere;
    [SerializeField] private Material       m_LightEffectedMat;

    public override void Awake()
    {
        base.Awake();
        m_ScreenHeight = Screen.height;
    }

    private void Update()
    {
        calculateBrightness();
        setLight();
        setPaint();
    }

    private void calculateBrightness()
    {
        var delta = PlayerMovement.Instance.Velocity * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);
    }

    private void setLight()
    {
        m_LightEffectedMat.SetVector(s_LightPos, transform.position-Vector3.up*(m_ScreenHeight/4f-12));
        m_LightEffectedMat.SetFloat(s_LightRange,        BrightnessFactor * 300);
        m_LightEffectedMat.SetFloat(s_VisibilityFalloff, m_LightVars.VisibilityFalloff);

        if (LightReverseEffectedMat != null)
        {
            LightReverseEffectedMat.SetVector(s_LightPos, transform.position+Vector3.up*3);
            LightReverseEffectedMat.SetFloat(s_LightRange, BrightnessFactor * 10);
        }
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = BrightnessFactor * 7.5f;
    }
    
    private void OnApplicationQuit()
    {
        m_Brightness       = 1;
        transform.position = Vector3.zero;
        setLight();
    }
}