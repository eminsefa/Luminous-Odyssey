using System;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerLight : LightObject
{
    public static float BrightnessFactor { get; private set; }

    private float m_Brightness = 1;

    private LightVariables m_LightVars => GameConfig.Instance.LightVars;

    [FoldoutGroup("Refs")] [SerializeField]
    private Transform m_LightCircleMask;

    [FoldoutGroup("Refs")] [SerializeField]
    private P3dPaintSphere m_PaintSphere;

    private void FixedUpdate()
    {
        setPaint();
    }

    private void LateUpdate()
    {
        calculateBrightness();
    }

    private void calculateBrightness()
    {
        var delta = PlayerController.Instance.Velocity.sqrMagnitude * m_LightVars.BrightenSpeed - m_LightVars.DarkenSpeed;
        m_Brightness += delta * Time.deltaTime;
        m_Brightness =  Mathf.Clamp01(m_Brightness);

        BrightnessFactor             = m_LightVars.BrightnessCurve.Evaluate(m_Brightness);
        m_LightCircleMask.localScale = Vector3.one * Mathf.Lerp(0, 16f, BrightnessFactor);
    }

    private void setPaint()
    {
        m_PaintSphere.Radius = Mathf.Max(BrightnessFactor * m_LightVars.LightRange * m_LightVars.PaintRangeMult, 0.01f); //paints all when it gets to 0
    }

    private void OnApplicationQuit()
    {
        m_Brightness = 1;
    }
}